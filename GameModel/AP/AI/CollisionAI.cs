﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AP
{
    /// <summary>
    /// The class that is used for collision detectiong and enemy AI.
    /// It keeps a seperate version of the game state which it is able to understand
    /// and use to efficiently run it's checks.
    /// </summary>
    class CollisionAI
    {
		#region Fields (3) 

        bool[,] blockedByWall;
        //private float TileSize = 1;
        List<Enemy>[,] tileList;
        private int Tiles = 50;

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Constructor to set up the basic game state as the AI will need to look at it.
        /// </summary>
        /// <param name="xWalls">The levels x wall positions.</param>
        /// <param name="yWalls">The levels y wall positions.</param>
        /// <param name="xSize">The x size of the walls.</param>
        /// <param name="ySize">The y size of the walls..</param>
        public CollisionAI(ref List<int> xWalls, ref List<int> yWalls, ref List<int> xSize, ref List<int> ySize)
        {
            tileList = new List<Enemy>[Tiles, Tiles];
            blockedByWall = new bool[Tiles, Tiles];
            int wallCount = 0;
            for (int i = 0; i < Tiles; i++)
                for (int j = 0; j < Tiles; j++)
                {
                    tileList[i, j] = new List<Enemy>();
                    bool blocked = false;
                    for (int w = 0; w < xWalls.Count; w++)
                    {
                        int countX = xSize[w];
                        int countY = ySize[w];
                        for (int x = countX - 1; x >= 0; x--)
                            for (int y = countY - 1; y >= 0; y--)
                            {
                                if (xWalls[w] + x == i - Tiles / 2 && yWalls[w] + y == j - Tiles / 2 && !blocked)
                                {
                                    blocked = true;
                                    blockedByWall[i, j] = true;
                                    wallCount++;
                                    //Console.WriteLine("{0} {1}", i, j);
                                }
                            }
                    }
                    if (!blocked)
                        blockedByWall[i, j] = false;
                }
            Console.WriteLine("wall count{0}", wallCount);
        }

		#endregion Constructors 

		#region Methods (4) 

		// Public Methods (4) 

        /// <summary>
        /// Checks to see if the source has collided with a zombie.
        /// </summary>
        /// <param name="source">The object that is being checked for collisions.</param>
        /// <param name="moveAwayFromX">The x position of a found object.</param>
        /// <param name="moveAwayFromY">The y positions of a found object.</param>
        /// <param name="enemyHit">The enemy that was hit.</param>
        /// <returns></returns>
        /// <output>Returns true if collided with a zombie. It also returns a ref to the enemy and it's centers positions as outs</output>
        public bool checkForCollision(Position source, out float moveAwayFromX, out float moveAwayFromY, out Enemy enemyHit)
        {
            int i = -1;
            int j = -1;
            for (int l = 0; l < Tiles; l++)
            {
                if (source.xPos <= l - Tiles / 2 && i == -1)
                {
                    i = l;
                }
                if (source.yPos <= l - Tiles / 2 && j == -1)
                {
                    j = l;
                }
            }
            if (i != -1 && j != -1)
            {
                if (i < Tiles - 1 && j < Tiles - 1)
                    foreach (Enemy b in tileList[i + 1, j + 1])
                    {
                        float diffX = source.xPos - b.xPos;
                        float diffY = source.yPos - b.yPos;
                        if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= source.radius + b.radius)
                        {
                            enemyHit = b;
                            moveAwayFromX = b.xPos;
                            moveAwayFromY = b.yPos;
                            return true;
                        }
                    }
                if (i < Tiles - 1)
                    foreach (Enemy b in tileList[i + 1, j])
                    {
                        float diffX = source.xPos - b.xPos;
                        float diffY = source.yPos - b.yPos;
                        if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= source.radius + b.radius)
                        {
                            enemyHit = b;
                            moveAwayFromX = b.xPos;
                            moveAwayFromY = b.yPos;
                            return true;
                        }
                    }
                if (i < Tiles - 1 && j > 0)
                    foreach (Enemy b in tileList[i + 1, j - 1])
                    {
                        float diffX = source.xPos - b.xPos;
                        float diffY = source.yPos - b.yPos;
                        if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= source.radius + b.radius)
                        {
                            enemyHit = b;
                            moveAwayFromX = b.xPos;
                            moveAwayFromY = b.yPos;
                            return true;
                        }
                    }
                if (j < Tiles - 1)
                    foreach (Enemy b in tileList[i, j + 1])
                    {
                        float diffX = source.xPos - b.xPos;
                        float diffY = source.yPos - b.yPos;
                        if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= source.radius + b.radius)
                        {
                            enemyHit = b;
                            moveAwayFromX = b.xPos;
                            moveAwayFromY = b.yPos;
                            return true;
                        }
                    }
                foreach (Enemy b in tileList[i, j])
                {
                    float diffX = source.xPos - b.xPos;
                    float diffY = source.yPos - b.yPos;
                    if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= source.radius + b.radius && source != b)
                    {
                        enemyHit = b;
                        moveAwayFromX = b.xPos;
                        moveAwayFromY = b.yPos;
                        return true;
                    }
                }
                if (j > 0)
                    foreach (Enemy b in tileList[i, j - 1])
                    {
                        float diffX = source.xPos - b.xPos;
                        float diffY = source.yPos - b.yPos;
                        if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= source.radius + b.radius)
                        {
                            enemyHit = b;
                            moveAwayFromX = b.xPos;
                            moveAwayFromY = b.yPos;
                            return true;
                        }
                    }
                if (i > 0 && j < Tiles - 1)
                    foreach (Enemy b in tileList[i - 1, j + 1])
                    {
                        float diffX = source.xPos - b.xPos;
                        float diffY = source.yPos - b.yPos;
                        if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= source.radius + b.radius)
                        {
                            enemyHit = b;
                            moveAwayFromX = b.xPos;
                            moveAwayFromY = b.yPos;
                            return true;
                        }
                    }
                if (i > 0)
                    foreach (Enemy b in tileList[i - 1, j])
                    {
                        float diffX = source.xPos - b.xPos;
                        float diffY = source.yPos - b.yPos;
                        if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= source.radius + b.radius)
                        {
                            enemyHit = b;
                            moveAwayFromX = b.xPos;
                            moveAwayFromY = b.yPos;
                            return true;
                        }
                    }
                if (i > 0 && j > 0)
                    foreach (Enemy b in tileList[i - 1, j - 1])
                    {
                        float diffX = source.xPos - b.xPos;
                        float diffY = source.yPos - b.yPos;
                        if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= source.radius + b.radius)
                        {
                            enemyHit = b;
                            moveAwayFromX = b.xPos;
                            moveAwayFromY = b.yPos;
                            return true;
                        }
                    }
            }
            //these need to be assigned but arnt ever used
            enemyHit = null;
            moveAwayFromX = 0;
            moveAwayFromY = 0;
            return false;                    
        }

        /// <summary>
        /// Checks for movement collision.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="moveAwayFromX">The move away from X.</param>
        /// <param name="moveAwayFromY">The move away from Y.</param>
        /// <returns></returns>
        public bool checkForMovementCollision(Position source, out float moveAwayFromX, out float moveAwayFromY)
        {
            Enemy enemyHit;
            //disabling the buggy wall check for now
            //if (checkForWallCollision(source, out moveAwayFromX, out moveAwayFromY))
                //return true;
            return checkForCollision(source, out moveAwayFromX, out moveAwayFromY, out enemyHit);
        }

        /// <summary>
        /// Checks for wall collision.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="moveAwayFromX">The move away from X.</param>
        /// <param name="moveAwayFromY">The move away from Y.</param>
        /// <returns></returns>
        public bool checkForWallCollision(Position source, out float moveAwayFromX, out float moveAwayFromY)
        {
            int i = -1;
            int j = -1;
            for (int l = 0; l < Tiles; l++)
            {
                if (source.xPos <= l - Tiles / 2 && i == -1)
                {
                    i = l;
                }
                if (source.yPos <= l - Tiles / 2 && j == -1)
                {
                    j = l;
                }
            }
            if (i != -1 && j != -1)
            {
                if (blockedByWall[i, j])
                {
                    moveAwayFromX = i - Tiles / 2;
                    moveAwayFromY = j - Tiles / 2;
                        int integerDropY = (int)source.yPos;
                        int integerDropX = (int)source.xPos;
                            if (source.yPos - integerDropY > -0.5f && source.yPos - integerDropY < 0.5f)
                            {
                                moveAwayFromX = source.xPos;
                                moveAwayFromY = source.yPos - 50;
                            }
                            if (source.xPos - integerDropX > -0.5f && source.xPos - integerDropX < 0.5f)
                            {
                                moveAwayFromX = source.xPos - 50;
                                moveAwayFromY = source.yPos;
                            }
                       
                        //Console.WriteLine("{0} {1} {2} {3}", source.xPos, source.yPos, source.xPos - integerDropX, source.yPos - integerDropY);
                        return true;
                }
                //Console.WriteLine("{0}", i);
                /*
                if (blockedByWall[i, j])
                {
                    moveAwayFromX = i + 0.5f;
                    moveAwayFromY = j + 0.5f;
                    return true;
                }*/
            }
            moveAwayFromX = 0;
            moveAwayFromY = 0;
            return false;
        }

        /// <summary>
        /// Sets up the enemy tile lists to be relevant.
        /// Called every game update.
        /// </summary>
        /// <param name="enemyList">The current enemyList</param>
        /// <output>None.</output>
        public void updateState(ref List<Enemy> enemyList)
        {
            //clear the tiles from the last update
            for (int i = 0; i < Tiles - 1; i++)
                for (int j = 0; j < Tiles - 1; j++)
                    tileList[i, j].Clear();

            foreach (var member in enemyList)
            {
                int x = -1, y = -1;
                for (int i = 0; i < Tiles; i++)
                {
                    if (member.xPos <= i - Tiles / 2 && x == -1)
                    {
                        x = i;
                    }
                    if (member.yPos <= i - Tiles / 2 && y == -1)
                    {
                        y = i;
                    }
                }
                if (x != -1 && y != -1)
                {
                    tileList[x, y].Add(member);
                    //Console.WriteLine("Enemy x: " + member.xPos + " added to x tile: " + x);
                }
                else
                    Console.WriteLine("Failed to add an enemy to the tile list");
            }
        }

		#endregion Methods 
    }
}
