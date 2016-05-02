using UnityEngine;
using System;
using System.Collections;
using AssemblyCSharp;

public class GameController : MonoBehaviour {

    private Logger mLogger = null;
    private FileLogger mFileLogger = null;

	private GameState state;
	public int MaxScore;
    private Vector3 countDown;
    private GameState flagState;
    private int[] scoreList;
    private int[] lastScoreList;
    private int timeToStart;
    private Players lastWinner;

	public int numOfPlayers;

    // text boards
    public GUIText GameInfoText;
    public GUIText ScoreBoardText;
    public GUIText winnerBoardText;

    public PuckController Puck;
    public ComputerController Player1;
    public HumanController Player2;
    public CommunicationController com;

    private int logCounter;
    private int logCycle;

	public GameState State
	{
		get { return state;}
	}

    public Players LastWinner
    {
        set { lastWinner = value; }
    }

	public int PlayersNum
	{
		get { return numOfPlayers;}
	}

	// Use this for initialization
	void Start ()
	{
        UpdateState(GameState.Disconnected);
        logCycle = 10;
        scoreList = new int[2];
        lastScoreList = new int[2];
        lastWinner = Players.All;

        if (mLogger == null)
        {
            mLogger = Logger.Instance;
            mFileLogger = new FileLogger(@"c:\temp\trajectories.txt");
            mFileLogger.Init();
            mLogger.RegisterObserver(mFileLogger);
            mLogger.AddLogMessage("***** New run of the server!! *****");
            mFileLogger.Terminate();
        }

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (flagState != GameState.None)
            UpdateState(flagState);

        //print(state.ToString());
        if (State == GameState.Idle)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                // start new game
                UpdateState(GameState.Starting);
                //StartCoroutine("GetReady");
            }
        }
        if (state == GameState.Starting)
        {
            timeToStart = (int)countDown.sqrMagnitude;
            //print ("starting, " + timeToStart.ToString());
            if (timeToStart > 0)
            {
                GameInfoText.text = timeToStart.ToString();
            }
            else
            {
                UpdateState(GameState.Playing);
                mFileLogger.Init();
                mLogger.AddLogMessage("Starting new chapter");
                mLogger.AddLogMessage("Puck \t\t Agent \t\t Human");
                mFileLogger.Terminate();
            }
        }

	}

    void FixedUpdate()
    {
        /*if (state == GameState.Playing)
        {
            if (logCounter < logCycle)
            {
                logCounter++;
            }
            else
            {
                mFileLogger.Init();
                mLogger.AddLogMessage(Puck.rigidbody2D.position.ToString() + "\t" + Player1.rigidbody2D.position.ToString() +
                                      "\t" + Player2.rigidbody2D.position.ToString());
                mFileLogger.Terminate();
                logCounter = 1;
            }
        }*/
    }

    public void UpdateState(GameState st)
    {
        GameState currentState = this.state;
        // send signals
        switch (st)
        {
            case GameState.Disconnected:
                GameInfoText.text = "Welcome!";
                ScoreBoardText.text = "";
                winnerBoardText.text = "";
                break;

            case GameState.Idle:
                UpdateScore(Players.All);
                if (lastWinner != Players.All)
                {
                    if (lastWinner == Players.Player1)
                        winnerBoardText.text = "Player 1 has won the game!";
                    else
                        winnerBoardText.text = "Player 2 has won the game!";
                    ScoreBoardText.text = lastScoreList[0].ToString() + ":" + lastScoreList[1].ToString();

                    com.SendSignal(Command.EndGame, null, Players.All);
                }
                GameInfoText.text = "Press 'R' To Start a New Game";
                countDown = new Vector3(1, 1, 1);
                break;

            case GameState.Starting:
                if (currentState == GameState.Idle)
                    com.SendSignal(Command.NewGame, MaxScore.ToString(), Players.All);
                else if (currentState == GameState.Playing)
                    com.SendSignal(Command.Pause, null, Players.All);

                winnerBoardText.text = "";
                ScoreBoardText.text = scoreList[0].ToString() + ":" + scoreList[1].ToString();
                ResetObjects();
                StartCoroutine("GetReady");
                break;

            case GameState.Playing:
                com.SendSignal(Command.Start, null, Players.All);
                GameInfoText.text = "";
                countDown = new Vector3(1, 1, 1);
                break;
        }
        flagState = GameState.None;
        state = st;
        print (state.ToString());
    }

    public void UpdateAsyncState(GameState st)
    {
        flagState = st;
    }

    IEnumerator GetReady()
    {
        int i;
        for (i=0; i<3; i++)
        {
            yield return new WaitForSeconds(1f);
            int norm = (int)countDown.sqrMagnitude;
            countDown [norm - 1] = 0;
            print(norm.ToString());
        }
    }

    public void UpdateScore(Players P)
    {
        switch (P)
        {
            case Players.Player1:
                scoreList [0] += 1;
                break;
            case Players.Player2:
                scoreList [1] += 1;
                break;
            case Players.All:
                for (int i=0; i<2; i++)
                    scoreList[i]=0;
                break;
        }

        if (P != Players.All)
        {
            if ((scoreList [0] == MaxScore) || (scoreList [1] == MaxScore))
            {
                lastScoreList [0] = scoreList [0];
                lastScoreList [1] = scoreList [1];
            }

            com.SendSignal(Command.Score, scoreList [0].ToString() + ":" + scoreList [1].ToString(), Players.Player1);
            com.SendSignal(Command.Score, scoreList [1].ToString() + ":" + scoreList [0].ToString(), Players.Player2);
        }
    }

    public Players IsWin()
    {
        if (scoreList[0] == MaxScore)
            return Players.Player1;
        else if (scoreList[1] == MaxScore)
            return Players.Player2;
        else
            return Players.All;
    }

    private void ResetObjects()
    {
        Puck.rigidbody2D.rotation = 0;
        Puck.rigidbody2D.angularVelocity = 0;
        Puck.rigidbody2D.position = new Vector2(0, 0);
        Puck.rigidbody2D.velocity = new Vector2(0, 0);

        Player1.rigidbody2D.position = new Vector2(-1000, 0);
        Player1.rigidbody2D.velocity = new Vector2(0, 0);

        Player2.rigidbody2D.position = new Vector2(1000, 0);
        Player2.rigidbody2D.velocity = new Vector2(0, 0);
    }
}
