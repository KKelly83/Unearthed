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
    class Player
    {
        public enum playerClass
        {
            miner, xenobiologist, scout, soldier
        }
        Texture2D playerTex;    //player texture
        Rectangle hitbox;       //player hitbox
        Rectangle srcRect;      //source rectangle
        Vector2 position;       //coords based on the Level's Tilemap
        Camera cam;
        int tileSize;           //tile size

        //movement variables
        readonly double grav = 1;
        double movespeed;
        double jumpSpeed;
        bool jumping;

        int leftbounds;
        int rightbounds;
        int upbounds;
        int lowbounds;

        double hspd;
        double vspd;
        int tvspd;

        Boolean canMove;

        //sprite drawing
        Boolean isFlipped;
        int imageIndex;
        int defaultIndex;
        int timer;

        //set up a test area
        Boolean setUpArea;
        Boolean spawn;

        Weapon weapon1;
        Weapon weapon2;
        Weapon equippedWeapon;
        MiningTool mining;

        double hp;
        bool isAlive;
        int points;

        public Player(Texture2D tex, int screenWidth, int ScreenHeight, int tileSize, playerClass character, Weapon w1, Weapon w2, MiningTool m)
        {
            this.playerTex = tex;
            this.hitbox = new Rectangle(screenWidth / 2 - 18, ScreenHeight / 2 - 36, 36, 72);
            this.srcRect = new Rectangle(0, 0, 72, 144);
            position = new Vector2(screenWidth / 10, ScreenHeight / 10);
            leftbounds = (int)position.X;
            upbounds = (int)position.Y;
            this.tileSize = tileSize;

            mining = m;

            cam = new Camera(new Rectangle(0, 0, screenWidth / tileSize / 1, ScreenHeight / tileSize / 1));

            //movement vars
            grav = .2;
            movespeed = tileSize;
            jumpSpeed = 4;
            jumping = false;
            
            hspd = 0;
            vspd = 0;
            tvspd = 0;

            canMove = false; //player can't move until put in the spawn location

            //set up a test area for movement - set to false to make the area
            setUpArea = false;
            spawn = false;

            //sprite drawing
            isFlipped = false;
            defaultIndex = 6;
            imageIndex = defaultIndex;
            timer = 0;

            //weapon initializing
            weapon1 = w1;
            weapon2 = w2;
            equippedWeapon = w1;

            hp = 100;
            isAlive = true;
            points = 0;

        }

        public void getInput(Level testLevel)
        {
            KeyboardState kb = Keyboard.GetState();

            /* //used for testing
            if (setUpArea == true && kb.IsKeyDown(Keys.G))
            {
                for (int i = 40; i < 80; i++)
                {
                    for (int a = 40; a < 120; a++)
                    {
                        testLevel.setTile((int)position.Y + i, (int)position.X + a, new Tile(Tile.Type.Air));
                    }
                }
                setUpArea = false;
                position.X += 55;
                position.Y += 55;

                Game1.incrementBackground(55, 55);
                testLevel.incrementOffset(new Vector2(-55 * (float)movespeed, (float)((-55) * movespeed)), cam);
            }
            */
            int jump = (kb.IsKeyDown(Keys.Space)) ? 1 : 0;

            float speed = (float)movespeed; 

            int up = (kb.IsKeyDown(Keys.W)) ? 1 : 0;
            int down = (kb.IsKeyDown(Keys.S)) ? -1 : 0;

            int left = (kb.IsKeyDown(Keys.A)) ? 1 : 0;
            int right = (kb.IsKeyDown(Keys.D)) ? -1 : 0;

            int moveup = up + down;
            moveup = 0; //comment for testing
            int move = left + right;


            //Sprite effects
            //if (move > 0)
            //{
            //    isFlipped = false;
            //}
            //else if (move < 0)
            //{
            //    isFlipped = true;
            //}
            
            //turn based on the direction the weapon is facing
            if (equippedWeapon.isFlipped || mining.isFlipped)
            {
                isFlipped = false;
            }
            else
            {
                isFlipped = true;
            }
            //animations
            if (move != 0)
            {
                if (timer % 10 == 0 && timer != 0)
                {
                    //imageIndex = (imageIndex + 1 > 7) ? 0 : imageIndex + 1;
                    if(imageIndex == 7)
                    {
                        imageIndex = 0;
                    }
                    else
                    {
                        imageIndex++;
                    }
                }
                timer++;
            }
            else
            {
                imageIndex = defaultIndex;
                timer = 0;
            }

            srcRect.X = imageIndex * 72;

            //ground collisions
            Boolean onGround = false;
            for (int i = -3; i < 3; i++)
            {
                if (testLevel.getTile((int)position.Y + 7 + tvspd, (int)position.X + i).isCollidable())
                {
                    onGround = true;
                }
            }
            //vertical collisions
            Boolean colUp = false;
            for (int i = -3; i < 3; i++)
            {
                if (testLevel.getTile((int)position.Y - 5 + tvspd, (int)position.X + i).isCollidable())
                {
                    colUp = true;
                }
            }
            if (colUp == true)
            {
                vspd = 0;
            }

            if (onGround) // check if on the ground
            {
                vspd = 0;

                if (jump == 1 && colUp == false) //jump
                {
                    vspd = -jumpSpeed;
                }
            }
            else if (vspd < jumpSpeed) //apply gravity
            {
                vspd += grav;
            }

            tvspd = (int)vspd; //round vspd to prevent issues with the camera

            //right collisions
            Boolean colRight = false;

            if (move != 0 && testLevel.getTile((int)position.Y + 6, (int)position.X + 3).isCollidable() && !testLevel.getTile((int)position.Y + 5, (int)position.X + 3).isCollidable())
            {
                position -= new Vector2(0, 1);
                testLevel.incrementOffset(new Vector2(0, speed), cam);
            }
            else
            {
                for (int i = -5; i < 7; i++)
                {
                    if (testLevel.getTile((int)position.Y + i, (int)position.X + 3).isCollidable())
                    {
                        colRight = true;
                    }
                }
                if (move < 0 && colRight == true)
                {
                    move = 0;
                }
            }

            //left ccollisions
            Boolean colLeft = false;

            if (move != 0 && testLevel.getTile((int)position.Y + 6, (int)position.X - 4).isCollidable() && !testLevel.getTile((int)position.Y + 5, (int)position.X - 4).isCollidable())
            {
                position -= new Vector2(0, 1);
                testLevel.incrementOffset(new Vector2(0, speed), cam);
            }
            else
            { 
                for (int i = -5; i < 7; i++)
                {
                    if (testLevel.getTile((int)position.Y + i, (int)position.X - 4).isCollidable())
                    {
                        colLeft = true;
                    }
                }
                if (move > 0 && colLeft == true)
                {
                    move = 0;
                }
            }


            if (canMove)
            {
                position -= new Vector2(move, (float)(moveup - tvspd));

                float camMoveX = move * speed;
                float camMoveY = (float)((moveup - tvspd) * speed);

                int bgMoveX = move;
                int bgMoveY = -tvspd;

                Game1.incrementBackground(bgMoveX, bgMoveY);

                testLevel.incrementOffset(new Vector2(camMoveX, camMoveY), cam);
                testLevel.offSetEnemies((int)camMoveX, (int)camMoveY);

                //testLevel.setTile((int)position.Y, (int)position.X, new Tile(Tile.Type.Test)); //used for testing player movement
            }
            //equipping different weapons
            if (kb.IsKeyDown(Keys.D2))
            {
                equippedWeapon = weapon1;
            }
            if(kb.IsKeyDown(Keys.D3))
            {
                equippedWeapon = weapon2;
            }

        }

        //public void checkEdges()

        public void spawnPlayer(Level testLevel)
        {
            position.X += 1000;
            position.Y += 500;

            Game1.incrementBackground(1000, 500);
            testLevel.incrementOffset(new Vector2(-1000 * (float)movespeed, (float)((-500) * movespeed)), cam);
            canMove = true;
        }

        public Camera getCam()
        {
            return cam;
        }

        public Vector2 getPosition()
        {
            return position;
        }

        public Weapon getEquippedWeapon()
        {
            return equippedWeapon;
        }
        public void setEquippedWeapon(Weapon weapon)
        {
            equippedWeapon = weapon;
        }

        public double getHP()
        {
            return hp;
        }

        public bool getIsAlive()
        {
            return isAlive;
        }

        public void incrementHP()
        {
            hp -= 0.5;
            if (hp <= 0)
                isAlive = false;
        }

        public int getPoints()
        {
            return points;
        }
        public void incrementPoints()
        {
            points += 100;
        }

        public void draw(SpriteBatch s)
        {
            s.Draw(playerTex, hitbox, this.srcRect, Color.White, 0, new Vector2(0, 0), (isFlipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
    }
}
