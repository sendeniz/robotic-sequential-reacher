# Robotic sequential reacher
 
General:
This repo contains a fully articulated robotic arm controller solving a sequential reaching task akin to a serial reaction time task, allowing close comparison of different learning algortihmns such as PPO, SAC, Q-learning etc. with human behavior on the same task. Agents are trained in parallel to each other within the same environment as shown below, to speed up training and reduce randomness, when examining performance metrics.

<p align="center">
  <img width="460" height="300" src=/figs/train_env.png?raw=true "Training Environment">
</p>

The target actication sequence can either be 1) a self repeating deterministic sequence or a 2) random activation sequence without repitions.  Comparisong between the two sequences shows that a self repeating sequence results in a circular movement trajectory over the targets to optimize reward, while a random sequence results in an optmimization strategy in which the agents centers its hand sensor location equidistant to all 4 targets as it is unable to predict the targets next location. 

<p align="center">
  <img width="460" height="300" src=/figs/fixed_active_scatter.png?raw=true "Fixed Sequence" title=Fixed sequence 3D point clouds PPO >
	<figcaption>Optional title</figcaption>
</p>

<p align="center">
  <img width="460" height="300" src=/figs/random_active_scatter.png?raw=true "Random Sequence" title=Random sequence 3D point clouds PPO >
</p>



C# scrips: 
"Game_Manager.cs" : manages properties of the game. Target activation sequence, successful reaching to an active target, reward giving and curriculum learning are defined here. If desired random activation can be disable which will result in a deterministic activation sequence in the Unity environment's UI. 
Curriculum learning can as well be disable or enabled in the environment's UI.
	
"Gen_Targets.cs" : manages properties of the targets, like color, the number of targets to be instantiates. If desired targets can initialised at random 	locations, which we used for experimentation but excluded in the present study. Random target location is activated in the unity environment's UI and ticking the
box "random_location. 

"Yates.cs" : Target activation is governed by an identity matrix of size 4 x 4. Here we define a Fisher-Yates shuffle to randomly shuffle the matrix to create a random activation sequence. Results in the same as sampling from a random uniform distribution. The benefit of using the identity matrix and applying a Fisher-Yates shuffle is that we can examine which targets are supposed to be inactive as these are codes 0 and active targets are coded 1.
	
"Noise.cs" : A Box-Muller transformation is applied to sample from a gaussian normal. This script is never called, but used for initial experimentation.
