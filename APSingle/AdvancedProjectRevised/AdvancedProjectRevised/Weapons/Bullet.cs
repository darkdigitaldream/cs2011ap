﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace AP
{
     /// <summary>
     /// The basic bullet class. It will keep track of bullet lifetime as well as movement.
    /// Contributors: Scott Herman, Gage Patterson, Adam Humeniuk, Kyle Galvin
    /// Revision: 265
    /// </summary>
    public class Bullet: Position
    {
		#region Fields (6) 

        //private Bullet prevBullet = null;
        private float direction = 1.0f;
        private int lifeTime = 30;
        public Vector3 mousePos;
        public Vector2 velocity;
        float xVel = 0;
        float yVel = 0;

		#endregion Fields 

		#region Constructors (2) 

        /// <summary>
        /// This is the bullet constructor. It sets up the attributes of a basic bullet
        /// </summary>
        /// <param name="position">This is the point that the bullet starts at</param>
        /// <param name="mousePos">The mouse pos.</param>
        /// <param name="life">The life.</param>
        /// <output>
        ///   </output>
        public Bullet(Vector3 position, Vector3 mousePos, int life)
        {
            radius = 0.1f;
            this.position = position;
            xPos = position.X;
            yPos = position.Y;
            speed = 2.0f;
            this.mousePos = mousePos;
            timestamp = 0;
            lifeTime = life;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bullet"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="velocity">The velocity.</param>
        public Bullet(Vector3 position, Vector2 velocity)
        {
            radius = 0.1f;
            this.position = position;
            xPos = position.X;
            yPos = position.Y;
            speed = 2.0f;
            this.velocity = velocity;
            timestamp = 0;
        }

		#endregion Constructors 

		#region Methods (5) 

		// Public Methods (5) 

        /// <summary>
        /// Draws this instance.
        /// </summary>
        public void draw()
        {
            /*GL.Begin(BeginMode.Polygon);
            
            GL.Color3(0.0f, 0.0f, 0.0f);
            GL.Vertex3(xPos - radius, yPos, 0.1f);
            GL.Vertex3(xPos - radius * 0.7, yPos + radius * 0.7, 0.1f);
            GL.Vertex3(xPos, yPos + radius, 0.1f);
            GL.Vertex3(xPos + radius * 0.7, yPos + radius * 0.7, 0.1f);
            GL.Vertex3(xPos + radius, yPos, 0.1f);
            GL.Vertex3(xPos + radius * 0.7, yPos - radius * 0.7, 0.1f);
            GL.Vertex3(xPos, yPos - radius, 0.1f);
            GL.Vertex3(xPos - radius * 0.7, yPos - radius * 0.7, 0.1f); 
            GL.End();*/

            GL.PushMatrix();
            GL.Translate(xPos, yPos, 0.5f);
            GL.Rotate(angle - 90, 0, 0, 1);
            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectBullet); //body 
            GL.PopMatrix();

            //if (prevBullet != null)
                //prevBullet.draw();
        }

        /// <summary>
        /// Called from our main program in order to see if a bullets lifetime has expired
        /// </summary>
        /// <returns></returns>
        /// <output>
        /// The output will be true if the bullet has reached 0 lifetime, false otherwise.
        ///   </output>
        public bool killProjectile()
        {
            if (lifeTime <= 0)
                return true;
            return false;
        }

        /// <summary>
        /// Called on update to move the bullet.
        /// </summary>
        /// <output>
        /// No output, but the location of the bullet is changed. The lifetime of the bullet is also decremented.
        ///   </output>
        public void move()
        {            
            xPos += xVel * speed;
            yPos += yVel * speed;

            lifeTime--;
        }

        /// <summary>
        /// This method is called in order to set the velocity of the bullet based on where the mouse is on the screen.
        /// It will set the bullet on a path that will lead directly to the cursor's position.
        /// </summary>
        /// <param name="x">The x position of the cursor in pixels.</param>
        /// <param name="y">The y position of the cursor in pixels.</param>
        /// <param name="screenX">The width of the game window.</param>
        /// <param name="screenY">The height of the game window.</param>
        /// <param name="player">A reference to the players location.</param>
        /// <output>
        /// No output but the velocity of the bullet will be set after calling this function.
        ///   </output>
        public void setDirectionByMouse(Vector2 mousePosition, Vector2 screenRes)
        {
            float mx = (float)(mousePosition.X - screenRes.X / 2) / (screenRes.X * 0.3f);
            float my = (float)(mousePosition.Y - screenRes.Y / 2) / (screenRes.Y * 0.3f);

            xVel = -mx;
            yVel = -my;

            float len = (float)Math.Sqrt(xVel * xVel + yVel * yVel);
            xVel /= len;
            yVel /= len;

            xVel /= 10;
            yVel /= 10;

            xVel *= -1;
            yVel *= 1;

            prevXPos = xPos;
            prevYPos = yPos;
            move();
            setAngle();
        }

        /// <summary>
        /// Sets the direction to position.
        /// </summary>
        /// <param name="xTarget">The x target.</param>
        /// <param name="yTarget">The y target.</param>
        public void setDirectionToPosition(float xTarget, float yTarget)
        {
            xVel = xPos - xTarget;
            yVel = yPos - yTarget;

            float len = (float)Math.Sqrt(xVel * xVel + yVel * yVel);
            xVel /= len;
            yVel /= len;

            xVel /= 10;
            yVel /= 10;

            prevXPos = xPos;
            prevYPos = yPos;
            move();
            setAngle();
        }

		#endregion Methods 
    }
}
