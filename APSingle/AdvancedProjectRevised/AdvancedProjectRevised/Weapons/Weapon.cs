﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace AP
{
    /// <summary>
    /// The class that will keep track of weapons and ammunition for a player.
    /// Each player will have it's own instance of this class.
    /// Contributors: Gage Patterson, Scott Herman, Kyle Galvin, Adam Humeniuk
    /// Revision: 285
    /// </summary>
    public class Weapon : Position
    {
		#region Fields (17) 

        private int bulletCooldown;
        Vector3 defaultVelocity = new Vector3(0, 0, 0);
        private bool grenadeAvailable = false;
        private UInt16 grenadeCount = 3;
        public bool pistolEquipped = false;
        public int rifleAmmo = 60;
        private bool rifleAvailable = false;
        private UInt16 rifleBulletCount = 0;
        public int rifleBurstCooldown = 0;
        public bool rifleEquipped = false;
        private bool rocketAvailable = false;
        private UInt16 rocketBulletCount = 0;
        public bool rocketEquipped = false;
        public int shotgunAmmo = 10;
        private bool shotgunAvailable = false;
        private UInt16 shotgunBulletCount = 0;
        public bool shotgunEquipped = false;

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Initializes a new instance of the <see cref="Weapon"/> class.
        /// </summary>
        public Weapon()
        {
            pistolEquipped = true;
            bulletCooldown = 1;
        }

		#endregion Constructors 

		#region Methods (13) 

		// Public Methods (13) 

        /// <summary>
        /// Determines whether this instance can shoot.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can shoot; otherwise, <c>false</c>.
        /// </returns>
        public bool canShoot()
        {
            
            if (bulletCooldown <= 0)
            {
                if (pistolEquipped)
                {
                    bulletCooldown = 10;
                    return true;
                }
                else if (rifleEquipped)
                {
                    if (rifleBurstCooldown < 2)
                    {
                        bulletCooldown = 3;
                        rifleBurstCooldown++;
                    }
                    else
                    {
                        bulletCooldown = 10;
                        rifleBurstCooldown = 0;
                    }
                    if (rifleAmmo <= 0)
                    {
                        equipPistol();
                    }
                    else
                    {
                        return true;
                    }
                }
                else if (shotgunEquipped)
                {
                    bulletCooldown = 10;
                    if (shotgunAmmo <= 0)
                        equipPistol();
                    else
                    {
                        return true;
                    }
                }                
            }
            return false;
        }

        /// <summary>
        /// Equips the pistol.
        /// </summary>
        public void equipPistol()
        {
            rifleEquipped = false;
            pistolEquipped = true;
            rocketEquipped = false;
            shotgunEquipped = false;
            bulletCooldown = 1;
            rifleBurstCooldown = 0;
        }

        /// <summary>
        /// Equips the rifle.
        /// </summary>
        public void equipRifle()
        {
            rifleEquipped = true;
            pistolEquipped = false;
            rocketEquipped = false;
            shotgunEquipped = false;
            bulletCooldown = 1;
            rifleBurstCooldown = 0;
        }

        /// <summary>
        /// Equips the rocket.
        /// </summary>
        public void equipRocket()
        {
            rifleEquipped = false;
            pistolEquipped = false;
            rocketEquipped = true;
            shotgunEquipped = false;
        }

        /// <summary>
        /// Equips the shotgun.
        /// </summary>
        public void equipShotgun()
        {
            rifleEquipped = false;
            pistolEquipped = false;
            rocketEquipped = false;
            shotgunEquipped = true;
            bulletCooldown = 1;
            rifleBurstCooldown = 0;
        }

        /// <summary>
        /// Called when grenades are attained
        /// </summary>
        public void grenadeAttained()
        {
            if (grenadeCount != 3)
            {
                grenadeCount++;
                grenadeAvailable = true;
            }

        }

        /// <summary>
        /// Called when rifle is attained
        /// </summary>
        public void rifleAttained()
        {
            if (rifleBulletCount != 100)
            {
                rifleBulletCount += 30;
                rifleAvailable = true;
            }
        }

        /// <summary>
        /// Called when rocket is attained
        /// </summary>
        public void rocketAttained()
        {
            if (rocketBulletCount != 3)
            {
                rocketBulletCount++;
                rocketAvailable = true;
            }
        }

        /// <summary>
        /// Shoots the specified bullet.
        /// </summary>
        /// <param name="bulletList">The bullet list.</param>
        /// <param name="player">The player.</param>
        /// <param name="screenX">The screen X.</param>
        /// <param name="screenY">The screen Y.</param>
        /// <param name="mouseX">The mouse X.</param>
        /// <param name="mouseY">The mouse Y.</param>
        //public void shoot(ref List<Bullet> bulletList, ref Player player, int screenX, int screenY, float mouseX, float mouseY)
        public void shoot(ref List<Bullet> bulletList, Vector3 playerPosition, Vector2 screenSize, Vector2 mousePosition)
        {
            if (ClientProgram.multiplayer)
            {
                Bullet b = new Bullet(playerPosition, new Vector3(mousePosition.X, mousePosition.Y, 0), 30);
                b.setDirectionByMouse(mousePosition, screenSize);
                b.mousePos.X = b.xVel;
                b.mousePos.Y = b.yVel;
                b.setID(ServerProgram.bulletID++);
                bulletList.Add(b);
            }
            else
            {
                
            }
        }

        /// <summary>
        /// Shoots the specified bullet.
        /// </summary>
        /// <param name="bulletList">The bullet list.</param>
        /// <param name="player">The player.</param>
        /// <param name="screenX">The screen X.</param>
        /// <param name="screenY">The screen Y.</param>
        /// <param name="mouseX">The mouse X.</param>
        /// <param name="mouseY">The mouse Y.</param>
        //public void shoot(ref List<Bullet> bulletList, ref Player player, int screenX, int screenY, float mouseX, float mouseY)
        public void shoot(ref List<Bullet> bulletList, Vector3 playerPosition, Vector2 screenSize, Vector2 mousePosition, ref Player player)
        {
            if (pistolEquipped)
            {
                ClientProgram.soundHandler.play(SoundHandler.SILENCER);
                bulletList.Add(new Bullet(player.position, defaultVelocity, 40));
                bulletList.Last().setDirectionByMouse(mousePosition, screenSize);
            }
            else if (rifleEquipped)
            {
                ClientProgram.soundHandler.play(SoundHandler.GUNSHOT);
                bulletList.Add(new Bullet(player.position, defaultVelocity, 30));
                bulletList.Last().setDirectionByMouse(mousePosition, screenSize);
                rifleAmmo--;
            }
            else if (shotgunEquipped)
            {
                ClientProgram.soundHandler.play(SoundHandler.EXPLOSION);

                float mx = (float)(mousePosition.X - screenSize.X / 2) / (screenSize.X * 0.3f);
                float my = (float)(mousePosition.Y - screenSize.Y / 2) / (screenSize.Y * 0.3f);

                float xVelo = -mx;
                float yVelo = -my;

                float len = (float)Math.Sqrt(xVelo * xVelo + yVelo * yVelo);
                xVelo /= len;
                yVelo /= len;

                xVelo /= 10;
                yVelo /= -10;

                int spread = 100; //higher spread value makes the spread cover less area
                Vector2 spreadTarget = new Vector2(player.xPos + xVelo * spread, player.yPos + yVelo * spread);

                bulletList.Add(new Bullet(player.position, defaultVelocity, 15));
                bulletList.Last().setDirectionByMouse(mousePosition, screenSize);
                bulletList.Add(new Bullet(player.position, defaultVelocity, 15));
                bulletList.Last().setDirectionToPosition(spreadTarget.X + 1, spreadTarget.Y + 1);
                bulletList.Add(new Bullet(player.position, defaultVelocity, 15));
                bulletList.Last().setDirectionToPosition(spreadTarget.X - 1, spreadTarget.Y + 1);
                bulletList.Add(new Bullet(player.position, defaultVelocity, 15));
                bulletList.Last().setDirectionToPosition(spreadTarget.X - 1, spreadTarget.Y - 1);
                bulletList.Add(new Bullet(player.position, defaultVelocity, 15));
                bulletList.Last().setDirectionToPosition(spreadTarget.X + 1, spreadTarget.Y - 1);
                shotgunAmmo--;
            }
            else if (rocketEquipped)
            {
                bulletList.Add(new Bullet(player.position, defaultVelocity, 45));
                bulletList.Last().setDirectionByMouse(mousePosition, screenSize);
            }
        }

        /// <summary>
        /// Called when shotgun is attained
        /// </summary>
        public void shotgunAttained()
        {
            if (shotgunBulletCount != 36)
            {
                shotgunBulletCount = 12;
                shotgunAvailable = true;
            }
        }

        /// <summary>
        /// Updates the bullet cooldown.
        /// </summary>
        public void updateBulletCooldown()
        {
            bulletCooldown--;
        }

        /// <summary>
        /// Updates the time stamp.
        /// </summary>
        public void updateTimeStamp()
        {
            timestamp = DateTime.Now.Ticks;
        }

		#endregion Methods 
    }
}
