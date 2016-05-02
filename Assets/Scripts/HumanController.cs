using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class HumanController : MonoBehaviour {

	private GameController gameController = null;
	public float Speed;
    public Camera cam;
    private Vector2 initPosition;
    private Vector2 initVelocity;

    //private Vector3 X_hat;
    //private Vector3 Y_hat;

	// Use this for initialization
	void Start ()
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController>();
		}
		if (gameController == null) {
			Debug.Log("Cannot find 'GameController' script");
		}
        initPosition = new Vector2(1000, 0);
        initVelocity = new Vector2(0, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate()
	{
		if (gameController.State == GameState.Playing)
        {
            float moveHorizontal = Input.GetAxis("Mouse X");
            float moveVertical = Input.GetAxis("Mouse Y");

            Vector2 move;
            if (cam.transform.rotation.z == 0)
                move = new Vector2(moveHorizontal, moveVertical) * Speed;
            else
                move = new Vector2(-moveVertical, moveHorizontal) * Speed;

            rigidbody2D.velocity = move;
            float posX,posY;
            posX = Mathf.Clamp(rigidbody2D.position.x, 0, 1180);
            posY = Mathf.Clamp(rigidbody2D.position.y, -590, 590);
            rigidbody2D.position = new Vector2(posX,posY);

            /*
            Vector2 posNext = rigidbody2D.position + 0.02f * move;
            float clampedX = Mathf.Clamp(posNext.x, 0, 1180);
            float clampedY = Mathf.Clamp(posNext.y, -590, 590);

            rigidbody2D.velocity = move;
            if (clampedX != posNext.x)
            {
                pos.x = clampedX;
                move.x = 0;
                rigidbody2D.position = pos;
                rigidbody2D.velocity = move;
            }
            if (clampedY != posNext.y)
            {
                pos.y = clampedY;
                move.y = 0;
                rigidbody2D.position = pos;
                rigidbody2D.velocity = move;
            } 
            */
        }
        else
        {
            this.rigidbody2D.position = initPosition;
            this.rigidbody2D.velocity = initVelocity;
        }
	}

	Vector2 Accelerate(Rigidbody2D body,Vector2 move)
	{
		Vector2 acc = new Vector2();
		acc.x = move.x*body.mass;
		acc.y = move.y*body.mass;
		return acc;
	}

}
