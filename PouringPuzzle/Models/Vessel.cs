
namespace PouringPuzzle.Models; 

public class Vessel 
{
    private int _value = 0;
    public int Value {
        get {
            return _value;
        }
        set {
            if (Max == -1) {
                _max = value;
            }
            _value = Math.Clamp(value, 0, Max);
        }
    }

    private int _max = -1;
    public int Max {
        get {
            return _max;
        }
        set {
            _max = Math.Max(0, value);
            _value = Math.Min(Value, Max);
        }
    }

    public int SpaceAvailable {
        get {
            return Max - Value;
        }
    }

    public bool IsTapAndDrain { get; set; } = false;

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
