# RL_SRT
 
General:
The interaction with the Unity environment itself is conducted via c# scripts. As 	all physic simulation operations are implemented
using c# scripts and the neural 	network controllers via pytorch and python. Unity provides a robust API between 	the Unity3D engine
for physics simulation and PyTorch.
As such the folder "scripts_folder" divides c# files and python files into two 		subfolders and all scripts used can be found there. 


C# scrips: 
"Game_Manager.cs" : manages properties of the game. Target activation sequence, 
successful reaching to an active target, reward giving and curriculum learning are 	defined here. 
If desired random activation can be disable which will result in a 
deterministic activation sequence in the Unity environment's UI. Curriculum
learning can as well be disable or enabled in the environment's UI.
	
"Gen_Targets.cs" : manages properties of the targets, like color, the number of 	targets to be instantiates. If desired targets can initialised at random 		locations, which we used for experimentation but excluded in the present study. 
Random target location is activated in the unity environment's UI and ticking the 	box "random_location. 

"Yates.cs" : Target activation is governed my an identity matrix of size 4 x 4. 	Here we define a Fisher-Yates shuffle to randomly shuffle the matrix to create a 	random activation sequence. Results in the same as sampling from a random uniform 	distribution. The benefit of using the identity matrix and applying a Fisher-Yates 	shuffle is that we can examine which targets are supposed to be inactive as these 	are codes 0 and active targets are coded 1.
	
"Noise.cs" : A Box-Muller transformation is applied to sample from a gaussian 	
normal. We experimented with this, but don't use it in our experiments.
