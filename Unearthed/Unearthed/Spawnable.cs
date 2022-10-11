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
    class Spawnable
    {
        public enum Type
        {
            stalactite, stalagmite, vine, grass, ore
        }

        Tile[,] tiles;       //stores the tiles of the stalactite
        Type SpawnableType;  //stores the type of stalactite 
        Vector2 position;    //stores the position (in terms of tiles, not pixels) and the size
        bool flipped;        //supplied maximum width space to spawn a spawnable
        Vector2 drawOffset;  //offset position to draw from based on the type of spawnable

        static Random rand = new Random(); //used for randomization in spawnable objects

        public Spawnable(Type t, Vector2 position, bool flipped)
        {
            SpawnableType = t;
            this.position = position;
            this.flipped = flipped;
            this.drawOffset = new Vector2(0, 0);
            generate();
        }

        //generates the spawnable based on its type
        private void generate()
        {
            //set random size and fill the object based on its type
            switch (SpawnableType)
            { 
                case Type.stalactite:
                    generateStalactite();
                    break;
                case Type.stalagmite:
                    generateStalagmite();
                    break;
                case Type.vine:
                    generateVine();
                    break;
                case Type.grass:
                    generateGrass();
                    break;
                case Type.ore:
                    generateOreVein();
                    break;
            }
        }

        //generates a stalactite
        private void generateStalactite()
        {
            int width = (int)getRandomRange(3, 7) * Game1.globalScaleFactor;
            tiles = new Tile[(int)(width * getRandomRange(1, 5)), width];
            int gridHeight = tiles.GetLength(0);
            int gridWidth = tiles.GetLength(1);

            //fill the grid with a stalactite
            for (int r = 0; r < gridHeight; r++)
            {
                for (int c = 0; c < gridWidth; c++)
                {
                    if (r <= ((c <= gridWidth / 2) ? c * (gridHeight / gridWidth) : gridHeight - (c * (gridHeight / gridWidth)))) //if it is in the triangle area, than fill it with dirt
                        tiles[r, c] = new Tile(Tile.Type.Dirt);
                    else
                        tiles[r, c] = new Tile(Tile.Type.Air); //otherwise fill it with air to avoid nullPointerException
                }
            }
        }
        //generates a stalagmite
        private void generateStalagmite()
        {
            int width = (int)getRandomRange(3, 7) * Game1.globalScaleFactor;
            tiles = new Tile[(int)(width * getRandomRange(1, 5)), width];
            int gridHeight = tiles.GetLength(0);
            int gridWidth = tiles.GetLength(1);
            drawOffset = new Vector2(0, -tiles.GetLength(0));

            //fill the grid with a stalagmite
            for (int r = 0; r < gridHeight; r++)
            {
                for (int c = 0; c < gridWidth; c++)
                {
                    if (r > ((c <= gridWidth / 2) ? gridHeight - (c * (gridHeight / gridWidth)) : (c * (gridHeight / gridWidth)))) //if it is in the triangle area, than fill it with dirt
                        tiles[r, c] = new Tile(Tile.Type.Dirt);
                    else
                        tiles[r, c] = new Tile(Tile.Type.Air); //otherwise fill it with air to avoid nullPointerException
                }
            }
        }
        //generates a vine
        private void generateVine()
        {
            int width = 1;
            tiles = new Tile[(int)(getRandomRange(4, 16)) * Game1.globalScaleFactor, width];
            int gridHeight = tiles.GetLength(0);
            int gridWidth = tiles.GetLength(1);

            //fill the grid with a vine
            for (int r = 0; r < gridHeight; r++)
            {
                for (int c = 0; c < gridWidth; c++)
                {
                    tiles[r, c] = new Tile(Tile.Type.Plant);
                }
            }
        }
        //generates grass
        private void generateGrass()
        {
            int width = 1;
            tiles = new Tile[(int)(getRandomRange(1, 4)) * Game1.globalScaleFactor, width];
            int gridHeight = tiles.GetLength(0);
            int gridWidth = tiles.GetLength(1);
            drawOffset = new Vector2(0, -tiles.GetLength(0));

            //fill the grid with grass
            for (int r = 0; r < gridHeight; r++)
            {
                for (int c = 0; c < gridWidth; c++)
                {
                    tiles[r, c] = new Tile(Tile.Type.Plant);
                }
            }
        }
        //generates a vein of ore
        private void generateOreVein()
        {
            int width = (int)getRandomRange(4, 12) * Game1.globalScaleFactor;
            int height = (int)getRandomRange(4, 12) * Game1.globalScaleFactor;
            tiles = new Tile[height, width];
            int gridHeight = tiles.GetLength(0);
            int gridWidth = tiles.GetLength(1);

            //fill the grid with air first
            for (int r = 0; r < gridHeight; r++)
            {
                for (int c = 0; c < gridWidth; c++)
                {
                    tiles[r, c] = new Tile(Tile.Type.Ore); 
                }
            }
            /*
            Vector2 origin = new Vector2(gridWidth / 2, gridHeight / 2); //in terms of columns, rows
            Vector2 line = new Vector2((int)(getRandomRange(1, 5)) * Game1.globalScaleFactor, (int)(getRandomRange(1, 5)) * Game1.globalScaleFactor); //columns, rows
            int lineLength = (int)getRandomRange(3, 8);

            for(int i = 0; i < lineLength; i++)
            {
                tiles[(int)(origin.Y + (line.Y * i)), (int)(origin.X + (line.X * i))] = new Tile(Tile.Type.Ore);
            }*/
        }
        
        //returns random number within a specified range
        private double getRandomRange(double min, double max)
        {
            return rand.NextDouble() * (max - min) + min;
        }

        //returns the spawnable's position in the level
        public Vector2 getPosition()
        {
            return position;
        }

        //returns if it should be drawn backwards
        public bool isFlipped()
        {
            return flipped;
        }

        //returns the offset of the origin to draw from
        public Vector2 getOffset()
        {
            return drawOffset;
        }

        //returns the spawnable's tile references
        public Tile[,] getTiles()
        {
            return tiles;
        }

        public Type getType()
        {
            return SpawnableType;
        }
    }
}
