using PouringPuzzle.Models;
using PouringPuzzle.Services;

namespace PouringPuzzle.ViewModels; 

public class PuzzleViewModel 
{
    public int Sender { get; set; } = 0;
    public int Receiver { get; set; } = 1;

    public bool ShowVictoryPopup { get; set; } = false;

    public PuzzleGame Game { get; set; } = new PuzzleGame();

    public List<PuzzleMove> SolutionMoves { get; set; } = new List<PuzzleMove>();

    private IPuzzleGeneratorService _puzzleGeneratorService;

    public PuzzleViewModel(IPuzzleGeneratorService puzzleGeneratorService) {
        _puzzleGeneratorService = puzzleGeneratorService;
    }

    public void GeneratePuzzle(List<Vessel> vessels, int goal) {
        Game = _puzzleGeneratorService.GeneratePuzzle(vessels, goal);
        ShowVictoryPopup = Game.CheckForWin(); // just in case a puzzle starts in a victory state
    }

    public void Pour() {
        Game.Pour(Sender, Receiver);
        ShowVictoryPopup = Game.CheckForWin();
    }
}
