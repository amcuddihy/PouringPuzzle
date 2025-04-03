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

        var nextState = new List<int>(CurrentNode.VesselValues);

        if(!CurrentNode.Vessels[send].IsTapAndDrain) {
            nextState[send] -= pourAmount;
        }

        if (!CurrentNode.Vessels[receive].IsTapAndDrain) {
            nextState[receive] += pourAmount;
        }

        CurrentNode = PuzzleUtils.GetNodeFromListByState(nextState, NodeList);
    }

    public void UpdateNodeDistances() {
        foreach (PuzzleNode node in NodeList) {
            node.Distance = -1;
        }

        foreach (PuzzleNode node in NodeList) {
            if (node.IsGoalNode(Goal)) {
                node.Distance = 0;
                SetNodeDistancesFromGoal(node);
            }
        }
    }

    private void SetNodeDistancesFromGoal(PuzzleNode goalNode) {
        var distanceSetterQueue = new Queue<PuzzleNode>();
        distanceSetterQueue.Enqueue(goalNode);

        var distanceSetterList = new List<PuzzleNode>();
        distanceSetterList.Add(goalNode);

        while (distanceSetterQueue.Count > 0) {
            var currentNode = distanceSetterQueue.Dequeue();
            foreach (var ancestor in currentNode.AncestorNodes) {
                if (PuzzleUtils.IsDuplicateState(ancestor.VesselValues, distanceSetterList)) {
                    continue;
                }

                if (ancestor.Distance == -1) {
                    ancestor.Distance = currentNode.Distance + 1;
                }
                else { 
                    ancestor.Distance = Math.Min(ancestor.Distance, currentNode.Distance + 1);
                }

                distanceSetterQueue.Enqueue(ancestor);
                distanceSetterList.Add(ancestor);
            }
        }
    }

    public bool CheckForWin() {
        return CurrentNode.Distance == 0;
    }
}
