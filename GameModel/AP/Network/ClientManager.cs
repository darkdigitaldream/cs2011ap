using System;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using AP;
using AP.Network;

namespace NetLib
{
    /// <summary>
    /// The network library for the client
    /// </summary>
	public class ClientManager : PlayerManager
	{
		#region Fields (1) 

        //HACK todo
        int i = 0;

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientManager"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="State">The state.</param>
        /// <param name="serv">The server.</param>
        public ClientManager(int port, ref GameState State,Server serv): base(port, ref State)
		{
			//set up variables
            IsLobby = false;
            client = new TcpClient();

            IPEndPoint serverEndPoint = new IPEndPoint(serv.ServerIP, port);

            Console.WriteLine("Port: {0} IP: {1}",port,serv.ServerIP);
			Console.WriteLine("Waiting for connections...");
			client.Connect(serverEndPoint);
			
			lock(this){
				myConnections.Add(new Connection(client));
				Console.WriteLine("Connected to {0}",client.Client.RemoteEndPoint);
			}
			
			NetworkStream clientStream = client.GetStream();
			
			Console.WriteLine("Creating listener thread for reading server communications...");
            Thread clientThread = new Thread(new ParameterizedThreadStart(HandleIncomingComm));
            clientThread.Start(myConnections[(myConnections.Count - 1)]);
            JoinGame(serv.Name);
		}

		#endregion Constructors 

		#region Methods (6) 

		// Public Methods (5) 

        /// <summary>
        /// Becomes the host.
        /// </summary>
        /// <param name="GameName">Name of the game.</param>
        public void BecomeHost(String GameName)
        {
            List<byte[]> data = myProtocol.encodeComm(Action.Describe, Type.Building, GameName);
            foreach (Connection c in myConnections)
            {
                foreach (NetPackage p in myOutgoing)
                {
                    //worker.

                    //Console.WriteLine("Writing model to stream: {0}",BitConverter.ToString(data[0],0)  );
                    c.Write(data);
                }
            }
        }

        /// <summary>
        /// Joins the game.
        /// </summary>
        /// <param name="GameName">Name of the game.</param>
        public void JoinGame(String GameName)
        {
            List<byte[]> data = myProtocol.encodeComm(Action.Request, Type.Building, GameName);
            foreach (Connection c in myConnections)
            {
                    Console.WriteLine("Writing model to stream: {0}",BitConverter.ToString(data[0],0)  );
                    c.Write(data);

            }
        }

        /// <summary>
        /// Servers the handshake.
        /// </summary>
        /// <param name="Username">The username.</param>
        public void ServerHandshake(String Username)
        {
            List<byte[]> data = myProtocol.encodeComm(Action.Describe, Type.Player, Username);
            foreach (Connection c in myConnections)
            {
                foreach (NetPackage p in myOutgoing)
                {
                    //worker.

                    //Console.WriteLine("Writing model to stream: {0}",BitConverter.ToString(data[0],0)  );
                    c.Write(data);
                }
            }
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void StartGame()
        {
            List<byte[]> data = myProtocol.encodeComm(Action.Create, Type.Player, "");
            foreach (Connection c in myConnections)
            {
                foreach (NetPackage p in myOutgoing)
                {
                    //worker.

                    //Console.WriteLine("Writing model to stream: {0}",BitConverter.ToString(data[0],0)  );
                    c.Write(data);
                }
            }
        }

        /// <summary>
        /// Syncs the state.
        /// </summary>
        public override void SyncState()
        {
            if (i == 0)
            {
                SendObjs<AP.Player>(Action.Create, State.Players);
            }
            else
            {
                SendObjs<AP.Player>(Action.Update, State.Players);
            }
        }
		// Protected Methods (1) 

        /// <summary>
        /// Handles the incoming comm.
        /// </summary>
        /// <param name="conn">The conn.</param>
		protected override void HandleIncomingComm(object conn)
		{

            Connection myConnection = (Connection)conn;
            NetPackage pack = new NetPackage();

            client = myConnection.GetClient();
            Console.WriteLine("client {0} has connected.", client.Client.RemoteEndPoint);

            while (true)
            {
                Console.WriteLine("Size of game state: {0}", State.Enemies.Count + State.Players.Count);
                try
                {
                    //read package data
                    Console.WriteLine("attempt to read pack:");
                    pack = myConnection.ReadPackage();
                    Console.WriteLine("Package recieved!");
                }
                catch
                {
                    //a socket error has occured
                    break;
                }

                //if (bytesRead == 0)//nothing was read from socket
                //{
                //	Console.WriteLine("Client {0} has disconnected.",client.Client.RemoteEndPoint);
                //	break;
                //}
                packetSwitcher(pack);
            }

            lock (this)
            {
                myConnections.Remove(myConnection);
            }
			
		}

		#endregion Methods 
	}
}

