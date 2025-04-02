using PouringPuzzle.Utilities;
using System.Diagnostics;
using System.Xml.Linq;

namespace PouringPuzzle.Models;

public class PuzzleGame 
{
    public int _goal = 4;
    public int Goal {
        get {
            return _goal;
        }
        set {
            _goal = value;
            UpdateNodeDistances();
        }
    }

    public List<PuzzleNode> NodeList { get; set; } = new List<PuzzleNode>();
    public PuzzleNode CurrentNode { get; set; } = new PuzzleNode();

    public void Pour(int send, int receive) {
        if (receive == send) {
            return;
        }

        if (receive < 0 || receive >= CurrentNode.Vessels.Count || 
            send < 0 || send >= CurrentNode.Vessels.Count) {
            return;
        }

        int pourAmount = Math.Min(CurrentNode.Vessels[send].Value, CurrentNode.Vessels[receive].SpaceAvailable);

        if (pourAmount <= 0) {
            return;
        }

        var nextNode = new PuzzleNode();
        nextNode.Vessels = CurrentNode.Vessels;

        nextNode.Vessels[send].Value -= pourAmount;
        nextNode.Vessels[receive].Value += pourAmount;

        CurrentNode = PuzzleUtils.GetNodeFromListByState(nextNode.VesselValues, NodeList);
    }

    public void UpdateNodeDistances() {
        foreach (PuzzleNode node in NodeList) {
            node.Distance = -1;
        }

        foreach (PuzzleNode node in NodeList) {

            var isGoalNode = false;
            foreach (var vessel in node.Vessels) {
                if (vessel.Value == Goal && vessel.IsTapAndDrain == false) {
                    isGoalNode = true;
                    break;
                }
            }

            if (isGoalNode) {
                SetNodeDistancesFromGoal(node);
            }
        }
    }

    private void SetNodeDistancesFromGoal(PuzzleNode goalNode) {
        goalNode.Distance = 0;

        var distanceSetQueue = new Queue<PuzzleNode>();
        distanceSetQueue.Enqueue(goalNode);

        var distanceNodeList = new List<PuzzleNode>();

        while (distanceSetQueue.Count > 0) {
            var currentNode = distanceSetQueue.Dequeue();

            foreach (var ancestor in currentNode.AncestorNodes) {
                if (PuzzleUtils.IsDuplicateState(ancestor.VesselValues, distanceNodeList)) {
                    continue;
                }

                if (ancestor.Distance == -1) { // distance hasn't been set yet
                    ancestor.Distance = currentNode.Distance + 1;
                }
                else {
                    ancestor.Distance = Math.Min(ancestor.Distance, currentNode.Distance + 1);
                }

                distanceNodeList.Add(ancestor);
                distanceSetQueue.Enqueue(ancestor);
            }
        }
    }

    public bool CheckForWin() {
        return CurrentNode.Distance == 0;
    }
}
