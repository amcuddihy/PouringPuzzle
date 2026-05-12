namespace PouringPuzzle.Core.Models;

public class SetupVessel 
{
    public event EventHandler? ValueChanged;

    private const int MAX_SIZE = 999; // Arbitrary limit to prevent UI issues and performance problems. Even at this limit, the vessel
                                      // renders impractically large in the UI. Int.MaxValue would be completely unmanageable. I considered
                                      // setting it even lower, such as 99, but I don't want to prevent users from creating puzzles with
                                      // large vessels if they really want to. The UI won't render them well, but it also won't break. 

    public int MaxValue {
        get {
            return _maxValue;
        }
        set { 
            if (value == _maxValue) {
                return;
            }
            _maxValue = Math.Clamp(value, 0, MAX_SIZE);
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public int StartingValue { 
        get {
            return _startingValue;
        }
        set { 
            if (value == _startingValue) {
                return;
            }
            _startingValue = Math.Clamp(value, 0, _maxValue);
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private int _maxValue;
    private int _startingValue;

    public SetupVessel(int maxValue, int startingValue) {
        _maxValue = maxValue;
        _startingValue = startingValue;
    }
}
