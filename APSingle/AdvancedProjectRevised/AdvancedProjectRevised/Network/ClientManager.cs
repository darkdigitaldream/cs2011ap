using System;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using AP;

/// <summary>
/// The network library for the client
/// Contributors: Kyle Galvin, Gage Patterson, Scott Herman
/// Revision: 299
/// </summary>
	public class ClientManager : NetManager
	{
		#region Fields (1) 

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientManager"/> class.
        /// As a client, it is our task to update our game state as the server dictates
        /// as well as to request our player's actions be performed on the server.
        /// In this manner, the host becomes the central game authority.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="State">The state.</param>
        /// <param name="serv">The server.</param>
        public ClientManager(int port, ref GameState State,Server serv): base(port, ref State)
		{
            //we have just recieved the signal to connect to a new game, along with the relevent host IP
            //We must connect to the given IP before the game starts

			//set up variables
            IsLobby = false;
            client = new TcpClient();

            IPEndPoint serverEndPoint = new IPEndPoint(serv.ServerIP, port);

            Console.WriteLine("Port: {0} IP: {1}",port,serv.ServerIP);
			Console.WriteLine("Waiting for connections...");
			
            client.Connect(serverEndPoint);//wait until we are connected to the server. 
			
            //we make a point of saving the server connection for future transmissions
			lock(this){
				myConnections.Add(new Connection(client));
				Console.WriteLine("Connected to {0}",client.Client.RemoteEndPoint);
			}
			
			NetworkStream clientStream = client.GetStream();
			
            //spawn a thread to listen to the server's signals/communication.
			Console.WriteLine("Creating listener thread for reading server communications...");
            Thread clientThread = new Thread(new ParameterizedThreadStart(HandleIncomingComm));
            clientThread.Start(myConnections[(myConnections.Count - 1)]);
            JoinGame(serv.Name);
		}

		#endregion Constructors 

		#region Methods (6) 

        //public override void SyncState(GameState s)
       // {
            /*case enemyModification:
            {
	            var foundEnemy = 	from e in <enemyList>
						            select e
						            where e.id = $
						            select e;
						
	            using(foundEnemy)
	            {
		            e.xPos = <packetInfo>;
		            e.yPos = <packetInfo>;
		            e.xVel = <packetInfo>;
		            e.yVel = <packetInfo>;
		            e.life = <packetInfo>;
	            }
	            break;
            }
            case playerModification:
            {
	            var foundPlayer = 	from p in <playerList>
						            select p
						            where p.id = #
						            select p;
						
	            using(foundPlayer)
	            {
		            p.xPos = <packetInfo>;
		            p.yPos = <packetInfo>;
		            p.xVel = <packetInfo>;
		            p.yVel = <packetInfo>;
		            p.life = <packetInfo>;
	            }
	            break;
            }
            case bulletModification:
            {
	            var foundBullet = 	from b in <List>
						            select b
						            where b.id = #
						            select b;
						
	            using(foundBullet)
	            {
		            b.xPos = <packetInfo>;
		            b.yPos = <packetInfo>;
	            }
	            break;
            }
            case deleteEnemy:
            {
	            var foundEnemy = 	from e in <enemyList>
						            select e
						            where e.id = $
						            select e;
						
	            using(foundEnemy)
	            {
		            enemyList.remove(e);
	            }
	            break;
            }
            case deletePlayer:
            {
	            var foundPlayer = 	from p in <playerList>
						            select p
						            where p.id = $
						            select p;
						
	            using(foundPlayer)
	            {
		            playerList.remove(p);
	            }
	            break;
            }
            case deleteBullet:
            {
	            var foundBullet = 	from e in <enemyList>
						            select e
						            where e.id = $
						            select e;
						
	            using(foundEnemy)
	            {
		            enemyList.remove(e);
	            }
	            break;
            }*/

            //case createEnemy:
            //{
            //    <enemyList>.add(new Enemy(/*info*/));
            //    break;
            //}
            //case createPlayer:
            //{
            //    <playerList>.add(new Player(/*info*/));
            //    break;
            //}
            //case createBullet:
            //{
            //    <bulletList>.add(new Bullet(/*info*/));
            //}
       // }

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
                    //c.Write(data);
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
                   // c.Write(data);

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
                   // c.Write(data);
                }
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
                    //Console.WriteLine("attempt to read pack:");
                    pack = myConnection.ReadPackage();
                    //Console.WriteLine("Package recieved!");
                    //Console.WriteLine("incoming id " + BitConverter.ToInt32( pack.body[0],0 )+"incoming x:" + BitConverter.ToInt32( pack.body[1],0 )+ " incoming y:" + BitConverter.ToInt32( pack.body[2],0));
                    packetSwitcher(pack, myConnection);
                }
                catch(Exception e)
                {
                    //a socket error has occured
                    Console.WriteLine(e.ToString());
                    break;
                }

                //if (bytesRead == 0)//nothing was read from socket
                //{
                //	Console.WriteLine("Client {0} has disconnected.",client.Client.RemoteEndPoint);
                //	break;
                //}
                
            }

            lock (this)
            {
                myConnections.Remove(myConnection);
            }
			
		}

		#endregion Methods 
	}


