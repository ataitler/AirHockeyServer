using System;

namespace AssemblyCSharp
{
	public enum Command
	{
		//Log into the server
		Login,
		//Logout of the server - irrelevent
		Logout,
		//Send a text message to all the chat clients     
		Message,
		//reset game - irrelevent
		Reset,
		//No command
		Null,
		//Start playing
		Start,
		//Pause the game
		Pause,
		//Score update
		Score,
		//Start new game
		NewGame,
		//Game has ended
		EndGame
	}
}

