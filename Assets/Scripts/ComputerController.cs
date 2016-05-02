using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class ComputerController : MonoBehaviour {

	private GameController gameController = null;
	private float xSpeed;
	private float ySpeed;
    private Vector2 initPosition;
    private Vector2 initVeclocity;
	
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

        initPosition = new Vector2(-1000, 0);
        initVeclocity = new Vector2(0, 0);
		
		xSpeed = 0;
		ySpeed = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
    void FixedUpdate()
    {
        if (gameController.State == GameState.Playing)
        {
            // Velocity control - step motors
            this.rigidbody2D.velocity = new Vector2(xSpeed, ySpeed);
            this.rigidbody2D.position = new Vector2(
                Mathf.Clamp(rigidbody2D.position.x, -1180, 0), Mathf.Clamp(rigidbody2D.position.y, -590, 590)
            );
        }
        else
        {
            xSpeed = 0;
            ySpeed = 0;
            this.rigidbody2D.position = initPosition;
            this.rigidbody2D.velocity = initVeclocity;
        }
    }

	public void UpdateSpeed(float xVel, float yVel)
	{
        float acTh = 100;
		if (gameController.State == GameState.Playing)
		{
            // first order filter for the velocity
            //xSpeed = xSpeed + 0.01f * 50 * (xVel - xSpeed);
            //ySpeed = ySpeed + 0.01f * 50 * (yVel - ySpeed);

            // velocity saturation
            if (Mathf.Abs(xVel - xSpeed) > acTh)
                xSpeed = xSpeed + Mathf.Sign(xVel-xSpeed) * acTh;
            if (Mathf.Abs(yVel - ySpeed) > acTh)
                ySpeed = ySpeed + Mathf.Sign(yVel-ySpeed) * acTh;

            // infinite bandwidth plant
			//xSpeed = xVel;
			//ySpeed = yVel;

		}
	}
	
	public void Reset()
	{
        xSpeed = 0;
        ySpeed = 0;
		rigidbody2D.position = new Vector2 (-1000, 0);
		rigidbody2D.angularVelocity = 0;
		rigidbody2D.velocity = new Vector2 (0, 0);
	}

}
