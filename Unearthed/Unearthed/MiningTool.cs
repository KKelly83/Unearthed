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
    class MiningTool
    {
        public Rectangle toolRectangle;//rectangle location of tool

        public Boolean isMining;//if the tool is mining
        public Boolean isBuilding;//if the tool is building

        Vector2 mousePosition; //gets the postion of the mouse
        public float toolRotation;//tool's rotation
        public float visualIndicatorRotation; //indicator faced direction based on toolRotation
        public String toolRotationString;

        public Vector2 usedRotationVector; //vector2 to hold the currently used rotation point
        Vector2 rightVector;
        Vector2 leftVector;

        KeyboardState oldKB; //used to check to see individual key presses
        MouseState oldMouse; //used to check for individual clicks w/ mouse

        int miningCooldown;
        int miningCooldownReset;

        public Boolean equipped;

        Rectangle indicatorRectangle;

        Texture2D spriteSheet;
        Texture2D indicatorTexture;
        Rectangle defaultSource;
        Rectangle usedSource;

        int swingFrameCooldown;
        int swingFrameCounter;

        Boolean recentlySwung;
        public Boolean isFlipped;

        public MiningTool(Rectangle l, Texture2D sS, Texture2D iT, Rectangle iR)
        {
            toolRectangle = l;
            spriteSheet = sS;
            indicatorTexture = iT;
            indicatorRectangle = iR;
            isMining = false;
            isBuilding = false;

            miningCooldownReset = 60;
            miningCooldown = miningCooldownReset;

            defaultSource = new Rectangle(0, 0, 128, 128);
            usedSource = defaultSource;
            swingFrameCounter = 0;
            swingFrameCooldown = 0;

            isFlipped = false;
            recentlySwung = false;
        }

        public void update(Level level, Vector2 playerPosition)
        {
            KeyboardState kb = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            mousePosition = new Vector2(mouse.X, mouse.Y);

            toolRotation = (float)Math.Atan2((mouse.Y - toolRectangle.Y), (mouse.X - toolRectangle.X)); //uses trigonometry to get the angle between the mouse and the tool

            //Actual tool rotation stuff
            if (MathHelper.ToDegrees(toolRotation) > -45 && MathHelper.ToDegrees(toolRotation) < 45)//facing right
            {
                visualIndicatorRotation = 0;
                toolRotationString = "right";
            }
            else if (MathHelper.ToDegrees(toolRotation) > -135 && MathHelper.ToDegrees(toolRotation) < -45)//facing up
            {
                visualIndicatorRotation = -90;
                toolRotationString = "up";
            }
            else if (MathHelper.ToDegrees(toolRotation) > 45 && MathHelper.ToDegrees(toolRotation) < 135)//facing down
            {
                visualIndicatorRotation = 90;
                toolRotationString = "down";
            }
            else //facing left
            {
                visualIndicatorRotation = 180;
                toolRotationString = "left";
            }


            if (kb.IsKeyDown(Keys.D1)) //checks to see if left ctrl is pressed, will be used for drawing purposes mostly
                equipped = true;
            else if(kb.IsKeyDown(Keys.D2) || kb.IsKeyDown(Keys.D3))
                equipped = false;

            if (equipped)
            {
                if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                {
                    isMining = true;
                }

                if (mouse.RightButton == ButtonState.Pressed && oldMouse.RightButton == ButtonState.Released)
                    isBuilding = true;
            }

            if (miningCooldown < 60)
                miningCooldown++;

            if (isMining && miningCooldown == 60)
            {
                destroyBlocks(level, playerPosition);
                recentlySwung = true;
            }
            if (isBuilding && miningCooldown == 60)
            {
                buildBlocks(level, playerPosition);
            }
            if (recentlySwung)
            {
                animateSwing();
            }
            if (MathHelper.ToDegrees(toolRotation) > 90 || MathHelper.ToDegrees(toolRotation) < -90) //checks to see if the degree made by the mouse is on the left side of the screen
            {
                isFlipped = true;
            }
            else
            {
                isFlipped = false;
            }
            oldKB = kb;
            oldMouse = mouse;
        }

        public void destroyBlocks(Level level, Vector2 playerPosition)
        {
            if (toolRotationString.Equals("right"))
            {
                Tile[,] tilesToMine = new Tile[13, 20];
                for (int row = 0; row < tilesToMine.GetLength(0); row++)
                {
                    for (int col = 0; col < tilesToMine.GetLength(1); col++)
                    {
                        if (!level.getTile(row + (int)playerPosition.Y - 6, col + (int)playerPosition.X + 3).isBedrock())
                        {
                            level.setTile(row + (int)playerPosition.Y - 6, col + (int)playerPosition.X + 3, new Tile(Tile.Type.Air));
                        }
                    }
                }
            }
            if (toolRotationString.Equals("up"))
            {
                Tile[,] tilesToMine = new Tile[20, 8];
                for (int row = 0; row < tilesToMine.GetLength(0); row++)
                {
                    for (int col = 0; col < tilesToMine.GetLength(1); col++)
                    {
                        if (!level.getTile(row + (int)playerPosition.Y - 22, col + (int)playerPosition.X - 4).isBedrock())
                        {
                            level.setTile(row + (int)playerPosition.Y - 22, col + (int)playerPosition.X - 4, new Tile(Tile.Type.Air));
                        }                    
                    }
                }
            }
            if (toolRotationString.Equals("down"))
            {
                Tile[,] tilesToMine = new Tile[20, 8];
                for (int row = 0; row < tilesToMine.GetLength(0); row++)
                {
                    for (int col = 0; col < tilesToMine.GetLength(1); col++)
                    {
                        if (!level.getTile(row + (int)playerPosition.Y + 2, col + (int)playerPosition.X - 4).isBedrock())
                            level.setTile(row + (int)playerPosition.Y + 2, col + (int)playerPosition.X - 4, new Tile(Tile.Type.Air));
                    }
                }
            }
            if (toolRotationString.Equals("left"))
            {
                Tile[,] tilesToMine = new Tile[13, 20];
                for (int row = 0; row < tilesToMine.GetLength(0); row++)
                {
                    for (int col = 0; col < tilesToMine.GetLength(1); col++)
                    {
                        if (!level.getTile(row + (int)playerPosition.Y - 6, col + (int)playerPosition.X - 23).isBedrock())
                            level.setTile(row + (int)playerPosition.Y - 6, col + (int)playerPosition.X - 23, new Tile(Tile.Type.Air));
                    }
                }
            }
            miningCooldown = 0;
            isMining = false;
        }

        public void buildBlocks(Level level, Vector2 playerPosition)
        {
            if (toolRotationString.Equals("right"))
            {
                Tile[,] tilesToMine = new Tile[8, 8];
                for (int row = 0; row < tilesToMine.GetLength(0); row++)
                {
                    for (int col = 0; col < tilesToMine.GetLength(1); col++)
                    {
                        level.setTile(row + (int)playerPosition.Y - 5, col + (int)playerPosition.X + 2, new Tile(Tile.Type.Dirt));
                    }
                }
            }
            if (toolRotationString.Equals("up"))
            {
                Tile[,] tilesToMine = new Tile[8, 8];
                for (int row = 0; row < tilesToMine.GetLength(0); row++)
                {
                    for (int col = 0; col < tilesToMine.GetLength(1); col++)
                    {
                        level.setTile(row + (int)playerPosition.Y - 15, col + (int)playerPosition.X - 4, new Tile(Tile.Type.Dirt));
                    }
                }
            }
            if (toolRotationString.Equals("down"))
            {
                Tile[,] tilesToMine = new Tile[8, 8];
                for (int row = 0; row < tilesToMine.GetLength(0); row++)
                {
                    for (int col = 0; col < tilesToMine.GetLength(1); col++)
                    {
                        level.setTile(row + (int)playerPosition.Y + 7, col + (int)playerPosition.X - 4, new Tile(Tile.Type.Dirt));
                    }
                }
            }
            if (toolRotationString.Equals("left"))
            {
                Tile[,] tilesToMine = new Tile[8, 8];
                for (int row = 0; row < tilesToMine.GetLength(0); row++)
                {
                    for (int col = 0; col < tilesToMine.GetLength(1); col++)
                    {
                        level.setTile(row + (int)playerPosition.Y - 5, col + (int)playerPosition.X - 12, new Tile(Tile.Type.Dirt));
                    }
                }
            }
            miningCooldown = 0;
            isBuilding = false;
        }
        public void animateSwing()
        {
            if (swingFrameCounter < 9 && swingFrameCooldown % 3 == 0) //iterating through array
            {
                usedSource = new Rectangle(0 + (swingFrameCounter * 128), 0, 128, 128);
                swingFrameCounter++;
            }
            swingFrameCooldown++;
            if (swingFrameCounter == 9) //if x has reached the end of the array, reset to defaults
            {
                recentlySwung = false;
                usedSource = defaultSource;
                swingFrameCooldown = 0;
                swingFrameCounter = 0;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(spriteSheet, toolRectangle, usedSource, Color.White, toolRotation,new Vector2(0,64), (isFlipped) ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);
            sb.Draw(indicatorTexture, indicatorRectangle, new Rectangle(0,0,16,16), Color.Red* 0.2f, MathHelper.ToRadians(visualIndicatorRotation), new Vector2(0,8), SpriteEffects.None, 0);
        }
    }
}
