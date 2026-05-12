namespace PouringPuzzle.Core.Models;

public class PuzzleNode 
{
    public List<PuzzleNode> AncestorNodes { get; } = [];
    public List<PuzzleNode> DescendantNodes { get; } = [];

    public int DistanceFromGoal { get; set; } = -1;

    public List<int> VesselValues { get; private set; }

    public PuzzleNode(List<int> vesselValues) {
        VesselValues = vesselValues;
    }
}
