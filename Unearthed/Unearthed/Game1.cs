using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Unearthed
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D tileTexture; //stores the texture used by the tiles
        Texture2D bulletTexture; //stores the texture used by the bullets
        Texture2D minerTexture; //the miner's texture
        Texture2D background; //the background texture
        Texture2D backgroundLayer2; //the second background layer's texture
        Texture2D loading; //loading screen
        //spritesheet textures
        Texture2D pistol_Shoot_Tex;
        Texture2D pistol_Reload_Tex;
        Texture2D shotgun_Shoot_Tex;
        Texture2D shotgun_Reload_Tex;
        Texture2D pick_Tex;
        Texture2D ammoCounter_Tex;
        Texture2D hotbar_Tex;

        Texture2D enemy;

        Texture2D menu_Tex;

        Rectangle hotbarRec;
        Rectangle hotbarSourceRec;
        Rectangle hotbarDefaultSourceRec;
        Rectangle hotbarPickSourceRec;
        Rectangle hotbarShotgunSoruceRec;

        Rectangle ammoCounterRec;
        Rectangle ammoCounterSourceRec;
        Rectangle ammoCounterSourceRec1;
        Rectangle ammoCounterSourceRec2;
        Rectangle ammoCounterSourceRec3;
        Rectangle ammoCounterSourceRec4;



        SpriteFont test; //used to test variables or such that would need to be shown on screen in real time

        Level testLevel;  //level for testing
        static int width;        //screen width
        static int height;       //screen height
        int tileSize;     //size of an individual tile
        int players;      //number of players
        Camera[] cameras; //stores all the cameras
        Weapon pistol;//pistol weapon
        Weapon shotgun;//shotgun weapon
        MiningTool miningTool;

        Boolean gameStart;

        Player p1;

        public static int globalScaleFactor; //stores an integer that everything scales by. Used for size testing
        public static int cameraRatio;

        Vector2 levelDimensions; //dimensions of the level

        Boolean devMode; //if dev test mode is enabled

        KeyboardState oldKB; //used to check to see if a key has been released

        //background layers
        //layer 1
        static Rectangle bgRect;
        static Rectangle bgRectSide;
        static Rectangle bgRectVert;
        static Rectangle bgRectCorner;
        Vector2 bgPos;

        //layer 2
        static Rectangle bgRect2;
        static Rectangle bgRectSide2;
        static Rectangle bgRectVert2;
        static Rectangle bgRectCorner2;
        Vector2 bgPos2;

        static int bgCounter;
        static int bgCounter2;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 720;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //set width and height variables for later use
            width = GraphicsDevice.Viewport.Width;
            height = GraphicsDevice.Viewport.Height;

            tileSize = 5; //set default tile size - scalable; 10 is terraria ish

            devMode = true; //currently in dev test mode

            globalScaleFactor = 2;

            levelDimensions = new Vector2(width * Game1.globalScaleFactor, height * Game1.globalScaleFactor); //level is 2000 by 1440
            cameraRatio = 1;

            players = 1;
            cameras = new Camera[players]; //BUGTEST CAMERAS TO MAKE THEM MORE OPTIMIZED//
            cameras[0] = new Camera(new Rectangle(0, 0, width/tileSize/cameraRatio, height/tileSize/cameraRatio));//new Camera(new Rectangle(0, 0, ((int)levelDimensions.X / Game1.globalScaleFactor) / tileSize, ((int)levelDimensions.Y / Game1.globalScaleFactor) / tileSize));

            //background
            bgRect = new Rectangle(0, 0, width, height);
            bgRectSide = new Rectangle(-width, 0, width, height);
            bgRectVert = new Rectangle(0, -height, width, height);
            bgRectCorner = new Rectangle(-width, -height, width, height);
            bgPos = new Vector2(0, 0);

            bgRect2 = new Rectangle(0, 0, width, height);
            bgRectSide2 = new Rectangle(-width, 0, width, height);
            bgRectVert2 = new Rectangle(0, -height, width, height);
            bgRectCorner2 = new Rectangle(-width, -height, width, height);
            bgPos2 = new Vector2(0, 0);

            bgCounter = 0;
            bgCounter2 = 0;

            gameStart = false;

            hotbarDefaultSourceRec = new Rectangle(1024, 0, 512, 512);
            hotbarSourceRec = hotbarDefaultSourceRec;
            hotbarRec = new Rectangle((width / 2) - 128, (height) - 256, 256, 256);
            hotbarPickSourceRec = new Rectangle(512, 0, 512, 512);
            hotbarShotgunSoruceRec = new Rectangle(1536, 0, 512, 512);

            ammoCounterRec = new Rectangle(width - 192, height - 84, 128, 128);
            ammoCounterSourceRec1 = new Rectangle(0, 0, 192, 192);
            ammoCounterSourceRec2 = new Rectangle(192, 0, 192, 192);
            ammoCounterSourceRec3 = new Rectangle(384, 0, 192, 192);
            ammoCounterSourceRec4 = new Rectangle(576, 0, 192, 192);
            ammoCounterSourceRec = ammoCounterSourceRec1;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load in the content
            tileTexture = this.Content.Load<Texture2D>("blank_white");
            bulletTexture = this.Content.Load<Texture2D>("Bullet Temp");
            //pistol sprites
            pistol_Shoot_Tex = this.Content.Load<Texture2D>("Pistol Shoot");
            pistol_Reload_Tex = this.Content.Load<Texture2D>("Pistol Reload");
            //shotgun sprites
            shotgun_Shoot_Tex = this.Content.Load<Texture2D>("Shotgun");
            shotgun_Reload_Tex = this.Content.Load<Texture2D>("Shotgun Reload");
            //Pick sprites
            pick_Tex = this.Content.Load<Texture2D>("Pick");
            //UI sprites
            ammoCounter_Tex = this.Content.Load<Texture2D>("Ammo Counter");
            hotbar_Tex = this.Content.Load<Texture2D>("Hotbar");
            menu_Tex = this.Content.Load<Texture2D>("Unearthed Menu");

            enemy = Content.Load<Texture2D>("Enemy");

            test = this.Content.Load<SpriteFont>("Test");

            background = this.Content.Load<Texture2D>("caveBackground");
            backgroundLayer2 = this.Content.Load<Texture2D>("backgroundLayer2");
            minerTexture = this.Content.Load<Texture2D>("Player_Spritesheet");

            loading = this.Content.Load<Texture2D>("Loading");

            miningTool = new MiningTool(new Rectangle((width / 2), (height / 2), 64, 64), pick_Tex, tileTexture, new Rectangle((width / 2), (height / 2), 96, 32));
            pistol = new Weapon(new Rectangle(width / 2, (height / 2) + 4, 48, 48), 240, 1, 10, false, pistol_Shoot_Tex, 8, pistol_Reload_Tex, 25, 5, miningTool);//initializes a test weapon
            shotgun = new Weapon(new Rectangle(width / 2, (height / 2) + 4, 48, 48), 90, 5, 5, false, shotgun_Shoot_Tex, 14, shotgun_Reload_Tex, 25, 15, miningTool);

            p1 = new Player(minerTexture, width, height, 5, Player.playerClass.miner, pistol, shotgun, miningTool);

            testLevel = new Level((int)levelDimensions.Y, (int)levelDimensions.X, tileTexture, tileSize, p1); //initialize the level

            for (int i = 0; i < testLevel.enemies.Count; i++)
                testLevel.enemies[i].setTexture(enemy);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();
            // Allows the game to exit
            if (kb.IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here
            /*
            if (devMode)
            { 
                KeyboardState devModeTest = Keyboard.GetState();

                int devTestMovespeed = 4 * tileSize;

                int up = (devModeTest.IsKeyDown(Keys.W)) ? 1 : 0;
                int down = (devModeTest.IsKeyDown(Keys.S)) ? -1 : 0;

                int left = (devModeTest.IsKeyDown(Keys.A)) ? 1 : 0;
                int right = (devModeTest.IsKeyDown(Keys.D)) ? -1 : 0;

                int moveup = up + down;
                int move = left + right;

                testLevel.incrementOffset(new Vector2(move * devTestMovespeed, moveup * devTestMovespeed), cameras[0]);
            }*/
            if (kb.IsKeyDown(Keys.Enter) && oldKB.IsKeyUp(Keys.Enter))
            {
                gameStart = true;
            }
            if (kb.IsKeyDown(Keys.Back) && oldKB.IsKeyUp(Keys.Back))
            {
                gameStart = false;
            }
            p1.getInput(testLevel);
            p1.getEquippedWeapon().update();
            testLevel.update();
            miningTool.update(testLevel, p1.getPosition());

            //if statements for the hotbar
            if (kb.IsKeyDown(Keys.D1))
                hotbarSourceRec = hotbarPickSourceRec;
            if (kb.IsKeyDown(Keys.D2))
                hotbarSourceRec = hotbarDefaultSourceRec;
            if (kb.IsKeyDown(Keys.D3))
                hotbarSourceRec = hotbarShotgunSoruceRec;
            //if statements for the ammo counter
            if (p1.getEquippedWeapon().reloadWait == p1.getEquippedWeapon().reloadWaitReset)
                ammoCounterSourceRec = ammoCounterSourceRec1;
            if (p1.getEquippedWeapon().reloadWait < 180 && p1.getEquippedWeapon().reloadWait > 120)
                ammoCounterSourceRec = ammoCounterSourceRec2;
            if (p1.getEquippedWeapon().reloadWait < 121 && p1.getEquippedWeapon().reloadWait > 60)
                ammoCounterSourceRec = ammoCounterSourceRec3;
            if (p1.getEquippedWeapon().reloadWait < 61)
                ammoCounterSourceRec = ammoCounterSourceRec4;

            for (int r = testLevel.enemies.Count - 1; r >= 0; r--)
            {
                testLevel.enemies[r].move(testLevel.p1);
                if(p1.getHP() < 1)
                {
                    gameStart = false;
                    Initialize();
                    break;
                }
                for (int c = p1.getEquippedWeapon().bullets.Count - 1; c >= 0; c--)
                {
                    testLevel.enemies[r].bulletCollision(p1.getEquippedWeapon().bullets[c]);
                    if (!testLevel.enemies[r].checkIsAlive())
                    {
                        testLevel.enemies.RemoveAt(r);
                        p1.getEquippedWeapon().bullets.RemoveAt(c);
                        p1.incrementPoints();
                        break;
                    }
                }
            }

            if (testLevel.enemies.Count == 0)
            {
                gameStart = false;
                Initialize();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            if(gameStart == false)
            {
                spriteBatch.Draw(menu_Tex, new Rectangle(0, 0, width, height),Color.White);
            }
            if (gameStart)
            {
                //background layer 2
                spriteBatch.Draw(backgroundLayer2, bgRect2, Color.White);
                spriteBatch.Draw(backgroundLayer2, bgRectSide2, Color.White);
                spriteBatch.Draw(backgroundLayer2, bgRectVert2, Color.White);
                spriteBatch.Draw(backgroundLayer2, bgRectCorner2, Color.White);

                //background layer 1
                spriteBatch.Draw(background, bgRect, Color.White);
                spriteBatch.Draw(background, bgRectSide, Color.White);
                spriteBatch.Draw(background, bgRectVert, Color.White);
                spriteBatch.Draw(background, bgRectCorner, Color.White);

                testLevel.Draw(spriteBatch, p1.getCam());

                if (devMode) //draw screen borders and 32x32 center during dev mode
                {
                    //spriteBatch.Draw(tileTexture, new Rectangle(width / 2 - 16, height / 2 - 16, 32, 32), Color.White);
                    /*
                    spriteBatch.Draw(tileTexture, new Rectangle((int)(-cameras[0].getCameraPosition().X / (levelDimensions.X / width)), (int)(-cameras[0].getCameraPosition().Y / (levelDimensions.Y / height)), tileSize, tileSize), Color.White);

                    spriteBatch.Draw(tileTexture, new Rectangle((int)(-cameras[0].getCameraPosition().X / (levelDimensions.X / width)) + (width / tileSize), (int)(-cameras[0].getCameraPosition().Y / (levelDimensions.Y / height)), tileSize, tileSize), Color.White);

                    spriteBatch.Draw(tileTexture, new Rectangle((int)(-cameras[0].getCameraPosition().X / (levelDimensions.X / width)), (int)(-cameras[0].getCameraPosition().Y / (levelDimensions.Y / height)) + (height / tileSize), tileSize, tileSize), Color.White);

                    spriteBatch.Draw(tileTexture, new Rectangle((int)(-cameras[0].getCameraPosition().X / (levelDimensions.X / width)) + (width / tileSize), (int)(-cameras[0].getCameraPosition().Y / (levelDimensions.Y / height)) + (height / tileSize), tileSize, tileSize), Color.White);
                    */
                }
                p1.draw(spriteBatch);

                //spriteBatch.Draw(testWeapon.usedTexture, testWeapon.location, new Rectangle(0, 0, 32, 32), Color.White, testWeapon.gunRotation, new Vector2(0, 16), SpriteEffects.None, 0);
                if(miningTool.equipped)
                {
                    miningTool.Draw(spriteBatch);
                }
                else
                {
                    p1.getEquippedWeapon().Draw(spriteBatch);
                }

                //UI 
                spriteBatch.Draw(hotbar_Tex, hotbarRec, hotbarSourceRec, Color.White);
                spriteBatch.Draw(ammoCounter_Tex, ammoCounterRec, ammoCounterSourceRec, Color.White);

                String tempString;
                if (p1.getEquippedWeapon().currentlyReloading == true)
                    tempString = "Reloading";
                else
                    tempString = p1.getEquippedWeapon().maxAmmo.ToString() + " / " + p1.getEquippedWeapon().maxAmmoReload.ToString();
                spriteBatch.DrawString(test, tempString, new Vector2(width - 172, height - 68), Color.White);


                foreach (Enemy e in testLevel.enemies)
                {
                    e.Draw(spriteBatch);
                }

                foreach (Bullet b in p1.getEquippedWeapon().bullets)
                {
                    spriteBatch.Draw(bulletTexture, new Rectangle((int)b.position.X, (int)b.position.Y, 8, 8), new Rectangle(0, 0, 8, 8), Color.White, b.rotation, new Vector2(4, 4), SpriteEffects.None, 0);
                }

                spriteBatch.DrawString(test, "Health : " + (int)p1.getHP(), new Vector2(800, 50), Color.White);
                spriteBatch.DrawString(test, "Enemies Left : " + testLevel.enemies.Count(), new Vector2(50, 50), Color.White);
                spriteBatch.DrawString(test, "Score : " + p1.getPoints(), new Vector2(400, 50), Color.White);

                //loading screen
                if (!testLevel.isFinishedGenerating())
                {
                    spriteBatch.Draw(loading, new Rectangle(0, 0, width, height), Color.White);
                }


            }
            spriteBatch.End();

            base.Draw(gameTime);
        }



        //BACKGROUND
        public static void incrementBackground(int xAmt, int yAmt)
        {
            //front background layer
            xAmt = (xAmt == 0) ? 0 : (xAmt > 0) ? 1 : -1;
            yAmt = (yAmt == 0) ? 0 : (yAmt > 0) ? 2 : -2;

            if (xAmt != 0 || yAmt != 0)
            {
                bgCounter+=10;
            }

            if (bgCounter % 8 > 0)
            {
                bgRect.X += xAmt;
                bgRect.Y += yAmt;

                bgRectSide.X += xAmt;
                bgRectSide.Y += yAmt;

                bgRectVert.X += xAmt;
                bgRectVert.Y += yAmt;

                bgRectCorner.X += xAmt;
                bgRectCorner.Y += yAmt;

                //main rect
                if (bgRect.X < -width)
                {
                    bgRect.X = width + xAmt;
                }
                else if (bgRect.X > width)
                {
                    bgRect.X = -width + xAmt;
                }
                if (bgRect.Y < -height)
                {
                    bgRect.Y = height + yAmt;
                }
                else if (bgRect.Y > height)
                {
                    bgRect.Y = -height + yAmt;
                }

                //side rect
                if (bgRectSide.X < -width)
                {
                    bgRectSide.X = width + xAmt;
                }
                else if (bgRectSide.X > width)
                {
                    bgRectSide.X = -width + xAmt;
                }
                if (bgRectSide.Y < -height)
                {
                    bgRectSide.Y = height + yAmt;
                }
                else if (bgRectSide.Y > height)
                {
                    bgRectSide.Y = -height + yAmt;
                }

                //vertical rect
                if (bgRectVert.X < -width)
                {
                    bgRectVert.X = width + xAmt;
                }
                else if (bgRectVert.X > width)
                {
                    bgRectVert.X = -width + xAmt;
                }
                if (bgRectVert.Y < -height)
                {
                    bgRectVert.Y = height + yAmt;
                }
                else if (bgRectVert.Y > height)
                {
                    bgRectVert.Y = -height + yAmt;
                }

                //corner rect
                if (bgRectCorner.X < -width)
                {
                    bgRectCorner.X = width + xAmt;
                }
                else if (bgRectCorner.X > width)
                {
                    bgRectCorner.X = -width + xAmt;
                }
                if (bgRectCorner.Y < -height)
                {
                    bgRectCorner.Y = height + yAmt;
                }
                else if (bgRectCorner.Y > height)
                {
                    bgRectCorner.Y = -height + yAmt;
                }
            }
            //back background layer
            xAmt = (xAmt == 0) ? 0 : (xAmt > 0) ? 1 : -1;
            yAmt = (yAmt == 0) ? 0 : (yAmt > 0) ? 2 : -2;

            if(xAmt != 0 || yAmt != 0)
            {
                bgCounter2++;
            }

            if (bgCounter2 % 2 == 0)
            {
                bgRect2.X += xAmt;
                bgRect2.Y += yAmt;

                bgRectSide2.X += xAmt;
                bgRectSide2.Y += yAmt;

                bgRectVert2.X += xAmt;
                bgRectVert2.Y += yAmt;

                bgRectCorner2.X += xAmt;
                bgRectCorner2.Y += yAmt;

                //main rect
                if (bgRect2.X < -width)
                {
                    bgRect2.X = width + xAmt;
                }
                else if (bgRect2.X > width)
                {
                    bgRect2.X = -width + xAmt;
                }
                if (bgRect2.Y < -height)
                {
                    bgRect2.Y = height + yAmt;
                }
                else if (bgRect2.Y > height)
                {
                    bgRect2.Y = -height + yAmt;
                }

                //side rect
                if (bgRectSide2.X < -width)
                {
                    bgRectSide2.X = width + xAmt;
                }
                else if (bgRectSide2.X > width)
                {
                    bgRectSide2.X = -width + xAmt;
                }
                if (bgRectSide2.Y < -height)
                {
                    bgRectSide2.Y = height + yAmt;
                }
                else if (bgRectSide2.Y > height)
                {
                    bgRectSide2.Y = -height + yAmt;
                }

                //vertical rect
                if (bgRectVert2.X < -width)
                {
                    bgRectVert2.X = width + xAmt;
                }
                else if (bgRectVert2.X > width)
                {
                    bgRectVert2.X = -width + xAmt;
                }
                if (bgRectVert2.Y < -height)
                {
                    bgRectVert2.Y = height + yAmt;
                }
                else if (bgRectVert2.Y > height)
                {
                    bgRectVert2.Y = -height + yAmt;
                }

                //corner rect
                if (bgRectCorner2.X < -width)
                {
                    bgRectCorner2.X = width + xAmt;
                }
                else if (bgRectCorner2.X > width)
                {
                    bgRectCorner2.X = -width + xAmt;
                }
                if (bgRectCorner2.Y < -height)
                {
                    bgRectCorner2.Y = height + yAmt;
                }
                else if (bgRectCorner2.Y > height)
                {
                    bgRectCorner2.Y = -height + yAmt;
                }
            }
        }
    }
}
