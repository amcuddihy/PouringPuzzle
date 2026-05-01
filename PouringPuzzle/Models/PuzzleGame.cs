namespace PouringPuzzle.Models;

public class PuzzleGame 
{
    public PuzzleNode? CurrentNode { get; set; }
    public List<PuzzleNode> NodeList { get; set; } = [];

    public List<int> StartingValues { get; private set; }
    public List<int> MaxValues { get; private set; }
    public bool UsingTapAndDrain { get; private set; }
    public int GoalValue { get; private set; }

    public PuzzleGame(List<int> startingValues, List<int> maxValues, bool usingTapAndDrain, int goalValue) {
        StartingValues = startingValues;
        MaxValues = maxValues;
        UsingTapAndDrain = usingTapAndDrain;
        GoalValue = goalValue;
    }
}
