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
    class Level
    {
        Tile[,] tileMap;            //stores the individual pixels (tiles) of the level
        Texture2D tileTex;          //stores the texture (sprite) of the tiles
        public Vector2 drawOffset;  //used when drawing tiles to the screen - draws tiles based on the offset
        int tileSize;               //default size per tile
        Tunnel[] tunnels;           //array storing the tunnel objects
        Boolean tunnelisGenerated;  //Stores whether the tunnels have finished generating
        int tunnelBorderSize;       //the size of the tunnels' borders
        List<Spawnable> spawnables; //Stores the generated cave details to be placed into the map
        List<Spawnable> laterSpawnables; //stores the last spawned spawnables
        public Player p1;                  //Stores a reference to the player
        Boolean levelisGenerated;   //Stores if the level is generatied

        public List<Enemy> enemies;

        static Random rand = new Random(); //used for randomization of the level

        public Level(int rows, int columns, Texture2D tileTex, int tileSize, Player p1)
        {
            tileMap = new Tile[rows, columns];  //initialize the array
            drawOffset = new Vector2(0, 0);     //set default offset
            this.tileTex = tileTex;             //set default tile texture
            this.tileSize = tileSize;           //set default tile size
            tunnelBorderSize = 0;               //set default tunnel border size; mostly used for testing
            spawnables = new List<Spawnable>(); //initialize spawnables list
            laterSpawnables = new List<Spawnable>(); //initialize later spawnables list
            levelisGenerated = false;
            this.p1 = p1;

            enemies = new List<Enemy>();

            //fill the array with plain dirt tiles
            for (int r = 0; r < tileMap.GetLength(0); r++)
            {
                for(int c = 0; c < tileMap.GetLength(1); c++)
                {
                    tileMap[r, c] = new Tile(Tile.Type.Dirt);
                }
            }

            //add in the tunnels
            int tileMapTunnelRatio = (rows * columns) / 2000; //using the correct ratio, scale the number of tunnels based on the size of the map
            int tileMapTunnelRatioY = (int)((1 * Math.Log(tileMapTunnelRatio, 2)) + 11); //logarithmically scales the tunnel count based on level size
            tunnels = new Tunnel[(int)getRandomRange(18, 35) * 2 * Game1.globalScaleFactor];//tileMapTunnelRatio, tileMapTunnelRatio * 2)];
            tunnelisGenerated = false;

            //fill the tunnel array
            for(int i = 0; i < tunnels.Length; i++) //FIX TUNNEL SPAWN AND COLLISION BUG OF GOING OUTSIDE THE MAP; TEST IF IT IS JUST A CAMERA ISSUE//////////////////////////////////////////////////////////////////////////////
            {
                int tunnelSize = (int)getRandomRange(3, 11); //assign the tunnel a random starting size
                tunnelSize *= (int)getRandomRange(2, 3) * Game1.globalScaleFactor; //scale the tunnel
                Rectangle tunnelCoords = new Rectangle((int)getRandomRange(tunnelBorderSize + 1, tileMap.GetLength(1) - tunnelSize - ((tunnelBorderSize + 1) * Game1.globalScaleFactor)), (int)getRandomRange(tunnelBorderSize + 1, tileMap.GetLength(0) - tunnelSize - tunnelBorderSize - 1), tunnelSize, tunnelSize); //tunnelCoords is in terms of columns, rows : x, y (based on tiles not pixels)
                tunnels[i] = new Tunnel(tunnelCoords, tileSize, new Vector2(getColumns(), getRows()), tunnelBorderSize);
                enemies.Add(new Enemy(tunnels[i], this));
            }
        }

        //returns random number within a specified range
        private double getRandomRange(double min, double max)
        {
            return rand.NextDouble() * (max - min) + min;
        }

        //update necessary components of the level
        public void update()
        {
            //continue to update the tunnels until the tunnels have generated
            if (!tunnelisGenerated)
            {
                Boolean anyGenerated = false;
                for (int i = 0; i < tunnels.Length; i++)
                {
                    Tunnel current = tunnels[i];
                    if (current.isAlive())
                    {
                        current.update();

                        //remove any dirt intersecting the tunnel object, creating tunnels in the map
                        for(int r = current.getBounds().Y / tileSize; r < (current.getBounds().Y / tileSize) + (current.getBounds().Height / tileSize); r++)
                        {
                            for (int c = current.getBounds().X / tileSize; c < (current.getBounds().X / tileSize) + (current.getBounds().Width / tileSize); c++)
                            {
                                //check euclidean distance of tiles from the center of the tunnel to make caves circular
                                if(getDistanceEuclidean(new Vector2((current.getBounds().X / tileSize) + ((current.getBounds().Width / tileSize) / 2), (current.getBounds().Y / tileSize) + ((current.getBounds().Height / tileSize) / 2)), new Vector2(c, r)) <= (current.getBounds().Width / tileSize) / 2)
                                {
                                    tileMap[r, c] = new Tile(Tile.Type.Air);
                                }
                            }
                        }
                        anyGenerated = true; //a tunnel was updated
                    }
                }
                if(anyGenerated == false) //if no tunnels updated, then they must all be dead
                {
                    tunnelisGenerated = true;
                    generateDetails(); //because tunnel generation is done, the program can safely iterate through the map checking for tunnel areas
                    generateBorder();
                    p1.spawnPlayer(this);
                    for(int i = -15; i < 15; i++)
                    {
                        for(int a = -15; a < 15; a++)
                        {
                            if (getDistanceEuclidean(p1.getPosition(), new Vector2(p1.getPosition().X + a, p1.getPosition().Y + i)) < 15)
                                tileMap[(int)p1.getPosition().Y + i, (int)p1.getPosition().X + a] = new Tile(Tile.Type.Air);
                        }
                    }
                    levelisGenerated = true;
                }
            }
        }

        //Generate the details of the map such as spawnables and the tunnel outline 
        private void generateDetails()
        {
            int maxSpace = Math.Max(tunnelBorderSize, 1) * Game1.globalScaleFactor; //give enough space to generate details
            int vineColumn = 0; //stores the last vine's column

            for (int r = maxSpace; r < tileMap.GetLength(0) - maxSpace; r++)
            {
                for (int c = maxSpace; c < tileMap.GetLength(1) - maxSpace; c++)
                {
                    //clear out the tunnels
                    clearTunnels(r, c);

                    //check tiles below for air to spawn cieling spawnables
                    if (tileMap[r, c].getType() == Tile.Type.Dirt && tileMap[r + 1, c].getType() == Tile.Type.Air && r < tileMap.GetLength(0) - (32 * Game1.globalScaleFactor) && c < tileMap.GetLength(1) - (8 * Game1.globalScaleFactor) && c > 8 * Game1.globalScaleFactor) //32 is the stalactite max height, 8 is the max width
                    {
                        if (rand.NextDouble() > .8) //chance of spawning
                        {
                            spawnStalactite(r, c); //spawn a stalactite
                        }
                        else if (rand.NextDouble() > .92 && Math.Abs(vineColumn - c) > 1) //vineColumn prevents two vines on the same row from spawning next to each other; an improvement can be made that stores them in an array, as well as a vine row, to prevent all cases of vine overlap
                        {
                            laterSpawnables.Add(new Spawnable(Spawnable.Type.vine, new Vector2(c, r), false)); //spawn a vine
                            vineColumn = c;
                        }
                    }

                    //check tiles above for air to spawn ground spawnables
                    if (tileMap[r, c].getType() == Tile.Type.Dirt && tileMap[r - 1, c].getType() == Tile.Type.Air && r > 32 * Game1.globalScaleFactor && c < tileMap.GetLength(1) - (8 * Game1.globalScaleFactor) && c > 8 * Game1.globalScaleFactor) //32 is the stalagmite max height, 8 is the max width
                    {
                        if (rand.NextDouble() > .92) //chance to spawn
                        {
                            spawnStalagmite(r, c); //spawn a stalagmite
                        }
                        else if (rand.NextDouble() > .85 && Math.Abs(vineColumn - c) > 1)
                        {
                            laterSpawnables.Add(new Spawnable(Spawnable.Type.grass, new Vector2(c, r), false)); //spawn the grass
                            vineColumn = c;
                        }
                    }

                    /*
                    //check tiles for dirt to spawn ore
                    if (tileMap[r, c].getType() == Tile.Type.Dirt && r > 12 * Game1.globalScaleFactor && c < tileMap.GetLength(1) - (12 * Game1.globalScaleFactor))
                    {
                        if (rand.NextDouble() > .95)
                        {
                            laterSpawnables.Add(new Spawnable(Spawnable.Type.ore, new Vector2(c, r), false));
                        }
                    }
                    */
                }
            }

            //copy the arrays of all the spawnables into the main tile map array
            for(int i = 0; i < spawnables.Count; i++)
            {
                for (int r = 0; r < spawnables[i].getTiles().GetLength(0); r++)
                {
                    for (int c = 0; c < spawnables[i].getTiles().GetLength(1); c++)
                    {
                        //tiles are copied beginning at the origin tile of the spawnable
                        if (!spawnables[i].isFlipped())
                        {
                            if (spawnables[i].getTiles()[r, c].getType() == Tile.Type.Dirt || spawnables[i].getTiles()[r, c].getType() == Tile.Type.Plant) //|| spawnables[i].getTiles()[r, c].getType() == Tile.Type.Test) //enable this line if using test tiles
                                tileMap[r + (int)spawnables[i].getPosition().Y + (int)spawnables[i].getOffset().Y, c + (int)spawnables[i].getPosition().X + (int)spawnables[i].getOffset().X] = spawnables[i].getTiles()[r, c]; //if (tileMap[r + (int)spawnables[i].getPosition().Y + (int)spawnables[i].getOffset().Y, c + (int)spawnables[i].getPosition().X + (int)spawnables[i].getOffset().X].getType() == Tile.Type.Air) //if target tile to change is air
                        }
                        else
                        {
                            if (spawnables[i].getTiles()[r, c].getType() == Tile.Type.Dirt || spawnables[i].getTiles()[r, c].getType() == Tile.Type.Plant) //|| spawnables[i].getTiles()[r, c].getType() == Tile.Type.Test) //enable this line if using test tiles
                                tileMap[r + (int)spawnables[i].getPosition().Y + (int)spawnables[i].getOffset().Y, (int)spawnables[i].getPosition().X - c + (int)spawnables[i].getOffset().X] = spawnables[i].getTiles()[r, c];
                        }
                    }
                }
            }

            //iterate through remaining spawnables that should be spawned last
            for (int i = 0; i < laterSpawnables.Count; i++)
            {
                for (int r = 0; r < laterSpawnables[i].getTiles().GetLength(0); r++)
                {
                    for (int c = 0; c < laterSpawnables[i].getTiles().GetLength(1); c++)
                    {
                        //tiles are copied beginning at the origin tile of the spawnable
                        if (laterSpawnables[i].getTiles()[r, c].getType() == Tile.Type.Dirt || laterSpawnables[i].getTiles()[r, c].getType() == Tile.Type.Plant) 
                            if (tileMap[r + (int)laterSpawnables[i].getPosition().Y + (int)laterSpawnables[i].getOffset().Y, c + (int)laterSpawnables[i].getPosition().X + (int)laterSpawnables[i].getOffset().X].getType() == Tile.Type.Air)
                                tileMap[r + (int)laterSpawnables[i].getPosition().Y + (int)laterSpawnables[i].getOffset().Y, c + (int)laterSpawnables[i].getPosition().X + (int)laterSpawnables[i].getOffset().X] = laterSpawnables[i].getTiles()[r, c];
                        /*
                        else if(laterSpawnables[i].getTiles()[r, c].getType() == Tile.Type.Ore)
                            if (tileMap[r + (int)laterSpawnables[i].getPosition().Y + (int)laterSpawnables[i].getOffset().Y, c + (int)laterSpawnables[i].getPosition().X + (int)laterSpawnables[i].getOffset().X].getType() == Tile.Type.Dirt)
                                tileMap[r + (int)laterSpawnables[i].getPosition().Y + (int)laterSpawnables[i].getOffset().Y, c + (int)laterSpawnables[i].getPosition().X + (int)laterSpawnables[i].getOffset().X] = laterSpawnables[i].getTiles()[r, c];
                                */
                    }
                }
            }
        }

        //clears the tunnel area in a radius equal to half the tunnels width
        private void clearTunnels(int r, int c)
        {
            //check tiles in a radius based on the tunnelBorderSize to create the bordering colors for the tunnels
            for (int rOff = -tunnelBorderSize; rOff <= tunnelBorderSize; rOff++)
            {
                Boolean br = false;
                for (int cOff = -tunnelBorderSize; cOff <= tunnelBorderSize; cOff++)
                {
                    if (tileMap[r + rOff, c + cOff].getType() == Tile.Type.Air && tileMap[r, c].getType() == Tile.Type.Dirt)
                    {
                        tileMap[r, c].setColor(new Color(150, 90, 20)); //visual effect; give dirt tiles bordering tunnels a lighter color
                        br = true;
                        break;
                    }
                    if (br == true)
                        break;
                }
            }
        }

        //checks for space to spawn a stalactite, and does so if there is enough space
        private void spawnStalactite(int r, int c)
        {
            Boolean enoughSpace = true;
            bool flipped = false;

            //check if there is enough space to spawn a stalactite to the right
            for (int i = 0; i < 3 * Game1.globalScaleFactor; i++)
            {
                if (tileMap[r, c + i].getType() == Tile.Type.Air)
                {
                    enoughSpace = false;
                    break;
                }
            }
            if (enoughSpace == false) //if not, check if there is enough space to the left
            {
                enoughSpace = true;
                for (int i = 0; i < 3 * Game1.globalScaleFactor; i++)
                {
                    if (tileMap[r, c - i].getType() == Tile.Type.Air)
                    {
                        enoughSpace = false;
                        break;
                    }
                }
                if (enoughSpace == true) //if there is enough space to the left, the stalactite drawing must be flipped
                    flipped = true;
            }

            if (enoughSpace == true) //if there is enough space, generate the stalactite
                spawnables.Add(new Spawnable(Spawnable.Type.stalactite, new Vector2(c, r), flipped));
        }

        //checks for space to spawn a stalagmite, and does so if there is enough space
        private void spawnStalagmite(int r, int c)
        {
            Boolean enoughSpace = true;
            bool flipped = false;

            //check if there is enough space to spawn a stalagmite to the right
            for (int i = 0; i < 3 * Game1.globalScaleFactor; i++)
            {
                if (tileMap[r, c + i].getType() == Tile.Type.Air)
                {
                    enoughSpace = false;
                    break;
                }
            }
            if (enoughSpace == false) //if not, check if there is enough space to the left
            {
                enoughSpace = true;
                for (int i = 0; i < 3 * Game1.globalScaleFactor; i++)
                {
                    if (tileMap[r, c - i].getType() == Tile.Type.Air)
                    {
                        enoughSpace = false;
                        break;
                    }
                }
                if (enoughSpace == true) //if there is enough space to the left, the stalagmite drawing must be flipped
                    flipped = true;
            }

            if (enoughSpace == true) //if there is enough space, generate the stalagmite
                spawnables.Add(new Spawnable(Spawnable.Type.stalagmite, new Vector2(c, r), flipped));
        }

        //generates the border of the world
        private void generateBorder()
        {
            for(int i = 0; i < getColumns(); i++)
            {
                for(int a = 0; a < 72; a++)
                {
                    tileMap[a, i] = new Tile(Tile.Type.Unbreakable);
                }
            }

            for (int i = 0; i < getColumns(); i++)
            {
                for (int a = 0; a < 72; a++)
                {
                    tileMap[getRows() - 72 + a, i] = new Tile(Tile.Type.Unbreakable);
                }
            }

            for (int i = 0; i < 100; i++)
            {
                for (int a = 0; a < getRows(); a++)
                {
                    tileMap[a, i] = new Tile(Tile.Type.Unbreakable);
                }
            }

            for (int i = 0; i < 100; i++)
            {
                for (int a = 0; a < getRows(); a++)
                {
                    tileMap[a, getColumns() - 100 + i] = new Tile(Tile.Type.Unbreakable);
                }
            }
        }

        //change the draw offset of the camera position by adding it to another vector
        public void incrementOffset(Vector2 other, Camera cam)
        {
            int height = tileMap.GetLength(0);
            int width = tileMap.GetLength(1);
            if (-cam.getCameraPosition().Y + (height / tileSize) - (other.Y/tileSize) <= height + 147 && -cam.getCameraPosition().Y - (other.Y / tileSize) >= 0)
                cam.incrementCameraY((int)other.Y / tileSize); //drawOffset.Y += other.Y / tileSize; //left in case of camera issues
            if (-cam.getCameraPosition().X + (width / tileSize) - (other.X / tileSize) <= width + 203 && -cam.getCameraPosition().X - (other.X / tileSize) >= 0)
                cam.incrementCameraX((int)other.X / tileSize); //drawOffset.X += other.X / tileSize; //left in case of camera issues
        }

        //return the number of rows(height)
        public int getRows()
        {
            return tileMap.GetLength(0);
        }
        //return the number of columns(width)
        public int getColumns()
        {
            return tileMap.GetLength(1);
        }

        //set a tile in the map
        public void setTile(int row, int col, Tile t)
        {
            tileMap[row, col] = t;
        }
        //get a tile in the map
        public Tile getTile(int row, int col)
        {
            return tileMap[row, col];
        }

        //returns whether the level has finished generating
        public Boolean isFinishedGenerating()
        {
            return levelisGenerated;
        }

        //returns the distance between two points via the Euclidean distance formula
        private int getDistanceEuclidean(Vector2 pointOne, Vector2 pointTwo)
        {
            return (int)Math.Sqrt(((pointTwo.X - pointOne.X) * (pointTwo.X - pointOne.X)) + ((pointTwo.Y - pointOne.Y) * (pointTwo.Y - pointOne.Y)));
        }
        //returns the distance between two points via the Manhattan distance formula
        private int getDistanceManhattan(Vector2 pointOne, Vector2 pointTwo)
        {
            return (int)(Math.Abs(pointOne.X - pointTwo.X) + Math.Abs(pointOne.Y - pointTwo.Y));
        }

        //iterate through the array, drawing the tiles to the screen by calling their draw methods
        public void Draw(SpriteBatch sb, Camera draw) 
        {
            int height = tileMap.GetLength(0);
            int width = tileMap.GetLength(1);

            //draw based on camera position
            for (int r = -(int)draw.getCameraPosition().Y; r < -(int)draw.getCameraPosition().Y + draw.getCameraPosition().Height; r++)
            {
                for (int c = -(int)draw.getCameraPosition().X; c < -(int)draw.getCameraPosition().X + draw.getCameraPosition().Width; c++)
                {
                    //draw tiles at coordinates based on their position and their tiles size
                    Rectangle tempRect = new Rectangle((int)(((c * tileSize) + (draw.getCameraPosition().X * tileSize))) * Game1.cameraRatio, (int)(((r * tileSize) + (draw.getCameraPosition().Y * tileSize))) * Game1.cameraRatio, tileSize * Game1.cameraRatio, tileSize * Game1.cameraRatio);
                    tileMap[r, c].Draw(sb, tempRect, tileTex);
                }
            }
        }

        public void offSetEnemies(int x, int y)
        {
            for(int i = 0; i < enemies.Count; i++)
            {
                enemies[i].hitBox.X += x;
                enemies[i].hitBox.Y += y;
            }
        }
    }
}
