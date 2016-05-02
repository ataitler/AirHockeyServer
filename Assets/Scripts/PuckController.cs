using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class PuckController : MonoBehaviour {

    public float MaxSpeed;
    private GameController gameController;

	// Use this for initialization
	void Start () {
        GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null)
        {
            Debug.Log("Cannot find 'GameController' script");
        }
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        if (gameController.State == GameState.Playing)
        {
            float ratio = rigidbody2D.velocity.magnitude / MaxSpeed;
            if (ratio > 1)
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x / ratio, rigidbody2D.velocity.y / ratio);
            }
        }
        else
        {
            rigidbody2D.position = new Vector2(0,0);
            rigidbody2D.velocity = new Vector2(0,0);
        }

        //CheckPuckOnBoard();
    }

    void CheckPuckOnBoard()
    {
        if (gameController.State == GameState.Playing)
        {
            if (rigidbody2D.position.x < -1300)
                gameController.UpdateScore(Players.Player2);  //human
            else if (rigidbody2D.position.x > 1300)
                gameController.UpdateScore(Players.Player1);  //agent
            else 
                return;
        
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
