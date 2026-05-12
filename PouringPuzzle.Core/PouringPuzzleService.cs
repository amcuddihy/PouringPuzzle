using PouringPuzzle.Core.Models;

namespace PouringPuzzle.Core;

public class PouringPuzzleService 
{
    public PuzzleGame CreateNewGame(List<int> maxValues, List<int> startingValues, bool useTapAndDrain, int goalValue) {
        var startingNode = new PuzzleNode(startingValues);
        var newGame = new PuzzleGame(startingValues, maxValues, useTapAndDrain, goalValue);

        GeneratePuzzleGameNodeList(newGame, startingNode);
        SetNodeDistancesFromGoal(newGame, goalValue);

        return newGame;
    }

    public PuzzleMove GetSuggestedMove(PuzzleGame game) {
        if (game.CurrentNode == null || game.CurrentNode.DescendantNodes.Count == 0) {
            throw new InvalidOperationException("Current node is null or has no descendants, cannot suggest move.");
        }

        // This should never happen if the node list is generated correctly, and checking for it in the minimum distance loop is very cumbersome
        // since -1 is always less than any valid distance, so I'm just going to throw an exception if it somehow happens.
        foreach (var descendant in game.CurrentNode.DescendantNodes) {
            if (descendant.DistanceFromGoal == -1) {
                throw new InvalidDataException("A descendant node has a distance of -1, which should not be possible if the node list was generated correctly.");
            }
        }

        var closestDescendantToGoal = game.CurrentNode.DescendantNodes[0];
        for (var i = 1; i < game.CurrentNode.DescendantNodes.Count; i++) {
            if (game.CurrentNode.DescendantNodes[i].DistanceFromGoal < closestDescendantToGoal.DistanceFromGoal) {
                closestDescendantToGoal = game.CurrentNode.DescendantNodes[i];
            }
        }

        // Now we have the best descendant, but the UI needs the move (which vessel to pour into which other vessel) and not the descendant node that will result
        // from that move. This is calculated by comparing the vessel values of the current node and the best descendant node to see which vessel values changed.
        // Only one vessel will have increased (the "best move to" vessel) and only one will have decreased (the "best move from" vessel).
        var bestMove = new PuzzleMove();
        for (int i = 0; i < game.CurrentNode.VesselValues.Count; i++) {
            if (closestDescendantToGoal.VesselValues[i] - game.CurrentNode.VesselValues[i] > 0) {
                bestMove.To = i;
            }
            else if (closestDescendantToGoal.VesselValues[i] - game.CurrentNode.VesselValues[i] < 0) {
                bestMove.From = i;
            }
        }

        return bestMove;
    }

    public void MakeAMove(PuzzleGame game, PuzzleMove move) {
        // This means something has gone terribly wrong, since the current node is set before the UI is rendered. 
        // Just crash and let the user reload the page if they really want to keep going. 
        // I suppose that in the future it would be good to recover from this, since this will be a web app and
        // I don't want the server crashing every time something crazy happens on the client side. 
        if (game.CurrentNode == null) {
            throw new InvalidOperationException("Current node is null, cannot make a move.");
        }

        // Depending on the UI, an invalid move input could be possible, and the app needs to be able to hand this without crashing, since
        // it's a user error and not a programming error, and I don't want the app crashing every time a user accidentally inputs an invalid move.
        if (move.To == move.From ||
            move.From < 0 ||
            move.From >= game.CurrentNode.VesselValues.Count ||
            move.To < 0 ||
            move.To >= game.CurrentNode.VesselValues.Count) {
            return;
        }

        // The amount to pour is the minimum of the amount available in the "from" vessel and the amount of space left in the "to" vessel.
        int pourAmount = Math.Min(game.CurrentNode.VesselValues[move.From], game.MaxValues[move.To] - game.CurrentNode.VesselValues[move.To]);

        // Again, this should be impossible if the move is valid, but I'm not sure if the UI will always give valid moves, and I don't want the
        // app crashing every time a user accidentally inputs an invalid move, so I'll just return if this happens.
        if (pourAmount <= 0) {
            return;
        }

        var nextNodeValues = new List<int>();
        for (int i = 0; i < game.CurrentNode.VesselValues.Count; i++) {
            if (i == move.From) {
                nextNodeValues.Add(game.CurrentNode.VesselValues[i] - pourAmount);
            }
            else if (i == move.To) {
                nextNodeValues.Add(game.CurrentNode.VesselValues[i] + pourAmount);
            }
            else {
                nextNodeValues.Add(game.CurrentNode.VesselValues[i]);
            }
        }

        // Setting the current node to the calculated new node is how the game state is advanced. The UI will respond to the change in the current node
        // by updating the display to show the new vessel values, and the suggested move will update based on the new current node as well.
        game.CurrentNode = GetNodeReferenceByValues(nextNodeValues, game);
    }

    private void GeneratePuzzleGameNodeList(PuzzleGame gameToGenerate, PuzzleNode startNode) {
        gameToGenerate.CurrentNode = startNode;
        gameToGenerate.NodeList.Add(startNode);

        var nodeSearchQueue = new Queue<PuzzleNode>();
        nodeSearchQueue.Enqueue(startNode);

        // This is a breadth first search of the node tree, starting with the initial node, then all of its descendants, then all of their descendants, etc.
        while (nodeSearchQueue.Count > 0) {

            var currentNode = nodeSearchQueue.Dequeue();

            // Descedant value sets are the one thing that can be calculated, so they are used for the initial graph generation.
            // Ancestor values cannot be calculated (at least I couldn't figure it out) so these are set by reference when the descendant node objects are created.
            // In the returned List<List<int>>, the outer list is the list of descendant nodes, and the inner list is the vessel values for that descendant node.
            var descendantValueSets = CalculateDescendantValueSets(gameToGenerate, currentNode);

            foreach (var descendantValueSet in descendantValueSets) {

                // Multiple nodes will have the same node as a descendant, so it's very likely (in fact it's probably guaranteed) that the same descendant value
                // set will be generated multiple times. This check prevents duplicate nodes with the same vessel values from being added to the node list, 
                // which would cause problems as the value set is used as a node's "key" when searching the game's node list.
                var duplicateValueSet = false;
                foreach (var existingNode in gameToGenerate.NodeList) {
                    if (VesselValuesAreEqual(gameToGenerate, descendantValueSet, existingNode.VesselValues)) {
                        duplicateValueSet = true;
                        break;
                    }
                }

                // Create a descendant node for the descendant value set if it doesn't already exist, and add it to the game's node list and the search queue. 
                // This is the ONLY place that descendant nodes are created with their constructor and added to the game's node list. Everywhere else needs to
                // use the GetNodeReferenceByValues method to get a reference to the existing node using it's vessel value set. Otherwise you are creating a
                // new node object with the same vessel values as an existing node, but no ancestor/descendant relationships. 
                if (!duplicateValueSet) {
                    var newDescendantNode = new PuzzleNode(descendantValueSet);

                    gameToGenerate.NodeList.Add(newDescendantNode);
                    nodeSearchQueue.Enqueue(newDescendantNode);
                }

                // At this point, we know the descendant node exists, either because it was just created or because it was created earlier in the search.
                // We need to get a reference to it so we can set the ancestor/descendant relationship between it and the current node.
                var descNode = GetNodeReferenceByValues(descendantValueSet, gameToGenerate);

                descNode.AncestorNodes.Add(currentNode); // This makes the distance setting algorithm possible, since it allows us to traverse the graph in reverse.
                currentNode.DescendantNodes.Add(descNode); // This is used to make moves and to suggest moves, since every valid move leads to a descendant node.
            }
        }
    }

    private void SetNodeDistancesFromGoal(PuzzleGame gameToProcess, int goal) {
        foreach (var node in gameToProcess.NodeList) {
            for (int i = 0; i < node.VesselValues.Count; i++) {
                // Any vessel value matching the goal value means this is a goal node.
                // The only exception is the tap and drain vessel, which is the first vessel in a tap and drain setup
                if (node.VesselValues[i] == goal && (i != 0 || gameToProcess.UsingTapAndDrain == false)) {
                    // The goal node is the goal, so it's distance from the goal is 0. This is also where the distance calculation for all 
                    // other nodes starts. 
                    node.DistanceFromGoal = 0;
                    var distanceSetQueue = new Queue<PuzzleNode>();
                    distanceSetQueue.Enqueue(node);

                    while (distanceSetQueue.Count > 0) {
                        var currentNode = distanceSetQueue.Dequeue();

                        var nextDistance = currentNode.DistanceFromGoal + 1;

                        foreach (var ancestor in currentNode.AncestorNodes) {
                            if (ancestor.DistanceFromGoal == -1 || ancestor.DistanceFromGoal > nextDistance) {
                                ancestor.DistanceFromGoal = nextDistance;
                                distanceSetQueue.Enqueue(ancestor);
                            }
                        }
                    }

                    break; // No need to check the other vessel values, since this node is already a goal node, and a double or triple goal node means nothing. 
                }
            }
        }
    }

    public List<List<int>> CalculateDescendantValueSets(PuzzleGame game, PuzzleNode node) {
        var descendantValueSets = new List<List<int>>();
        for (int i = 0; i < game.MaxValues.Count; i++) {
            for (int j = 0; j < game.MaxValues.Count; j++) {

                // i is sending vessel and j is receiving vessel, so when i equals j it has no real world meaning, just skip!
                if (i == j) {
                    continue;
                }

                var nextValueSet = new List<int>(node.VesselValues);
                var amountToPour = 0;

                amountToPour = Math.Min(node.VesselValues[i], game.MaxValues[j] - node.VesselValues[j]);

                // Amount to pour being equal to zero means this is an invalid move, either pouring from an empty vessel or pouring
                // into a full vessel. These invalid moves will create duplicate vessel values in the list without this check. 
                if (amountToPour <= 0) {
                    continue;
                }

                nextValueSet[i] -= amountToPour;
                nextValueSet[j] += amountToPour;

                descendantValueSets.Add(nextValueSet);
            }
        }
        return descendantValueSets;
    }

    private PuzzleNode? GetNodeReferenceByValues(List<int> vesselValues, PuzzleGame gameToSearch) {
        foreach (var node in gameToSearch.NodeList) {
            if (VesselValuesAreEqual(gameToSearch, vesselValues, node.VesselValues)) {
                return node;
            }
        }
        return null;
    }

    private bool VesselValuesAreEqual(PuzzleGame game, List<int> firstVesselValues, List<int> secondVesselValues) {
        if (firstVesselValues.Count != secondVesselValues.Count) {
            return false;
        }

        for (int i = 0; i < firstVesselValues.Count; i++) {
            if (firstVesselValues[i] != secondVesselValues[i]) {
                return false;
            }
        }

        return true;
    }
}
