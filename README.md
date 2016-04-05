Project Name: Voice Control for 2D Platformer
Author: Anonymous
Start Date: 3 April 2016
Category: Action, RPG, Game
Keywords: Unity, 2D, RPG, Action, Game, C#, Voice Control, Frequency Spectrum, Fourier Transform, Similarity, Classification, Speech Recognition, Real Time, Time Series
Original Resources Link: https://www.assetstore.unity3d.com/en/#!/content/11228
Original Resources Provider: Unity Technologies
Platform: Unity
Programming Language: C#
Link to Resource: https://onedrive.live.com/redir?resid=62565545DD5BA587!37204&authkey=!AJxF8s24yZclGTo&ithint=folder%2cmd
Link to Demonstration Video: https://www.youtube.com/watch?v=fURhEN6brGo

Overview:
The original project is a sample game from Unity. Player uses key board to control character. I did add voice control functionality as an alternative way to control character. Now, player can speak "Jump" and "Shoot" to control character.

User Guide:
1. Download and Extract "2DPlatformer.zip".
2. Import it to Unity.
3. Run it.

Game Control:
Press and Hold "J" then speak "Jump" to calibrate JUMP command.
Press and Hold "S" then speak "Shoot" to calibrate SHOOT command.
Press left arrow to go the the left.
Press right arrow to go the the right.
(Alternative) Click on the screen to shoot.
(Alternative) Press up arrow to jump.

Release Note:
Version 1.0.0.0 (5 April 2016):
- Edit ./Assets/Scripts/PlayerControl.cs
	* Receive jump command from voice control via VoiceControl.jump variable.
- Edit ./Assets/Scripts/Gun.cs
	* Receive shoot command from voice control via VoiceControl.shoot variable.
- Add ./Assets/Scripts/VoiceControl.cs
 	* Add training function for training JUMP and SHOOT commands.
 	* Add AudioSource from default microphone.
 	* Add getSpectrum method to compute current frame of voice and transform into frequency based representation.
 	* Add prediction module to compute similarity and predict action based on input voice.
