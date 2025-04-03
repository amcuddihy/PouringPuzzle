using PouringPuzzle.Models;
using PouringPuzzle.Utilities;
using System.Diagnostics;
using System.Security.Cryptography;

namespace PouringPuzzle.Services;

public class PuzzleGeneratorService  : IPuzzleGeneratorService
{
    public PuzzleGame GeneratePuzzle(List<Vessel> vessels, int goal) {
        var startingNode = new PuzzleNode();
        startingNode.Vessels = vessels;

        var game = new PuzzleGame();
        game.CurrentNode = startingNode;
        game.NodeList = GenerateNodeList(startingNode);
        game.Goal = goal;

        return game;
    }

    private List<PuzzleNode> GenerateNodeList(PuzzleNode startNode) {
        var nodeList = new List<PuzzleNode>();
        var nodeSearchQueue = new Queue<PuzzleNode>();

        Debug.WriteLine(startNode.DebugString);

        nodeList.Add(startNode);
        nodeSearchQueue.Enqueue(startNode);

        while (nodeSearchQueue.Count > 0) { 
            var currentNode = nodeSearchQueue.Dequeue();
            foreach (var descState in currentNode.DescendantStates) {
                if (!PuzzleUtils.IsDuplicateState(descState, nodeList)) {
                    var newNode = new PuzzleNode();
                    
                    for (int i = 0; i < descState.Count; i++) {
                        var vessel = new Vessel();
                        if (currentNode.Vessels[i].IsTapAndDrain) {
                            vessel.IsTapAndDrain = true;
                        }
                        else {
                            vessel.Value = descState[i];
                            vessel.Max = currentNode.Vessels[i].Max;
                        }

                        newNode.Vessels.Add(vessel);
                    }
                    Debug.WriteLine($"Next Node: {newNode.DebugString}");

                    nodeList.Add(newNode);
                    nodeSearchQueue.Enqueue(newNode);
                }

                var descNode = PuzzleUtils.GetNodeFromListByState(descState, nodeList);
                descNode.AncestorNodes.Add(currentNode);
                currentNode.DescendantNodes.Add(descNode);
            }
        }

        return nodeList;
    }

    
}
