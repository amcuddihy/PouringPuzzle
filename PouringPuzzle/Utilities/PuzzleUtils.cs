using PouringPuzzle.Models;
using System.Diagnostics;

namespace PouringPuzzle.Utilities;

public static class PuzzleUtils 
{
    public static bool StatesAreEqual(List<int> stateOne, List<int> stateTwo) {
        if (stateOne.Count != stateTwo.Count) {
            return false;
        }

        for (int i = 0; i < stateOne.Count; i++) {
            if (stateOne[i] != stateTwo[i]) {
                return false;
            }
        }

        return true;
    }

    public static bool IsDuplicateState(List<int> stateToTest, List<PuzzleNode> listToTestAgainst) {
        bool duplicate = false;

        foreach (PuzzleNode node in listToTestAgainst) {
            if (StatesAreEqual(stateToTest, node.VesselValues)) {
                duplicate = true;
                break;
            }
        }

        return duplicate;
    }

    public static PuzzleNode GetNodeFromListByState(List<int> state, List<PuzzleNode> nodeList) {
        var puzzleNode = new PuzzleNode();

        foreach (var node in nodeList) {
            if (StatesAreEqual(state, node.VesselValues)) {
                puzzleNode = node;
                break;
            }
        }
        
        return puzzleNode;
    }

    public static void DebugPrintNodeList(List<PuzzleNode> nodeList, string message) {
        Debug.WriteLine(message);
        foreach (var node in nodeList) {
            Debug.WriteLine(node.DebugString);
        }
    }
}
