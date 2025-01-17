﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using System.Net;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;

namespace AP
{
    /// <summary>
    /// Client side program. Used for both multiplayer and singleplayer.
    /// Contributors: Scott Herman, Gage Patterson, Kyle Galvin, Adam Humeniuk, Todd Burton.
    /// Revision: 298
    /// </summary>
    public class ClientProgram : GameWindow
    {
		#region Fields (58) 

        public static CollisionAI collisionAI;
        public EffectsHandler effectsHandler = new EffectsHandler();
        private bool enemySpawned = false;
        private GameState gameState;
        List<int> heightSquares = new List<int>();
        public static int imageBenson;
        public static int imageGameOver;
        ImageHandler imageHandler;
        private int imageLifeBar;
        private int imageLifeBarBG;
        private int imagePistolAvailable;
        private int imagePistolSelected;
        private int imageRifleAvailable;
        private int imageRifleSelected;
        private int imageRifleUnavailable;
        private int imageShotgunAvailable;
        private int imageShotgunSelected;
        private int imageShotgunUnavailable;
        private int imageYouWin;
        CreateLevel level;
        private bool levelComplete = false;
        public static int loadedBloodTexture;
        public static int loadedObjectBullet;
        public static int loadedObjectCrate;
        public static int loadedObjectGrass;
        public static int loadedObjectPlayer;
        public static LoadedObjects loadedObjects = new LoadedObjects();
        public static int loadedObjectWall;
        public static int loadedObjectZombie;
        public bool lose;
        private PathFinder mPathFinder;
        public static bool multiplayer = false;
        public static NetManager net;
        private Player player;
        List<int> playerSpawnID = new List<int>();
        private Random rand = new Random();
        // Screen dimensions
        private const int screenX = 700;
        private const int screenY = 700;
        public static SoundHandler soundHandler;
        List<EnemySpawn> spawns = new List<EnemySpawn>();
        TextHandler textHandler;
        public Tiles tiles;
        //camera related things
        Vector3d up = new Vector3d(0.0, 1.0, 0.0);
        Vector3d viewDirection = new Vector3d(0.0, 0.0, 1.0);
        private double viewDist = 23.0;
        private List<Wall> walls = new List<Wall>();
        List<int> widthSquares = new List<int>();
        public bool win;
        List<int> xPosPlayerSpawn = new List<int>();
        List<int> xPosSpawn = new List<int>();
        List<int> xPosSquares = new List<int>();
        List<int> yPosPlayerSpawn = new List<int>();
        List<int> yPosSpawn = new List<int>();
        List<int> yPosSquares = new List<int>();
        private int zombieCount = 0;
        private int ZombieCountTotal = 20;
        private int zombieIterator = 0;
        private int zombieKillCount = 0;

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Creates a window with the specified title.
        /// </summary>
        /// <param name="multi">if set to <c>true</c> [multi].</param>
        public ClientProgram(bool multi)
            : base(screenX, screenY, OpenTK.Graphics.GraphicsMode.Default, "ROFLPEWPEW")
        {
            multiplayer = multi;

            lose = false;
            win = false;

            VSync = VSyncMode.On;
        }

		#endregion Constructors 

		#region Methods (20) 

		// Public Methods (3) 

        /// <summary>
        /// Moves the player.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void movePlayer(int x, int y)
        {
            if (!multiplayer)
            {
                player.move(x, y);
            }
            else
            {
                try
                {
                    net.SendObjs<int>(Action.Request, new List<int>() { x, y }, Type.Move, net.myConnections[0]);

                }
                catch (Exception)
                {
                    MessageBox.Show("Error: Disconnected from server");
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// Starts the network.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public NetManager StartNetwork(ref GameState s)
        {
            //create client and/or server
            NetManager manager;

            Server serv = new Server("Serv", IPAddress.Parse("192.168.105.176"));
            manager = new ClientManager(9999, ref s, serv);
            manager.setRole("client");
            while (manager.myConnections.Count == 0) { }
            manager.Connected = true;
            return manager;
        }

        /// <summary>
        /// Toggles the sound.
        /// </summary>
        public void toggleSound()
        {
            if (soundHandler.getSoundState())
            {
                soundHandler.setSoundState(false);
                soundHandler.stopSong();
            }
            else
            {
                soundHandler.setSoundState(true);
                soundHandler.playSong(SoundHandler.BACKGROUND);
                soundHandler.continueSong();
            }
        }
		// Protected Methods (4) 

        /// <summary>
        /// Load resources here.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            // Create 
            gameState = new GameState();
            level = new CreateLevel(1);
            level.parseFile(ref xPosSquares, ref yPosSquares, ref heightSquares, ref widthSquares, ref xPosSpawn, ref yPosSpawn, ref xPosPlayerSpawn, ref yPosPlayerSpawn, ref playerSpawnID);

            soundHandler = new SoundHandler();
            textHandler = new TextHandler("../../Images/mybitmapfont.png");
            imageHandler = new ImageHandler();
            effectsHandler = new EffectsHandler();

            soundHandler.playSong(SoundHandler.BACKGROUND);

            imageLifeBarBG = imageHandler.loadImage("../../Images/LifeBarBg.png");
            imageLifeBar = imageHandler.loadImage("../../Images/LifeBar.png");

            imagePistolSelected = imageHandler.loadImage("Objects//Guns//Pistol_selected.png");
            imagePistolAvailable = imageHandler.loadImage("Objects//Guns//Pistol_available.png");
            imageRifleSelected = imageHandler.loadImage("Objects//Guns//Rifle_selected.png");
            imageRifleAvailable = imageHandler.loadImage("Objects//Guns//Rifle_available.png");
            imageRifleUnavailable = imageHandler.loadImage("Objects//Guns//Rifle_unavailable.png");
            imageShotgunSelected = imageHandler.loadImage("Objects//Guns//Shotgun_selected.png");
            imageShotgunAvailable = imageHandler.loadImage("Objects//Guns//Shotgun_available.png");
            imageShotgunUnavailable = imageHandler.loadImage("Objects//Guns//Shotgun_unavailable.png");

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.IndexArray);

            //Load Mesh Data into a buffer to be referenced in the future.
            loadedObjectWall = loadedObjects.LoadObject("Objects//UnitCube.obj", "Objects//cube.png", 1.0f);
            loadedObjectGrass = loadedObjects.LoadObject("Objects//groundTile.obj", "Objects//dry_grass.png", 5);
            loadedObjectBullet = loadedObjects.LoadObject("Objects//bullet.obj", "Objects//bullet.png", 0.04f);
            loadedObjectCrate = loadedObjects.LoadObject("Objects//guns//crate.obj", "Objects//guns//rifleCrate.png", 0.5f);
            loadedObjects.LoadObject("Objects//guns//crate.obj", "Objects//guns//shotgunCrate.png", 0.5f);
            imageGameOver = imageHandler.loadImage("Objects//GameOver.png");
            imageYouWin = imageHandler.loadImage("Objects//YouWin.png");
            imageBenson = imageHandler.loadImage("Objects//BensonHead.png");

            loadedObjectPlayer = loadedObjects.LoadObject("Objects//PlayerBody.obj", "Objects//Player.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerLeftLeg.obj", "Objects//Player.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerRightLeg.obj", "Objects//Player.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerLeftArm.obj", "Objects//Player.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerRightArm.obj", "Objects//Player.png", 0.08f);

            loadedObjectZombie = loadedObjects.LoadObject("Objects//PlayerBody.obj", "Objects//zombie.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerLeftLeg.obj", "Objects//zombie.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerRightLeg.obj", "Objects//zombie.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerLeftArm.obj", "Objects//zombie.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerRightArm.obj", "Objects//zombie.png", 0.08f);

            loadedBloodTexture = loadedObjects.LoadObject("Objects//square.obj", "Objects//BloodSplatters//Blood1.png", 0.4f);
            loadedObjects.LoadObject("Objects//square.obj", "Objects//BloodSplatters//Blood2.png", 0.4f);
            loadedObjects.LoadObject("Objects//square.obj", "Objects//BloodSplatters//Blood3.png", 0.4f);
            loadedObjects.LoadObject("Objects//square.obj", "Objects//BloodSplatters//Blood4.png", 0.4f);



            if (multiplayer)
            {
                net = StartNetwork(ref gameState);
                while (!net.Connected) { }
                Console.WriteLine("Connected!");
                //add players to collisionAI here
                setUpLevel();
            }
            else
            {
                player = new Player();
                player.assignPlayerID(0);

                player.xPos = xPosPlayerSpawn[0];
                player.yPos = yPosPlayerSpawn[0];

                gameState.Players.Add(player);
                setUpLevel();
                collisionAI.addToPlayerList(ref player);
                for (int i = 0; i < xPosSquares.Count; i++)
                {
                    walls.Add(new Wall(xPosSquares[i], yPosSquares[i], heightSquares[i], widthSquares[i]));
                }
                tiles = new Tiles(walls);
                mPathFinder = new PathFinder(tiles.byteList());
                collisionAI.wallTiles = tiles;
                foreach (var x in gameState.Players)
                {
                    x.tiles = tiles;
                }
            }
        }

        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Color3(1.0f, 1.0f, 1.0f); //reset colors
            if (gameState.Players.Count > 0)
            {
                base.OnRenderFrame(e);
                int i = 0;
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                Matrix4d camera = Matrix4d.LookAt(OpenTK.Vector3d.Multiply(viewDirection, viewDist), OpenTK.Vector3d.Zero, up);
                GL.LoadMatrix(ref camera);
                DrawMyPlayer();
                DrawObjects();
                if (enemySpawned)
                {
                    zombieIterator = 0;
                    enemySpawned = false;
                }
                DrawBackground();
                DrawWalls();
                effectsHandler.drawEffects();
                DrawMyGUI();

                //ADAM
                //move this into the for loop as an else     
                DrawOtherGUI();

                SwapBuffers();
            }
        }

        /// <summary>
        /// Called when your window is resized. Set your viewport here. It is also
        /// a good place to set up your projection matrix (which probably changes
        /// along when the aspect ratio of your window).
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnResize(EventArgs e)
        {
            //base.OnResize(e);
            GL.Viewport(0, 0, screenX, screenY);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        /// <summary>
        /// Called when it is time to setup the next frame. Add you game logic here.
        /// </summary>
        /// <param name="e">Contains timing information for framerate independent logic.</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            win = true;
            if (multiplayer && gameState.Players.Count > 0)
            {
                if (gameState.Players.Where(y => y.playerId == gameState.myUID).First().health <= 0)
                {
                    lose = true;
                }
                else
                {

                    for (int i = 0; i < gameState.Players.Count(); i++)
                    {
                        if (gameState.Players[i].playerId != gameState.myUID && gameState.Players[i].health > 0 && gameState.Players.Count > 1)
                        {
                            win = false;
                        }
                    }
                }
            }

            if (!multiplayer)
            {

                HandleCrates();
                HandleSpawning();
                HandleBullets();
                //ADAM
                /*
                 * MOVE THIS TO SERVER STUFFFFFFFFFFF
                 * 
                 */
                HandleSounds();
                collisionAI.updateState(ref gameState.Enemies);
                HandlePathing();
            }
            else
            {
                for (int i = 0; i < gameState.Bullets.Count; i++)
                {
                    gameState.Bullets[i].multiplayermove();
                }
                // player.walking = false;
                if (gameState.Players.Count > 0)
                {
                    player = gameState.Players.Where(y => y.playerId == gameState.myUID).First();
                    player.weapons.updateBulletCooldown();
                }

            }


            effectsHandler.updateEffects();

            if (!lose)
            {
                // Move your player
                if (Keyboard[Key.W] && Keyboard[Key.D])
                    movePlayer(1, 1);
                else if (Keyboard[Key.W] && Keyboard[Key.A])
                    movePlayer(-1, 1);
                else if (Keyboard[Key.S] && Keyboard[Key.D])
                    movePlayer(1, -1);
                else if (Keyboard[Key.S] && Keyboard[Key.A])
                    movePlayer(-1, -1);
                else if (Keyboard[Key.W])
                    movePlayer(0, 1);
                else if (Keyboard[Key.S])
                    movePlayer(0, -1);
                else if (Keyboard[Key.A])
                    movePlayer(-1, 0);
                else if (Keyboard[Key.D])
                    movePlayer(1, 0);
            }
            if (Keyboard[Key.Escape])
               Exit();


            // Toggle the sound on/off
            if (Keyboard[Key.F1] && !soundHandler.pressingF1)
            {
                toggleSound();
                soundHandler.pressingF1 = true;
            }
            else
                soundHandler.pressingF1 = false;

            // Change the view distance to be farther or closer to the point of reference.
            if (Keyboard[OpenTK.Input.Key.Up])
                viewDist *= 1.1f;
            else if (Keyboard[OpenTK.Input.Key.Down])
                viewDist *= 0.9f;

            // Equip a weapon
            if (Keyboard[Key.Number1])
                player.weapons.equipPistol();
            if (Keyboard[Key.Number2])
                player.weapons.equipRifle();
            if (Keyboard[Key.Number3])
                player.weapons.equipShotgun();
            if (Keyboard[Key.Number4])
                player.weapons.equipRocket();

            float wheelD = Mouse.WheelDelta;
            viewDirection.Y += wheelD / 10;
            viewDirection.Z += wheelD / 10;

            if (Keyboard[Key.Left])
                viewDirection.Y += 0.1f;
            if (Keyboard[Key.Right])
                viewDirection.Y -= 0.1f;


            //ADAM
            //move to serverrrrr
            //logic for picking up ammo crates


            if (Mouse[OpenTK.Input.MouseButton.Left] == true)
            {
                if (multiplayer && gameState.Players.Count > 0)
                {

                    if (player.weapons.canShoot())
                    {
                        if (player.weapons.shotgunEquipped)
                        {
                            soundHandler.play(SoundHandler.EXPLOSION);
                            Vector3 defaultVelocity = new Vector3(0, 0, 0);

                            float mx = (float)(Mouse.X - screenX / 2) / (screenX * 0.3f);
                            float my = (float)(Mouse.Y - screenY / 2) / (screenY * 0.3f);

                            float xVelo = -mx;
                            float yVelo = -my;

                            float len = (float)Math.Sqrt(xVelo * xVelo + yVelo * yVelo);
                            xVelo /= len;
                            yVelo /= len;

                            xVelo /= 10;
                            yVelo /= -10;

                            int spread = 100; //higher spread value makes the spread cover less area
                            Vector2 spreadTarget = new Vector2(player.xPos + xVelo * spread, player.yPos + yVelo * spread);

                            Bullet b1 = new Bullet(new Vector3(player.xPos, player.yPos, 0), new Vector2(Mouse.X*800/screenX, Mouse.Y*800/screenY), gameState.myUID);
                            b1.setDirectionByMouse(new Vector2(Mouse.X, Mouse.Y),new Vector2(screenX, screenY));
                            Bullet b2 = new Bullet(new Vector3(player.xPos, player.yPos, 0), new Vector2(Mouse.X * 800 / screenX, Mouse.Y * 800 / screenY), gameState.myUID);
                            b2.setDirectionToPosition(spreadTarget.X + 1, spreadTarget.Y + 1);
                            Bullet b3 = new Bullet(new Vector3(player.xPos, player.yPos, 0), new Vector2(Mouse.X * 800 / screenX, Mouse.Y * 800 / screenY), gameState.myUID);
                            b3.setDirectionToPosition(spreadTarget.X - 1, spreadTarget.Y + 1);
                            Bullet b4 = new Bullet(new Vector3(player.xPos, player.yPos, 0), new Vector2(Mouse.X * 800 / screenX, Mouse.Y * 800 / screenY), gameState.myUID);
                            b4.setDirectionToPosition(spreadTarget.X - 1, spreadTarget.Y - 1);
                            Bullet b5 = new Bullet(new Vector3(player.xPos, player.yPos, 0), new Vector2(Mouse.X * 800 / screenX, Mouse.Y * 800 / screenY), gameState.myUID);
                            b5.setDirectionToPosition(spreadTarget.X + 1, spreadTarget.Y - 1);

                            net.SendObjs<Bullet>(Action.Request, new List<Bullet>() { b1 }, Type.Bullet);
                            net.SendObjs<Bullet>(Action.Request, new List<Bullet>() { b2 }, Type.Bullet); 
                            net.SendObjs<Bullet>(Action.Request, new List<Bullet>() { b3 }, Type.Bullet);
                            net.SendObjs<Bullet>(Action.Request, new List<Bullet>() { b4 }, Type.Bullet);
                            net.SendObjs<Bullet>(Action.Request, new List<Bullet>() { b5 }, Type.Bullet);

                            gameState.Players.Where(y => y.playerId == gameState.myUID).First().weapons.shoot();
                        }
                        else
                        {
                            soundHandler.play(SoundHandler.EXPLOSION);
                            net.SendObjs<Bullet>(Action.Request,
                                                 new List<Bullet>()
                                                     {
                                                         new Bullet(new Vector3(player.xPos, player.yPos, 0),
                                                                    new Vector2(Mouse.X*800/screenX, Mouse.Y*800/screenY),
                                                                    gameState.myUID)
                                                     }, Type.Bullet);
                            gameState.Players.Where(y => y.playerId == gameState.myUID).First().weapons.shoot();
                        }
                    }
                }
                else
                {
                    if (player.weapons.canShoot())
                    {
                        player.weapons.shoot(ref gameState.Bullets, new Vector3(player.xPos, player.yPos, 0), new Vector2(screenX, screenY), new Vector2(Mouse.X, Mouse.Y), ref player);
                    }
                }

            }
            if (!multiplayer)
            {
                player.weapons.updateBulletCooldown();
                if (player.weapons.rifleBurstCooldown != 0)
                {
                    if (player.weapons.canShoot())
                    {
                        player.weapons.shoot(ref gameState.Bullets, new Vector3(player.xPos, player.yPos, 0), new Vector2(screenX, screenY), new Vector2(Mouse.X, Mouse.Y), ref player);
                    }
                }
            }
            if (!multiplayer)
            {
                if (zombieKillCount >= 2)
                {
                    levelComplete = true;
                    Console.WriteLine("HELLO");
                }


            }

            GC.Collect();


            

        }
		// Private Methods (13) 

        /// <summary>
        /// Draws the background.
        /// </summary>
        private void DrawBackground()
        {
            GL.Color3(1.0f, 1.0f, 1.0f);//resets the colors so the textures don't end up red
            //change this to be the same way as you do the walls
            for (int x = 0; x < 6; x++)
                for (int y = 0; y < 6; y++)
                {
                    GL.PushMatrix();
                    GL.Translate(-14 + x * 5.75, -14 + y * 5.75, 0);
                    loadedObjects.DrawObject(loadedObjectGrass); //grassssssssssssss
                    GL.PopMatrix();
                }
        }

        /// <summary>
        /// Draws my GUI.
        /// </summary>
        private void DrawMyGUI()
        {
            foreach (Player p in gameState.Players)
            {
                if (p.playerId == gameState.myUID)
                {
                    //life bar
                    TexUtil.InitTexturing();
                    GL.Color3(1.0f, 0.0f, 0.0f);
                    imageHandler.drawImage(imageLifeBar, 0.9f, 97.2f, 0.68f, 1.89f * p.health * 0.01f);
                    GL.Color3(1.0f, 1.0f, 1.0f);
                    imageHandler.drawImage(imageLifeBarBG, 0, 96, 0.68f, 1.0f);

                    if (lose)
                    {
                        imageHandler.drawImage(imageGameOver, 12.0f, 40.0f, 3.0f, 1.0f);
                        imageHandler.drawImageRotate(imageBenson, 50.0f, 30.0f, 2.0f, 1.0f, imageHandler.bensonRotate);
                        imageHandler.bensonRotate += 5;
                    }

                    if (win)
                    {
                        imageHandler.drawImage(imageYouWin, 12.0f, 40.0f, 3.0f, 1.0f);
                    }
                    //text stuff
                    GL.Color3(1.0f, 0.0f, 0.0f);
                    textHandler.writeText("Player " + (p.playerId + 1), 2, 55.0f, 98.1f, 0);
                    textHandler.writeText("Score: " + p.score, 2, 87.0f, 98.1f, 0);
                    p.score++;

                    //gun images
                    GL.Color3(1.0f, 1.0f, 1.0f);
                    if (p.weapons.pistolEquipped)
                        imageHandler.drawImage(imagePistolSelected, 0.7f, 89.0f, 1.0f, 1.0f);
                    else
                        imageHandler.drawImage(imagePistolAvailable, 0.7f, 89.0f, 1.0f, 1.0f);
                    if (p.weapons.rifleEquipped)
                        imageHandler.drawImage(imageRifleSelected, 8.0f, 89.0f, 1.0f, 1.0f);
                    else if (p.weapons.rifleAmmo <= 0)
                        imageHandler.drawImage(imageRifleUnavailable, 8.0f, 89.0f, 1.0f, 1.0f);
                    else
                        imageHandler.drawImage(imageRifleAvailable, 8.0f, 89.0f, 1.0f, 1.0f);
                    textHandler.writeText(p.weapons.rifleAmmo.ToString(), 2, 13.0f, 85.0f, 0);
                    if (p.weapons.shotgunEquipped)
                        imageHandler.drawImage(imageShotgunSelected, 21.5f, 89.0f, 1.0f, 1.0f);
                    else if (p.weapons.shotgunAmmo <= 0)
                        imageHandler.drawImage(imageShotgunUnavailable, 21.5f, 89.0f, 1.0f, 1.0f);
                    else
                        imageHandler.drawImage(imageShotgunAvailable, 21.5f, 89.0f, 1.0f, 1.0f);
                    textHandler.writeText(p.weapons.shotgunAmmo.ToString(), 2, 28.0f, 85.0f, 0);
                }
            }
        }

        /// <summary>
        /// Draws my player.
        /// </summary>
        private void DrawMyPlayer()
        {
            if (multiplayer)
            {
                Player player = gameState.Players.Where(y => y.playerId == gameState.myUID).First();

                player.draw();

                GL.Translate(-player.xPos, -player.yPos, 0);
                lock (gameState)
                {
                    foreach (Player p in gameState.Players)
                    {

                        if (p.playerId != player.playerId)
                        {
                            //Console.WriteLine("x:" + p.xPos + " y:" + p.yPos);
                            GL.PushMatrix();
                            GL.Translate(p.xPos, p.yPos, 0);
                            p.draw();
                            GL.PopMatrix();
                        }
                    }
                }
            }
            else
            {
                player.draw();
                GL.Translate(-player.xPos, -player.yPos, 0);
            }
        }

        /// <summary>
        /// Draws the objects.
        /// </summary>
        private void DrawObjects()
        {
            lock (gameState)
            {
                for (int index = 0; index < gameState.Bullets.Count; index++)
                {
                    Bullet bullet = gameState.Bullets[index];
                    GL.PushMatrix();
                    bullet.draw();
                    GL.PopMatrix();
                }

                for (int index = 0; index < gameState.Crates.Count; index++)
                {
                    Crate crate = gameState.Crates[index];
                    GL.PushMatrix();
                    crate.draw();
                    GL.PopMatrix();
                }

                GL.Color3(1.0f, 1.0f, 1.0f);
                for (int index = 0; index < gameState.Enemies.Count; index++)
                {
                    var enemy = gameState.Enemies[index];
                    enemy.draw();
                }
            }
        }

        /// <summary>
        /// Draws the other GUI.
        /// </summary>
        private void DrawOtherGUI()
        {
            var i = 0;
            int horizontalInc = 0;
            foreach (var x in gameState.Players.Where(x => x.playerId != gameState.myUID))
            {
                horizontalInc = i * 37;
                GL.Color3(0.0f, 0.0f, 1.0f);
                imageHandler.drawImage(imageLifeBar, 0.7f + horizontalInc, 0.84f, 0.5f, 1.89f * 0.01f * x.health);
                GL.Color3(1.0f, 1.0f, 1.0f);
                imageHandler.drawImage(imageLifeBarBG, 0 + horizontalInc, 0, 0.5f, 1.0f);
                GL.Color3(0.0f, 0.0f, 1.0f);
                textHandler.writeText("Player " + x.playerId, 2, 12.0f + horizontalInc, 6.0f, 0);
                i++;
            }

        }

        /// <summary>
        /// Draws the walls.
        /// </summary>
        private void DrawWalls()
        {
            int i;
            i = 0;
            foreach (var x in xPosSquares)
            {
                if (widthSquares[i] > 1)
                {
                    for (int idx = 0; idx < widthSquares[i]; idx++)
                    {
                        GL.PushMatrix();
                        GL.Translate(x + idx + 0.5f, yPosSquares[i] - 0.5f, 0.5f);
                        loadedObjects.DrawObject(loadedObjectWall);
                        GL.PopMatrix();
                    }
                }

                if (heightSquares[i] > 1)
                {
                    for (int idx = 0; idx < heightSquares[i]; idx++)
                    {
                        GL.PushMatrix();
                        GL.Translate(x + 0.5f, yPosSquares[i] + idx - 0.5f, 0.5f);
                        loadedObjects.DrawObject(loadedObjectWall);
                        GL.PopMatrix();
                    }
                }

                i++;
            }
        }

        /// <summary>
        /// Handles the bullets.
        /// </summary>
        private void HandleBullets()
        {
            List<Bullet> tmpBullet = new List<Bullet>();
            foreach (Bullet bullet in gameState.Bullets)
            {
                if (tiles.isWall(bullet.xPos, bullet.yPos))
                {
                    tmpBullet.Add(bullet);
                }
                bullet.move();
                if (bullet.killProjectile())
                    tmpBullet.Add(bullet);
                float moveX;
                float moveY;
                Enemy enemyHit;
                bool hit = collisionAI.checkForCollision(bullet, out moveX, out moveY, out enemyHit);
                if (hit)
                {
                    //ADAM
                    effectsHandler.addBlood(moveX, moveY); //needs to be on the server too
                    tmpBullet.Add(bullet);
                    if (enemyHit.decreaseHealth())
                    {
                        gameState.Enemies.Remove(enemyHit);
                        zombieKillCount++;
                        Console.WriteLine(zombieKillCount);
                    }
                    GC.Collect();
                    bullet.timestamp = -1;
                    soundHandler.play(SoundHandler.ZOMBIE);
                    if (rand.Next(0, 10) < 1) //new ammo crate
                        gameState.Crates.Add(new Crate(new Vector2(enemyHit.xPos, enemyHit.yPos), gameState.myUID));
                }
            }


            foreach (Bullet bullet in tmpBullet)
            {
                gameState.Bullets.Remove(bullet);
            }
        }

        /// <summary>
        /// Handles the crates.
        /// </summary>
        private void HandleCrates()
        {
            List<Crate> cratesToRemove = new List<Crate>();
            foreach (Crate crate in gameState.Crates)
            {
                float diffX = player.xPos - crate.xPos;
                float diffY = player.yPos - crate.yPos;
                if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= player.radius + crate.radius)
                {
                    if (crate.crateType == 0)
                        player.weapons.rifleAmmo += 25;
                    else if (crate.crateType == 1)
                        player.weapons.shotgunAmmo += 5;
                    cratesToRemove.Add(crate);
                    soundHandler.play(SoundHandler.RELOAD);
                }
            }
            foreach (Crate crate in cratesToRemove)
            {
                gameState.Crates.Remove(crate);
            }
        }

        /// <summary>
        /// Handles the pathing.
        /// </summary>
        private void HandlePathing()
        {
            for (int index = 0; index < gameState.Enemies.Count; index++)
            {
                var zombie = gameState.Enemies[index];
                //Find closest player
                var playerPos = tiles.returnTilePos(player);
                var enemyPos = tiles.returnTilePos(zombie);
                //Check to see how close the zombie is to the player
                float x1 = player.xPos - zombie.xPos;
                float y1 = player.yPos - zombie.yPos;
                float len1 = (float)Math.Sqrt(x1 * x1 + y1 * y1);
                if (len1 <= 1.1)
                {
                    zombie.moveTowards(player);
                }
                //Find a path
                else if (enemyPos != null)
                {
                    if (playerPos != null)
                    {
                        List<PathFinderNode> path = mPathFinder.FindPath(enemyPos[0], enemyPos[1], playerPos[0],
                                                                         playerPos[1]);
                        //Give next X Y Coords
                        if (path != null && path.Count > 1)
                        {
                            var nextMove = tiles.returnCoords(path[1].X, path[1].Y);
                            //Move towards them
                            //Calculates the len between the moves
                            float x = nextMove[0] - zombie.xPos;
                            float y = nextMove[1] - zombie.yPos;
                            float len = (float)Math.Sqrt(x * x + y * y);
                            if (len < 1 && path.Count > 2)
                            {
                                nextMove = tiles.returnCoords(path[2].X, path[2].Y);
                                zombie.moveTowards(nextMove[0], nextMove[1]);
                            }
                            else
                            {
                                zombie.moveTowards(nextMove[0], nextMove[1]);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the sounds.
        /// </summary>
        private void HandleSounds()
        {
            float moveX2, moveY2;
            if (collisionAI.checkForMovementCollision(player, out moveX2, out moveY2))
            {
                player.health--;
                if (soundHandler.injuredSoundCooldown <= 0)
                {
                    soundHandler.play(SoundHandler.SCREAM);
                    soundHandler.injuredSoundCooldown = 14;
                }
            }
            soundHandler.injuredSoundCooldown--;
        }

        /// <summary>
        /// Handles the spawning.
        /// </summary>
        private void HandleSpawning()
        {
            zombieIterator++;
            if (zombieCount < 100)
            {
                foreach (var spawn in spawns)
                {
                    spawn.draw();
                    if (zombieIterator == 40)
                    {
                        lock (gameState)
                        {
                            //need to ping server for a UID
                            gameState.Enemies.Add(spawn.spawnEnemy(0));
                            enemySpawned = true;
                            zombieCount++;
                        }

                    }
                }
                if (enemySpawned)
                {
                    zombieIterator = 0;
                    enemySpawned = false;
                }
            }
        }

        /// <summary>
        /// Sets the spawns.
        /// </summary>
        private void setSpawns()
        {
            spawns.Clear();
            for (int index = 0; index < xPosSpawn.Count; index++)
            {
                spawns.Add(new EnemySpawn(xPosSpawn[index], yPosSpawn[index]));
            }
        }

        /// <summary>
        /// Sets up level.
        /// </summary>
        private void setUpLevel()
        {
            collisionAI = new CollisionAI(ref xPosSquares, ref yPosSquares, ref widthSquares, ref heightSquares);
            setSpawns();
        }

		#endregion Methods 
    }
}
