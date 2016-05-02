using System;
using System.Net;
using System.Net.Sockets;
namespace AssemblyCSharp
{

	//The ClientInfo structure holds the required information about every
	//client connected to the server
	struct ClientInfo
	{
		public EndPoint endpoint;   //Socket of the client
		public int ID;      		//ID of the player
	}
}

