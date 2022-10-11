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
    class Weapon
    {
        public Rectangle location; //stores the location of the weapon
        public int rateOfFire; //stores the RPM of the weapon
        public int bulletsPerShot; //stores the amount of bullets that are shot per "round" (shotguns would be >1)
        public int maxAmmo; //stores the amount of ammo that can be shot before reloading;
        public int maxAmmoReload; //used to keep the max amount of ammo so the weapons can be reloaded
        public Boolean isFullAuto; //will determine wetheer or not the user will have to click individually to fire
        public float gunRotation; //the direction the gun "end" is facing
        double cooldown; //uses the RPM to determine how much it should shoot (RPM/60 == bullets per second)
        double cooldownReset; //used to reset cooldown
        Random rand; //used to create bullet spread
        
        public Texture2D usedTexture; //used to hold the texture that the gun is currently using

        public Vector2 usedRotationVector; //vector2 to hold the currently used rotation point
        Vector2 mousePosition; //gets the postion of the mouse
        Vector2 rightVector;
        Vector2 leftVector;
        Boolean needToReload;
        public Boolean currentlyReloading;

        public Boolean isFlipped; //if the texture needs to be flipped(if the weapon is "facing" left)

        public int reloadWait;
        public int reloadWaitReset;

        public Rectangle usedSourceRec;
        Rectangle defaultSourceRec;

        public Rectangle[] shootSourceArr;

        public Rectangle[] reloadSourceArr;

        public Texture2D shootingTexture;
        public Texture2D reloadTexture;

        public List<Bullet> bullets = new List<Bullet>(); //a list for the bullets


        public float spreadTemp;

        KeyboardState oldKB; //used to check to see individual key presses
        MouseState oldMouse; //used to check for individual clicks w/ mouse

        Boolean recentlyShot;
        Boolean recentlyReloaded;

        int shotFrameCooldown;
        int shotFrameCounter;

        int reloadFrameCooldown;
        int reloadFrameCounter;

        int spreadRange;

        public MiningTool mining;

        public Weapon(Rectangle l, int rOF, int bPS, int mA, Boolean iFA, Texture2D shootT, int sLength, Texture2D reloadT, int rLength, int sp, MiningTool iM)
        {
            shootSourceArr = new Rectangle[sLength];
            reloadSourceArr = new Rectangle[rLength];
            defaultSourceRec = new Rectangle(0, 0, 64, 64);
            usedSourceRec = defaultSourceRec;
            //loading textures into sheet
            shootingTexture = shootT;
            reloadTexture = reloadT;
            loadReloadTextures();
            loadShootingTextures();
            //constructor
            mining = iM;
            location = l;
            rateOfFire = rOF;
            bulletsPerShot = bPS;
            maxAmmo = mA;
            maxAmmoReload = mA;
            isFullAuto = iFA;
            usedTexture = shootingTexture; //default is the right facing texture and shooting spriteSheet;
            spreadRange = sp;

            //non constructor
            cooldown = 60 / (rateOfFire / 60);
            cooldownReset = cooldown;
            rand = new Random();
            currentlyReloading = false;
            needToReload = false;
            reloadWait = 180;
            reloadWaitReset = reloadWait;

            recentlyShot = false;
            shotFrameCooldown = 0;
            shotFrameCounter = 0;

            recentlyReloaded = false;
            reloadFrameCooldown = 0;
            reloadFrameCounter = 0;
        }

        public void update()
        {
            KeyboardState kb = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            mousePosition = new Vector2(mouse.X, mouse.Y);

            usedTexture = shootingTexture;

            gunRotation = (float)Math.Atan2((mouse.Y - location.Y), (mouse.X - location.X)); //uses trigonometry to get the angle between the mouse and the gun
            if (!mining.equipped)
            {
                if (currentlyReloading == false)
                {
                    if (isFullAuto) //checks to see if the gun is full auto or not
                    {
                        if (cooldown <= 0 && mouse.LeftButton == ButtonState.Pressed && needToReload == false)
                        {
                            int tempMultiShot = bulletsPerShot; //the amount of bullets per shot (possible shotgun)
                            if (tempMultiShot > 1) //if the amount of bullets per shot is more than 1
                            {
                                while (tempMultiShot > 1)
                                {
                                    tempMultiShot--;
                                    firedGun();

                                }
                                recentlyShot = true;
                            }
                            else //if the amount of bullets per shot is 1
                            {
                                firedGun();
                                recentlyShot = true;
                            }
                            maxAmmo--;
                            cooldown = cooldownReset;
                        }
                        if (cooldown <= cooldownReset) //if the weapon cooldownperiod is less than or equal to the default value that it can be
                        {
                            cooldown--;
                        }
                    }
                    else //if the gun is semi-auto
                    {
                        if (cooldown <= 0 && mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released && needToReload == false)
                        {
                            int tempMultiShot = bulletsPerShot; //temp variable for the bullets per shot
                            if (tempMultiShot > 1) //if its a "shotgun"
                            {
                                while (tempMultiShot > 1)
                                {
                                    tempMultiShot--;
                                    firedGun();
                                }
                                recentlyShot = true;
                            }
                            else //if it is not a shotgun
                            {
                                firedGun();
                                recentlyShot = true;
                            }
                            maxAmmo--;
                            cooldown = cooldownReset;
                        }
                        if (cooldown <= cooldownReset)
                        {
                            cooldown--;
                        }

                    }
                }
                //end of shooting code

                if (maxAmmo < 1) //if the weapon is out of ammo
                {
                    needToReload = true;
                }

                if (kb.IsKeyDown(Keys.R) && currentlyReloading == false)//code that reloads the gun (press r)
                {
                    currentlyReloading = true;
                    recentlyReloaded = true;
                }

                if (currentlyReloading == true) //if the gun is currently "reloading"
                {
                    usedTexture = reloadTexture;
                    reloadWait--;
                    if (reloadWait < 1) //if the timer is up, it sets the ammo in the gun to the max ammo, and then it is not reloading
                    {
                        maxAmmo = maxAmmoReload;
                        currentlyReloading = false;
                        needToReload = false;
                        reloadWait = reloadWaitReset;
                        usedTexture = shootingTexture;
                        //usedSourceRec = shootSourceArr[0];
                    }
                }

                foreach (Bullet b in bullets) //uses the moveBullet method in the bullet class to move the bullets a static speed ( a variable to adjust bullet speed can be used based on the weapon at hand)
                {
                    b.moveBullet(20);
                }
                for (int x = 0; x < bullets.Count; x++)
                {
                    if (Vector2.Distance(bullets[x].position, new Vector2(location.X, location.Y)) > 800) //if the bullet is 800 pixels away from the gun's position, it is removed
                        bullets.Remove(bullets[x]);
                }

                if (MathHelper.ToDegrees(gunRotation) > 90 || MathHelper.ToDegrees(gunRotation) < -90) //checks to see if the degree made by the mouse is on the left side of the screen
                {
                    isFlipped = true;
                }
                else
                {
                    isFlipped = false;
                }

                if (recentlyShot) //if the weapon has recently shot, play the animation
                    animateShoot();
                if (recentlyReloaded)
                    animateReload();

                oldKB = kb;
                oldMouse = mouse;
            }
        }
        public void Draw(SpriteBatch sb)
        {
            //(isFlipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None
            sb.Draw(usedTexture, location, usedSourceRec, Color.White, gunRotation, new Vector2(0,32), (isFlipped) ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);
        }

        //Assorted Methods

        public void firedGun()//a method that adds a bullet to the list and 
        {
            usedTexture = shootingTexture;
            float spread = MathHelper.ToRadians(rand.Next(-Math.Abs(spreadRange), spreadRange)); //the spread of the bullet between the min/max value
            spreadTemp = spread;

            float tempRotation = gunRotation;

            bullets.Add(new Bullet(tempRotation + spread, new Vector2(location.X, location.Y))); //adds a new bullet to the bullet list

        }
        public void loadReloadTextures() //loads the reload sprites into the array
        {
            for (int x = 0; x < reloadSourceArr.Length; x++)
            {
                reloadSourceArr[x] = new Rectangle(0 + (x * 64), 0, 64, 64);
            }

        }
        public void loadShootingTextures() //loads the shooting sprites into the array.
        {
            for (int x = 0; x < shootSourceArr.Length; x++)
            {
                shootSourceArr[x] = new Rectangle(0 + (x * 64), 0, 64, 64);
            }
        }
        public void animateShoot()
        {
            usedTexture = shootingTexture;
            if (shotFrameCounter < shootSourceArr.Length && shotFrameCooldown % 2 == 0) //iterating through array
            {
                usedSourceRec = new Rectangle(0 + (shotFrameCounter * 64), 0, 64, 64);
                shotFrameCounter++;
            }
            shotFrameCooldown++;
            if (shotFrameCounter == shootSourceArr.Length) //if x has reached the end of the array, reset to defaults
            {
                recentlyShot = false;
                usedSourceRec = defaultSourceRec;
                shotFrameCooldown = 0;
                shotFrameCounter = 0;
            }
        }
        public void animateReload()
        {
            usedTexture = reloadTexture;
            if (reloadFrameCounter < reloadSourceArr.Length && reloadFrameCooldown % 3 == 0) //iterating through array
            {
                usedSourceRec = new Rectangle(0 + (reloadFrameCounter * 64), 0, 64, 64);
                reloadFrameCounter++;
            }
            reloadFrameCooldown++;
            if (reloadFrameCounter == reloadSourceArr.Length) //if x has reached the end of the array, reset to defaults
            {
                recentlyReloaded = false;
                usedSourceRec = defaultSourceRec;
                reloadFrameCooldown = 0;
                reloadFrameCounter = 0;
            }


        }

    }
}
