using PouringPuzzle.Core;
using PouringPuzzle.Core.Models;

namespace PouringPuzzle.ViewModels; 

public class PuzzleViewModel 
{
    // Default sender/receiver use vessels 2 and 3 to avoid the "suggest move" button appearing nonfunctional on
    // the first move of the default puzzle, where the optimal move happens to be vessel 1 -> vessel 2.
    private const int DEFAULT_SENDER = 1;
    private const int DEFAULT_RECEIVER = 2;

    private const int MIN_VESSEL_COUNT = 3; // Game doesn't work with less than 3 vessels
    private const int MAX_VESSEL_COUNT = 20; // Arbitrary limit to prevent UI issues and performance problems

    private readonly List<int> DEFAULT_MAX_VALUES = new List<int> { 8, 5, 3 };
    private readonly List<int> DEFAULT_STARTING_VALUES = new List<int> { 8, 0, 0 };
    private readonly bool DEFAULT_USE_TAP_AND_DRAIN = false;
    private readonly int DEFAULT_GOAL = 4;
    
    // This is only for the UI. The service needs the zero ind exed values, so use _sender for those. 
    public int Sender { 
        get {
            return _sender + 1;
        }
        set { 
            _sender = Math.Clamp(value - 1, 0, VesselCount - 1);
        }
    }

    // This is only for the UI. The service needs the zero indexed values, so use _receiver for those.
    public int Receiver {
        get {
            return _receiver + 1;
        }
        set {
            _receiver = Math.Clamp(value - 1, 0, VesselCount - 1);
        }
    }

    public int VesselCount {
        get {
            return _vessels.Count;
        }
        set {
            var newVesselCount = Math.Clamp(value, MIN_VESSEL_COUNT, MAX_VESSEL_COUNT);

            if (newVesselCount == _previousVesselCount) { // No change in vessel count, probably because the value was out of bounds and got clamped.
                return;
            }
            else if (newVesselCount > _previousVesselCount) {
                for (int i = _previousVesselCount; i < newVesselCount; i++) {
                    _vessels.Add(new SetupVessel(1, 0));

                    _vessels.Last().ValueChanged += GeneratePuzzle!;
                    _currentGame.MaxValues.Add(1);
                    _currentGame.StartingValues.Add(0);
                }
            }
            else if (newVesselCount < _previousVesselCount) { // This could technically be an "else" but this is more explicit.
                for (int i = _previousVesselCount - 1; i >= newVesselCount; i--) {
                    _currentGame.MaxValues.RemoveAt(i);
                    _currentGame.StartingValues.RemoveAt(i);
                    _vessels.RemoveAt(i);   
                }
            }

            _previousVesselCount = newVesselCount;
            GeneratePuzzle(_currentGame.MaxValues, _currentGame.StartingValues, _currentGame.UsingTapAndDrain, _currentGame.GoalValue);
        }
    }

    public List<SetupVessel> Vessels {
        get {
            return _vessels;
        }
    }

    public bool UseTapAndDrain {
        get {
            return _currentGame.UsingTapAndDrain;
        }
        set {
            // Converting the tap and drain into a mathematically equivalent normal vessel removes all special case handling logic
            // from the service, except for checking if a node is a victory node. (If the goal value occurs in the tap and drain vessel, it doesn't count)
            if (value) {
                _sinkPreviousMax = _vessels[0].MaxValue;
                _sinkPreviousStarting = _vessels[0].StartingValue;

                var sinkVesselMax = 0;
                var sinkVesselStart = 0;
                for (var i = 1; i < VesselCount; i++) {
                    sinkVesselMax += _vessels[i].MaxValue;
                    sinkVesselStart += _vessels[i].MaxValue - _vessels[i].StartingValue;
                }
                _vessels[0].MaxValue = sinkVesselMax;
                _vessels[0].StartingValue = sinkVesselStart;
            }
            else { 
                _vessels[0].MaxValue = _sinkPreviousMax;
                _vessels[0].StartingValue = _sinkPreviousStarting;
            }

            GeneratePuzzle(_currentGame.MaxValues, _currentGame.StartingValues, value, _currentGame.GoalValue);
        }
    }

    public PuzzleNode CurrentNode {
        get { 
            return _currentGame.CurrentNode!;
        }
    }

    public int Goal {
        get {
            return _currentGame.GoalValue;
        }
        set {
            GeneratePuzzle(_currentGame.MaxValues, _currentGame.StartingValues, _currentGame.UsingTapAndDrain, value);
        }
    }

    public bool PuzzleHasSolution {
        get {
            var hasSolution = false;
            foreach(var node in _currentGame.NodeList) {
                if (node.DistanceFromGoal == 0) {
                    hasSolution = true;
                    break;
                }
            }
            return hasSolution;
        }
    }

    // Only this class should ever show the victory popup, so the setter is private.
    // There is a public method for hiding the victory popup, which the victory popup itself needs. 
    public bool ShowVictoryPopup { get; private set; } = false;

    private readonly PouringPuzzleService _pouringPuzzleService;

    private PuzzleGame _currentGame;
    private List<SetupVessel> _vessels;

    // These are used to reset the first vessel's values when the user changes from a tap and drain setup to a standard one. 
    private int _sinkPreviousMax = 0;
    private int _sinkPreviousStarting = 0;

    private int _previousVesselCount;
    private int _sender;
    private int _receiver;

    public PuzzleViewModel(PouringPuzzleService pouringPuzzleService) {
        _pouringPuzzleService = pouringPuzzleService;

        _sender = DEFAULT_SENDER;
        _receiver = DEFAULT_RECEIVER;

        _currentGame = _pouringPuzzleService.CreateNewGame(DEFAULT_MAX_VALUES, DEFAULT_STARTING_VALUES, DEFAULT_USE_TAP_AND_DRAIN, DEFAULT_GOAL);
        _previousVesselCount = _currentGame.MaxValues.Count;

        _vessels = [];
        for (int i = 0; i < _currentGame.MaxValues.Count; i++) {
            var vessel = new SetupVessel(_currentGame.MaxValues[i], _currentGame.StartingValues[i]);
            vessel.ValueChanged += GeneratePuzzle!;
            _vessels.Add(vessel);
        }
    }

    public void GeneratePuzzle(object sender, EventArgs e) {
        _currentGame.MaxValues.Clear();
        _currentGame.StartingValues.Clear();
        foreach(var vessel in _vessels) {
            _currentGame.MaxValues.Add(vessel.MaxValue);
            _currentGame.StartingValues.Add(vessel.StartingValue);
        }
        GeneratePuzzle(_currentGame.MaxValues, _currentGame.StartingValues, _currentGame.UsingTapAndDrain, _currentGame.GoalValue);
    }

    public void GeneratePuzzle(List<int> maxValues, List<int> startingValues, bool useTapAndDrain, int goal) {
        _currentGame = _pouringPuzzleService.CreateNewGame(maxValues, startingValues, useTapAndDrain, goal);
    }

    public void Pour() {
        var puzzleMove = new PuzzleMove { 
            From = _sender, 
            To = _receiver 
        };

        _pouringPuzzleService.MakeAMove(_currentGame, puzzleMove);
        if (_currentGame.CurrentNode!.DistanceFromGoal == 0) {
            ShowVictoryPopup = true;
        }
    }

    public void SuggestMove() {
        var puzzleMove = _pouringPuzzleService.GetSuggestedMove(_currentGame);
        _sender = puzzleMove.From;
        _receiver = puzzleMove.To;
    }

    // The @onclick of the victory popup needs a method to call to hide the popup.
    // It can't set the property directly, even if the property's setter was public.
    public void HideVictoryPopup() {
        ShowVictoryPopup = false;
    }   
}
