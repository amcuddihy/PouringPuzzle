using PouringPuzzle.Models;
using PouringPuzzle.Services;

namespace PouringPuzzle.ViewModels; 

public class PuzzleViewModel 
{
    private int _sender = 0;
    public int Sender { 
        get {
            return _sender;
        }
        set { 
            _sender = Math.Clamp(value, 0, Game.CurrentNode.Vessels.Count - 1);
        }
    }
    
    private int _receiver = 1;
    public int Receiver {
        get {
            return _receiver;
        }
        set {
            _receiver = Math.Clamp(value, 0, Game.CurrentNode.Vessels.Count - 1);
        }
    }

    public List<Vessel> Vessels { get; set; } = new List<Vessel>();

    public bool ShowVictoryPopup { get; set; } = false;

    public PuzzleGame Game { get; set; } = new PuzzleGame();

    public List<PuzzleMove> SolutionMoves { get; set; } = new List<PuzzleMove>();

    private IPuzzleGeneratorService _puzzleGeneratorService;
    private IPuzzleSolverService _puzzleSolverService;

    public PuzzleViewModel(IPuzzleGeneratorService puzzleGeneratorService, IPuzzleSolverService puzzleSolverService) {
        _puzzleGeneratorService = puzzleGeneratorService;
        _puzzleSolverService = puzzleSolverService;
    }

    public void GeneratePuzzle(List<Vessel> vessels, int goal) {
        Game = _puzzleGeneratorService.GeneratePuzzle(vessels, goal);
        Vessels = Game.CurrentNode.Vessels;
        ShowVictoryPopup = Game.CheckForWin(); // just in case a puzzle starts in a victory state
    }

    public void Pour() {
        Game.Pour(Sender, Receiver);
        Vessels = Game.CurrentNode.Vessels;
        ShowVictoryPopup = Game.CheckForWin();
    }

    public void SuggestMove() {
        var puzzleMove = _puzzleSolverService.GetSuggestedMove(Game);
        Sender = puzzleMove.From;
        Receiver = puzzleMove.To;
    }

    public void ShowSolution() {
        SolutionMoves = _puzzleSolverService.GetSolution(Game);
    }
}
