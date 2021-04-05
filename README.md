# snake-game-AI
A Machine Learning project in Unity to teach the agent how to play the snake game.

<p align="center">
  <img src="https://user-images.githubusercontent.com/7780770/113526251-bd140580-958f-11eb-8462-6f6d0485d746.gif">
</p>

## Running

The project is developed using the Unity MLAgents package, so there are 3 modes that you can run it. To change the mode, `Inspect` the Agent, and open the `Behaviour Parameters` group, then you will be able to select one.

#### Default

This is where you train your agents, the MLAgents package trains them by connecting unity to a python server. 
To setup it, follow the instructions of MLAgent instalation for the unity's `1.9.0` version.
Then, run it via the MLAgents CLI.

#### Inference

In this mode, you can run my pre-trainned brain, or add yours into the `NN-Brain` section of the `Behaviour Parameters` group.

#### Heuristic

To test it, you can run it in the Heuristic mode, where you will be able to test the agent with the input arrows or WASD.

## How it works

Each Agent starts in a random position with the size 3, and for each apple that it takes, it will increase in size. 
If the Agent hit a wall or its own tail, it will die.
The Agents is able to see the walls, its tail, and the apple distance in 360 degrees and 8 directions.
The Agent reward is calculated by the apples that he took, and the steps that he walked to take each one. The highest apples, and the least steps, the bigger the reward.

## Modifying

You can add more agents if you want by adding the `Environment` prefab, or copying the `Environment` instance.
You can also change the game size by modifiying the parameters of the GameManager script, and train the snake in other conditions, or test a trained snake into a bigger or lower map.
