## Rushour

### Overview

This a C# implementation of the puzzle game rush hour. I created this to experiment with puzzle enumeration and attempt to generate complex rush hour puzzles.

Unfortunately the code is poorly readable as I never intended to have this published. I still hope that you find this useful as a step stone if you're thinking of creating your own rush hour GUI, solver or enumerator.

I had the enumerator running on a couple of desktop computers for 4 days and managed to generate pretty decent puzzles (including one which appears to be the most difficult standard rush hour puzzle ever published; solvable in 50 steps).

The repository contains another Visual Studio project (NumberOfPossibilities). This is an attempt by a friend of mine to calculate an upper bound on the number of standard rush hour puzzles using dynamic programming. We haven't thought this through at the time but I thought I should still point this out as an interesting programming problem.

### Features

1. Puzzle visualization with mouse control and animated solving
2. Puzzle string representation
3. Simple state space explorer
4. Solver
5. Enumerator

### Quick start

1. Clone the repo: `git clone git://github.com/gtarawneh/rushhour.git`. 
2. Open the Visual Studio Solution file RushHour.sln

### Screenshot

![Screenshot](https://raw.github.com/gtarawneh/rushhour/master/screenshots/screenshot1.png "GUI")