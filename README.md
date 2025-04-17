# Water Pouring Puzzle Solver 🧪💧

An interactive web app for experimenting with and solving water jug puzzles - like the classic 8-5-3 setup that was featured in the "Dispersal Cartridge" puzzle in *Resident Evil 2 (2019)* and the park fountain bomb defusal scene in *Die Hard with a Vengeance*.

Built in C# and .NET 9, this Blazor app uses a breadth-first state space search algorithm to calculate the optimum path to the desired volume. 

## Features 🚀

- Solve any water pouring puzzle - not just the 8-5-3 classic
- Configure vessel sizes, starting amounts, and goal value
- Optional **Faucet and Drain** mode for puzzles with an infinite water source
- Suggest the best possible move for the current puzzle state 
- Able to display the optimum route to the goal
- Clean, interactive UI powered by Blazor and two-way data binding
- Self-hosted on a Raspberry Pi for live testing and deployment experience

## Installation 💻

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Visual Studio 2022 or later (recommended)

### Installation Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/amcuddihy/PouringPuzzle

2. Open the project in Visual Studio.

3. Build and run the application.

## Usage ⚡

1. **Customize your puzzle setup**  
   - Set the number of vessels, each vessel's maximum capacity, and its starting fill level.  
   - Optionally, enable **Faucet and Drain mode**, which treats the first vessel as an infinite source and sink.

2. **Set your goal**  
   - Choose a target amount of water to reach in any one vessel.

3. **Run the puzzle**  
   - The app will automatically compute and display the shortest sequence of pour actions to reach the goal.
   - You can step through the solution manually using the **"Pour"** button.

4. **Experiment freely**  
   - Change the setup at any time to create new puzzles - the solver updates in real time.

## Screenshots 🖼️

![Classic Setup](Screenshots/default_setup.png)

![Custom Setup](Screenshots/custom_setup.png)

![Solution Shown](Screenshots/show_solution.png)

(Coming soon — consider adding a screenshot of the main UI or a GIF of it solving a puzzle)
Contributing 🤝

Contributions are welcome!

    Fork this repository

    Create a feature branch:

git checkout -b my-feature-branch

Commit your changes:

    git commit -m "Add cool feature"

    Push to your branch and open a pull request!

License 📜

This project is licensed under the MIT License – see the LICENSE file for details.
Contact 📬

For questions or feedback, feel free to open an issue or reach out through GitHub.

⭐ If you find this project useful, consider giving it a star! ⭐