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
    class Tile
    {
        public enum Type
        {
            Dirt, Air, Plant, Ore, Unbreakable, Test
        }

        Color col;       //color of the tile  
        Type tileType;   //the type of tile
        bool collidable; //whether it uses collisions

        static Random rand = new Random(); //used for randomization of tiles

        public Tile(Type type)
        {
            tileType = type;

            //red, green, blue, alpha
            int r = 0;
            int g = 0;
            int b = 0;
            int a = 0;

            //change the saturation of the color by a random amount
            int satOffset = (int)getRandomRange(-17, 17);

            //defaults to not being collidable
            collidable = false;

            //get the color from the tile type
            switch (type)
            {
                case Type.Dirt: //dirt
                    r = 117; g = 77; b = 17; a = 255;
                    collidable = true;
                    break;

                case Type.Air: //air
                    r = 40; g = 30; b = 8; a = 0;
                    break;

                case Type.Plant: //plant - genaric green
                    r = 10; g = 160; b = 30; a = 255;
                    break;

                case Type.Ore: //Ore
                    r = 100; g = 100; b = 100; a = 255;
                    break;

                case Type.Unbreakable: //Unbreakable
                    r = 30; g = 30; b = 30; a = 255;
                    collidable = true;
                    break;

                case Type.Test: //test - sets tile red
                    r = 255; g = 0; b = 0; a = 255;
                    break;

            }

            //set the tile's color with offset saturation
            if(this.tileType == Type.Air)
                this.col = new Color(r, g, b, a);
            else
                this.col = new Color(r + satOffset, g + satOffset, b + satOffset, a);
        }

        //returns random number within a specified range
        private double getRandomRange(double min, double max)
        {
            return rand.NextDouble() * (max - min) + min;
        }

        //returns the type of this tile
        public Type getType()
        {
            return this.tileType;
        }
        public Boolean isBedrock()
        {
            if (this.tileType == Type.Unbreakable)
                return true;
            else
                return false;
        }
        //returns this object's color
        public Color getColor()
        {
            return col;
        }
        //set the color
        public void setColor(Color c)
        {
            this.col = c;
        }
        //returns if it is collidable
        public bool isCollidable()
        {
            return collidable;
        }

        //draw the tile
        public void Draw(SpriteBatch sb, Rectangle rect, Texture2D tex)
        {
            sb.Draw(tex, rect, col);
        }
    }
}
