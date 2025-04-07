using PouringPuzzle.Models;
using System.ComponentModel;
using System.Diagnostics;

namespace PouringPuzzle.ViewModels;

public class PuzzleSetupViewModel 
{
    public event Action? SetupChanged;

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
                var vessel = new Vessel();
                vessel.PropertyChanged += OnVesselPropertyChanged;
                vessel.Max = 1;
                Vessels.Add(vessel);
            }
            while (Vessels.Count > _vesselCount) {
                var vessel = Vessels.Last();
                vessel.PropertyChanged -= OnVesselPropertyChanged;
                Vessels.Remove(vessel);
            }

            SetupChanged?.Invoke();
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

            SetupChanged?.Invoke();
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

    private void OnVesselPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        SetupChanged?.Invoke();
    }

    public PuzzleSetupViewModel() {

        Vessels = [
            new Vessel {
                Max = 8,
                Value = 8
            },
            new Vessel {
                Max = 5,
                Value = 0,
            },
            new Vessel {
                Max = 3,
                Value = 0
            },
        ];

        foreach(var vessel in Vessels) {
            vessel.PropertyChanged += OnVesselPropertyChanged;
        }
    }
}
