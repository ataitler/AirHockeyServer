using System;

namespace AssemblyCSharp
{
	public enum GameState
	{
		// No player connected
		Disconnected,
		// Players connected, no game in progress
		Idle,
		// beginning an episode of a game
		Starting,
		// game in progress, players are playing
		Playing,
        // not in any game state
        None
	}
}

