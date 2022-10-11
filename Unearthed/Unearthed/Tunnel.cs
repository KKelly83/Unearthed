using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Unearthed
{
    class Tunnel
    {
        enum Type
        {
            Cavern = 0,
            Shaft = 1,
        }

        public Rectangle bounds;   //stores the hitbox, size, and position; may change to a circle - bounds are stored in units of tiles, not pixels. X, Y is stored in columns, rows
        Vector2 velocity;   //stores the velocity of the tunneling object - velocity is stored in units of tiles, not pixels
        int tileSize;       //stores the size of the tiles
        int life;           //stores the life (time in game ticks) of the tunnel object
        Vector2 outerBounds;//stores the outside boundries of the map
        int tunnelBorder;   //stores the size of the bordering dirt around the tunnel
        Type tunnelType;    //stores the type of tunnel 

        static Random rand = new Random(); //used for random movement and resizing of tunnels; TINKER WITH RANDOM SEEDS, SAVING SEEDS

        public Tunnel(Rectangle coords, int tileSize, Vector2 outerBounds, int tunnelBorderRadius)
        {
            this.tileSize = tileSize;
            bounds = new Rectangle((int)coords.X * tileSize, (int)coords.Y * tileSize, coords.Width * tileSize, coords.Height * tileSize);
            velocity = new Vector2((float)getRandomRange(-5, 5), (float)getRandomRange(-5, 5)) * (Game1.globalScaleFactor/2);

            //int tileMapTunnelLifeRatio = (int)((outerBounds.X * outerBounds.Y) / 1000); //scale the max tunnel length (using life) based on the size of the map
            //int tileMapTunnelRatioY = (int) ((Math.Log(tileMapTunnelLifeRatio, 2)) + 70); //the tunnel length scales logarithmically to prevent too large tunnels
            life = (int)getRandomRange(10, ((bounds.Width > 15 * Game1.globalScaleFactor) ? 100 : 80));//tileMapTunnelLifeRatio);

            this.outerBounds = new Vector2(outerBounds.X * tileSize, outerBounds.Y * tileSize); //outer bounds of the level's tile array
            tunnelBorder = tunnelBorderRadius * tileSize;
            tunnelType = (Type)rand.NextDouble();
        }

        //update the position of the tunnel, then generate a new size and velocity; decrement the life counter
        public void update()
        {
            //move the tunnel, taking into account tile size
            if(bounds.X + ((int)velocity.X * tileSize) + bounds.Width < outerBounds.X - tileSize - tunnelBorder && bounds.X + ((int)velocity.X * tileSize) > tunnelBorder + tileSize)
                bounds.X += (int)velocity.X * tileSize;
            if(bounds.Y + ((int)velocity.Y * tileSize) + bounds.Height < outerBounds.Y - tileSize - tunnelBorder && bounds.Y + ((int)velocity.Y * tileSize) > tunnelBorder + tileSize)
                bounds.Y += (int)velocity.Y * tileSize;
            if (tunnelType == Type.Cavern) //if it is a cavern, set the velocity to a completely random value, resulting in more clustered cave areas (multiplied to increase overall cave size)
                velocity = new Vector2((int)getRandomRange(-5, 5), (int)getRandomRange(-5, 5)) * ((bounds.Width > 15 * Game1.globalScaleFactor) ? 3 : 2) * Game1.globalScaleFactor;
            if (tunnelType == Type.Shaft) //if it is a shaft, change velocity by a small amount to create straighter tunnels (multiplied to increase overall cave size)
                velocity += new Vector2((int)getRandomRange(-1, 1), (int)getRandomRange(-1, 1)) * ((bounds.Width > 15 * Game1.globalScaleFactor) ? 3 : 2) * Game1.globalScaleFactor;

            //change the size of the tunnel, checking if it will be out of bounds
            do
            {
                int sizeChange = (int)getRandomRange(-1, 1) * tileSize * ((bounds.Width > 15 * Game1.globalScaleFactor) ? 3 : 2) * Game1.globalScaleFactor; 
                bounds.Width += sizeChange;
                bounds.Height += sizeChange;
            } while (bounds.X + bounds.Width >= outerBounds.X - tileSize - tunnelBorder || bounds.Y + bounds.Height >= outerBounds.Y - tileSize - tunnelBorder); 
            
            life--; //decrement the life
        }

        //return true if the tunnel is alive
        public Boolean isAlive()
        {
            return life > 0;
        }

        //return true if the tunnel hitbox intersects the supplied rectangle
        public Boolean intersects(Rectangle other)
        {
            return bounds.Intersects(other);
        }

        //return the tunnel's hitbox
        public Rectangle getBounds()
        {
            return bounds;
        }

        //returns random number within a specified range
        private double getRandomRange(double min, double max)
        {
            return rand.NextDouble() * (max - min) + min;
        }
    }
}
