using PouringPuzzle.Models;

namespace PouringPuzzle.Services;

public interface IPuzzleSolverService {
    PuzzleMove GetSuggestedMove(PuzzleGame game);
    List<PuzzleMove> GetSolution(PuzzleGame game);
}
