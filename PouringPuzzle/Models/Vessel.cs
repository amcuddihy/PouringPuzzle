using System.ComponentModel;

namespace PouringPuzzle.Models; 

public class Vessel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private int _value = 0;
    public int Value {
        get {
            if (IsTapAndDrain) { 
                return int.MaxValue;
            }
            return _value;
        }
        set {
            if (Max == -1) { // without this check, Max and Value would need to be set in the correct order (Max first)
                _max = value;
            }
            _value = Math.Clamp(value, 0, Max);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }

    private int _max = -1;
    public int Max {
        get {
            return _max;
        }
        set {
            _max = Math.Max(0, value);
            _value = Math.Min(_value, Max);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Max)));
        }
    }

    public int SpaceAvailable {
        get {
            if (IsTapAndDrain) { 
                return int.MaxValue;
            }
            return Max - Value;
        }
    }

    private bool _isTapAndDrain = false;
    public bool IsTapAndDrain {
        get {
            return _isTapAndDrain;
        }
        set {
            _isTapAndDrain = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsTapAndDrain))); 
        }
    }

    public string VesselString {
        get {
            if (IsTapAndDrain) {
                return "Tap and Drain";
            }
            else {
                return $"[{Value}/{Max}]";
            }
        }
    }
}
