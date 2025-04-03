using PouringPuzzle.Models;
using PouringPuzzle.Utilities;
using System.Diagnostics;

namespace PouringPuzzle.Services;

public class PuzzleSolverService : IPuzzleSolverService
{
    public PuzzleMove GetSuggestedMove(PuzzleGame Game) {
        var currentBestDescendant = new PuzzleNode();
        foreach (var descendant in Game.CurrentNode.DescendantNodes) {
            if (currentBestDescendant.Distance == -1) {
                currentBestDescendant = descendant;
            }
            else if (currentBestDescendant.Distance > descendant.Distance) {
                currentBestDescendant = descendant;
            }
        }

        return GetMoveFromStates(Game.CurrentNode.VesselValues, currentBestDescendant.VesselValues);
    }

    private PuzzleMove GetMoveFromStates(List<int> currentState, List<int> descendantState) {
        var move = new PuzzleMove();
        for (int i = 0; i < currentState.Count; i++) {
            if(descendantState[i] - currentState[i] > 0) {
                move.To = i;
            }
            else if (descendantState[i] - currentState[i] < 0) {
                move.From = i;
            }
        }
        return move;
    }

    public List<PuzzleMove> GetSolution(PuzzleGame game) {
        var moveList = new List<PuzzleMove>();

        var startingNode = game.CurrentNode;

        while (game.CurrentNode.Distance > 0) {
            var nextMove = GetSuggestedMove(game);
            
            moveList.Add(nextMove);

            game.Pour(nextMove.From, nextMove.To);
        }

        game.CurrentNode = startingNode;
        return moveList;
    }
}
