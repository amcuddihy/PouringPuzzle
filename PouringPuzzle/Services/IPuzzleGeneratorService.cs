using PouringPuzzle.Models;

namespace PouringPuzzle.Services;

public interface IPuzzleGeneratorService {
    public PuzzleGame GeneratePuzzle(List<Vessel> vessels, int goal);
}
