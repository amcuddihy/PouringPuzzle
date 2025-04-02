using System.Text;

namespace PouringPuzzle.Models; 

public class PuzzleNode 
{
    public List<Vessel> Vessels { get; set; } = new List<Vessel>();

    public List<PuzzleNode> AncestorNodes { get; set; } = new List<PuzzleNode>();
    public List<PuzzleNode> DescendantNodes { get; set; } = new List<PuzzleNode>();

    public int Distance { get; set; } = -1;

    public string DebugString {
        get {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append('[');
            foreach (var vessel in Vessels) {
                if (vessel == Vessels.Last()) {
                    stringBuilder.Append(vessel.Value);
                }
                else {
                    stringBuilder.Append(vessel.Value);
                    stringBuilder.Append(',');
                }
            }
            stringBuilder.Append("] A:");
            stringBuilder.Append(AncestorNodes.Count);
            stringBuilder.Append(" D:");
            stringBuilder.Append(DescendantNodes.Count);
            stringBuilder.Append(" Distance:");
            stringBuilder.Append(Distance);

            return stringBuilder.ToString();
        }
    }

    List<int> _vesselValues = new();
    public List<int> VesselValues {
        get {
            if (_vesselValues.Count != Vessels.Count) {
                _vesselValues.Clear();
                foreach (var vessel in Vessels) {
                    _vesselValues.Add(vessel.Value);
                }
            }

            return _vesselValues;
        }
    }

    public bool NodeEquals(PuzzleNode otherNode) {
        if (Vessels.Count != otherNode.Vessels.Count) {
            return false;
        }

        for (int i = 0; i < Vessels.Count; i++) {
            if (Vessels[i].Value != otherNode.Vessels[i].Value) {
                return false;
            }
        }

        return true;
    }

    private List<List<int>> _descendantStates = new();
    public List<List<int>> DescendantStates {
        get {
            if (_descendantStates.Count == 0) { 
                _descendantStates = CalculateDescendantStates();
            }
            return _descendantStates;
        }
    }

    private List<List<int>> CalculateDescendantStates() { 
        var states = new List<List<int>>();
        
        for (int i = 0; i < Vessels.Count; i++) {
            for (int j = 0; j < Vessels.Count; j++) { 
                if(i == j) {
                    continue;
                }

                var pourAmount = Math.Min(Vessels[i].Value, Vessels[j].SpaceAvailable);

                if (pourAmount == 0) {
                    continue;
                }

                var newState = new List<int>(VesselValues);
                newState[i] -= pourAmount;
                newState[j] += pourAmount;
                states.Add(newState);
            }
        }

        return states;
    }
}
