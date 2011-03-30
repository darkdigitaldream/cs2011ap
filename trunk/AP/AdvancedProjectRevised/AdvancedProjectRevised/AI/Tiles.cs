﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AP
{
    public class Tiles
    {
        public Tile[,] tileList;
        private float maxX;
        private float minX;
        private float maxY;
        private float minY;
        public Tiles(float _minX, float _maxX, float _minY, float _maxY)
        {
            minX = _minX;
            maxX = _maxX;
            minY = _minY;
            maxY = _maxY;
            //calculateTiles();
        }
        public Tiles(List<Wall> walls)
        {
            minX = walls.Min(y => y.xPos) - 1;
            maxX = walls.Max(y => y.xPos) + 1;
            minY = walls.Min(y => y.yPos) - 1;
            maxY = walls.Max(y => y.yPos) + 1;
            calculateTiles(walls);
        }
        private void calculateTiles(List<Wall> walls)
        {
            var width = Int32.Parse((maxX - minX).ToString());
            var height = Int32.Parse((maxY - minY).ToString());
            tileList = new Tile[width, height];
            //create the tiles with two for loops
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    //Sets the x and y for each tile
                    tileList[i, j] = new Tile(minX + i, minY + j, i + j);
                    if (walls.Where(y => (y.yPos <= tileList[i, j].Y) && ((y.yPos + y.height) > tileList[i, j].Y) && (y.xPos <= tileList[i, j].X) && ((y.xPos + y.width) > tileList[i, j].X)).Count() > 0)
                    {
                        tileList[i, j].isWall = true;
                    }
                    //if (walls.Where(y => (y.yPos == tileList[i, j].y) && (y.xPos == tileList[i, j].x) ).Count() > 0)
                    //{
                    //    tileList[i, j].isWall = true;
                    //}
                }
            }
            //Go through each loop and check to see if the space is occuppied by the walls or not.

        }
        public List<int> returnTilePos(Position p)
        {
            var returnList = new List<int>();
            for (int i0 = 0; i0 <= tileList.GetUpperBound(0); i0++)
                for (int i1 = 0; i1 <= tileList.GetUpperBound(1); i1++)
                {
                    var x = tileList[i0, i1];
                    if (p.xPos >= x.X && p.xPos < x.X + 1 && p.yPos >= x.Y - 1 && p.yPos < x.Y)
                    {
                        returnList.Add(i0);
                        returnList.Add(i1);
                        return returnList;
                    }
                }
            return null;
        }
        public List<float> returnCoords(int i, int j)
        {
            var returnList = new List<float>();
            returnList.Add(minX + i + 0.5f);
            returnList.Add(minY + j - 0.5f);
            return returnList;
        }

        public byte[,] byteList()
        {
            var returnList = new byte[tileList.GetUpperBound(0) + 1, tileList.GetUpperBound(1) + 1];
            for (int i0 = 0; i0 <= tileList.GetUpperBound(0); i0++)
                for (int i1 = 0; i1 <= tileList.GetUpperBound(1); i1++)
                {
                    if (tileList[i0, i1].isWall)
                    {
                        returnList[i0, i1] = 0;
                    }
                    else
                    {
                        returnList[i0, i1] = 1;
                    }
                }
            return returnList;
        }

        public bool isWall(float xPos, float yPos)
        {
            for (int i0 = 0; i0 <= tileList.GetUpperBound(0); i0++)
                for (int i1 = 0; i1 <= tileList.GetUpperBound(1); i1++)
                {
                    var x = tileList[i0, i1];
                    if (xPos >= x.X && xPos < x.X + 1 && yPos >= x.Y - 1 && yPos < x.Y)
                    {
                        if (tileList[i0, i1].isWall)
                        {
                            return true;
                        }
                    }
                }
            return false;
        }
    }
}
