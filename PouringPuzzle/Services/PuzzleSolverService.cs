using PouringPuzzle.Models;
using PouringPuzzle.Utilities;
using System.Diagnostics;

namespace PouringPuzzle.Services;

public class PuzzleSolverService 
{
    public PuzzleMove? GetSuggestedMove(PuzzleNode currentNode, List<int> maxes) {
        var bestDescendantNode = new PuzzleNode();
        foreach (var node in currentNode.DescendantNodes) {
            if (bestDescendantNode.Distance < 0) {
                bestDescendantNode = node;
            }
            else if (bestDescendantNode.Distance > node.Distance) {
                bestDescendantNode = node;
            }
        }
        return GetMoveFromStates(currentNode.VesselValues, bestDescendantNode.VesselValues, maxes);
    }

    private PuzzleMove? GetMoveFromStates(List<int> currentState, List<int> descendantState, List<int> maxes) {
        for (int i = 0; i < currentState.Count; i++) {
            for (int j = 0; j < currentState.Count; j++) {
                if (i == j) {
                    continue;
                }

                List<int> tempState = new List<int>(currentState);
                
                int pourAmount = Math.Min(currentState[i], maxes[j] - currentState[j]);
                tempState[i] -= pourAmount;
                tempState[j] += pourAmount;

                if (PuzzleUtils.StatesAreEqual(tempState, descendantState)) {
                    var move = new PuzzleMove();
                    move.From = i;
                    move.To = j;

                    return move;
                }
            }
        }
        return null;
    }

    public List<PuzzleMove> GetSolution(PuzzleGame game) {
        var moveList = new List<PuzzleMove>();

        var startingNode = game.CurrentNode;

        while (game.CurrentNode.Distance > 0) {
            var nextMove = GetSuggestedMove(game.CurrentNode, new List<int>())!; //game.VesselMaxes)!;
            
            moveList.Add(nextMove);

            game.Pour(nextMove.From, nextMove.To);
        }

        game.CurrentNode = startingNode;
        return moveList;
    }
}
