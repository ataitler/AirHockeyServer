using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class Goal : MonoBehaviour
{
    private GameController gameController;
    private PuckController puckController;

	// Use this for initialization
	void Start ()
    {
        GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null)
        {
            Debug.Log("Cannot find 'GameController' script");
        }
        
        // find the puck
        GameObject puckControllerObject = GameObject.FindWithTag ("Puck");
        if (puckControllerObject != null)
        {
            puckController = puckControllerObject.GetComponent<PuckController>();
        }
        if (puckController == null)
        {
            Debug.Log("Cannot find 'PuckController' script");
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Puck")
        {
            if (other.rigidbody2D.position.x < 0)
                gameController.UpdateScore(Players.Player2);  //human
            else
                gameController.UpdateScore(Players.Player1);  //agent

            Players win = gameController.IsWin();
            if (win == Players.All)
            {
                gameController.UpdateAsyncState(GameState.Starting);
            } else
            {
                gameController.UpdateAsyncState(GameState.Idle);
                gameController.LastWinner = win;
            }
        }
    }

}
