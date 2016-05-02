using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using AssemblyCSharp;

public class CommunicationController : MonoBehaviour {

	public PuckController Puck;
	public HumanController Human;
	public ComputerController Computer;
	public GameController gameController;
    public double visionTimeStep;                   //new
    private int counter, stepsCounter;              //new

	//The collection of all clients logged into the game (an array of type ClientInfo)
	//ArrayList clientList;
    List<ClientInfo> clientList;
	private int numOfPlayers;

	//The main socket on which the server listens to the clients
	Socket serverSocket;
	byte[] byteData = new byte[1024];


	// Use this for initialization
	void Start ()
	{
        stepsCounter = (int)Math.Round(visionTimeStep / Time.fixedDeltaTime);       //new
        counter = 1;                                                                //new
		numOfPlayers = gameController.PlayersNum;
		//clientList = new ArrayList();
        clientList = new List<ClientInfo>();

		// recieve connection object for articifial player and send camera data.
		try
		{
			//We are using UDP sockets
			serverSocket = new Socket(AddressFamily.InterNetwork,
			                          SocketType.Dgram, ProtocolType.Udp);
			
			//Assign the any IP of the machine and listen on port number 1234
			IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 1234);
			
			//Bind this address to the server
			serverSocket.Bind(ipEndPoint);
			
			IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
			//The epSender identifies the incoming clients
			EndPoint epSender = (EndPoint)ipeSender;
			
			//Start receiving data
			serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length,
			                              SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);
		}
		catch (Exception ex)
		{
			print ("Start: " + ex.Message);
		}
	}
	
	// Update is called once per frame
    void FixedUpdate () {
		if (gameController.State == GameState.Playing)
        {
            if (counter == stepsCounter)
            {
                // agentX, agentY, agentVx, agentVy, puckX, puckY, puckVx, puckVy, puckR, oppX, oppY, oppVx, oppVy
                string puckData = Puck.rigidbody2D.position.x.ToString()+","+Puck.rigidbody2D.position.y.ToString()+","+
                    Puck.rigidbody2D.velocity.x.ToString()+","+Puck.rigidbody2D.velocity.y.ToString()+","+
                    Puck.rigidbody2D.angularVelocity.ToString();
                string player1Data = Computer.rigidbody2D.position.x.ToString()+","+Computer.rigidbody2D.position.y.ToString()+","+
                    Computer.rigidbody2D.velocity.x.ToString()+","+Computer.rigidbody2D.velocity.y.ToString();
                string player2Data = Human.rigidbody2D.position.x.ToString()+","+Human.rigidbody2D.position.y.ToString()+","+
                    Human.rigidbody2D.velocity.x.ToString()+","+Human.rigidbody2D.velocity.y.ToString();

                SendSignal(Command.Message, player1Data+","+puckData+","+player2Data, Players.Player1);
                if (gameController.PlayersNum == 2)
                    SendSignal(Command.Message, player2Data+","+puckData+","+player1Data, Players.Player2);

                counter = 1;
            }
            else
            {
                counter++;
            }
        }
	}

    public void SendSignal(Command cmd, string msg, Players P)
    {
        Data msgToSend = new Data();
        msgToSend.sID = 0;
        msgToSend.cmdCommand = cmd;
        msgToSend.strMessage = msg;

        print(cmd.ToString());

        byte[] message = msgToSend.ToByte ();
        try
        {
            switch (P)
            {
                case Players.All:
                    foreach (ClientInfo client in clientList)
                    {
                        serverSocket.BeginSendTo (message, 0, message.Length, SocketFlags.None, client.endpoint,
                                                  new AsyncCallback (OnSend), client.endpoint);
                    }
                    break;
                case Players.Player1:
                    ClientInfo client1 = clientList[0];
                    serverSocket.BeginSendTo (message, 0, message.Length, SocketFlags.None, client1.endpoint,
                                              new AsyncCallback (OnSend), client1.endpoint);
                    break;
                case Players.Player2:
                    if (clientList.Count > 1)
                    {
                        ClientInfo client2 = clientList[1];
                        serverSocket.BeginSendTo (message, 0, message.Length, SocketFlags.None, client2.endpoint,
                                                  new AsyncCallback (OnSend), client2.endpoint);
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            print("SendSigal: " + ex.Message);
        }
    }

	private void OnReceive(IAsyncResult ar)
	{
		try
		{
			IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
			EndPoint epSender = (EndPoint)ipeSender;
			serverSocket.EndReceiveFrom(ar, ref epSender);
			
			Data msgReceived = new Data(byteData);
            print("Received: " + msgReceived.cmdCommand.ToString());
			// message handling (switch)
			switch (msgReceived.cmdCommand)
			{
				case Command.Login:
					if (clientList.Count< numOfPlayers)
					{
						ClientInfo clientInfo = new ClientInfo();
						clientInfo.endpoint = epSender;
						if (clientList.Count == 1)
                        {
							clientInfo.ID = 1;			//Add the first player - always the left player
                            clientList.Add(clientInfo);
                            SendSignal(Command.Login,clientInfo.ID.ToString(),Players.Player1);
                        }
						else 
                        {
							clientInfo.ID = 2;			// Add the second player - always the right player
                            clientList.Add(clientInfo);
                            SendSignal(Command.Login,clientInfo.ID.ToString(),Players.Player2);
                        }
					}
					if (clientList.Count == numOfPlayers)
					{
                        gameController.UpdateAsyncState(GameState.Idle);
					}
					break;

				case Command.Logout:
					int nIndex = 0;
					foreach (ClientInfo client in clientList)
					{
						if (client.endpoint.Equals(epSender))
						{
							print ("removing player " + (nIndex+1).ToString());
							clientList.RemoveAt(nIndex);
							break;
						}
						++nIndex;
					}
                    gameController.UpdateAsyncState(GameState.Disconnected);
					break;

				case Command.Message:
                    string[] values = msgReceived.strMessage.Split(',');
                    /*computerXvelocity = float.Parse(values[0])*2;
                    computerYvelocity = float.Parse(values[1])*2;*/
                    if (msgReceived.sID == 1)
                    {
                        Computer.UpdateSpeed(float.Parse(values[0]), float.Parse(values[1]));
                    }
					break;

				default:
					break;
			}

			ipeSender = new IPEndPoint(IPAddress.Any, 0);
			epSender = (EndPoint)ipeSender;
			//Start listening to the messages send by the players
			serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, ref epSender,
			                              new AsyncCallback(OnReceive), epSender);

		}
		catch (Exception ex)
		{
			print("OnReceive Error: " + ex.Message);
		}
	}

	private void OnSend(IAsyncResult ar)
	{
		try
		{
			serverSocket.EndSend(ar);
		}
		catch (Exception ex)
		{
			print("OnSend Error: " + ex.Message);
		}
	}

}
