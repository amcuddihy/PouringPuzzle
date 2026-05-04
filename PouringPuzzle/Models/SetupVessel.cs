namespace PouringPuzzle.Models;

public class SetupVessel 
{
    public event EventHandler? ValueChanged;

    public int MaxValue {
        get {
            return _maxValue;
        }
        set { 
            if (value == _maxValue) {
                return;
            }
            _maxValue = value;
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
            _startingValue = value;
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
