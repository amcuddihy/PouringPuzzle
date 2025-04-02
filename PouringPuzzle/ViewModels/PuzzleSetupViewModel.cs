using PouringPuzzle.Models;

namespace PouringPuzzle.ViewModels;

public class PuzzleSetupViewModel 
{
    private const int MIN_VESSEL_COUNT = 3;
    private const int MAX_VESSEL_COUNT = 20;

    private const int MIN_GOAL = 1;

    public List<Vessel> Vessels { get; set; }

    private int _vesselCount = 3;
    public int VesselCount {
        get {
            return _vesselCount;
        }
        set {
            _vesselCount = Math.Clamp(value, MIN_VESSEL_COUNT, MAX_VESSEL_COUNT);

            while (Vessels.Count < _vesselCount) {
                Vessels.Add(new Vessel());
            }
            while (Vessels.Count > _vesselCount) {
                Vessels.RemoveAt(Vessels.Count - 1);
            }
        }
    }

    private bool _useTapAndDrain = false;
    public bool UseTapAndDrain {
        get {
            return _useTapAndDrain;
        }
        set {
            _useTapAndDrain = value;

            if (Vessels.Count > 0) {
                Vessels.First().IsTapAndDrain = _useTapAndDrain;
            }
        }
    }

    private int _goal = 4;
    public int Goal {
        get {
            return _goal;
        }
        set {
            var maxSize = 0;
            foreach (var vessel in Vessels) {
                maxSize = Math.Max(maxSize, vessel.Max);
            }

            _goal = Math.Clamp(value, MIN_GOAL, maxSize);
        }
    }

    public PuzzleSetupViewModel() {

        Vessels = new List<Vessel>();
        Vessels.Add(new Vessel
        {
            Max = 8,
            Value = 8
        });
        Vessels.Add(new Vessel
        {
            Max = 5,
            Value = 0,
        });
        Vessels.Add(new Vessel
        {
            Max = 3,
            Value = 0
        });
    }
}
