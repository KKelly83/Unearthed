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
    class Enemy
    {
        Texture2D enemyTex;
        public Rectangle hitBox;
        Random random;
        Level level;
        Tunnel tunnel;

        public const int Width = 30;
        public const int Height = 30;

        int hp;
        bool isAlive;
        
        bool isFlipped;

        int horizontalVelocity;

        public Enemy(Tunnel tunnel, Level level)
        {
            this.level = level;
            this.tunnel = tunnel;
            random = new Random();
            hitBox = new Rectangle(tunnel.bounds.X, tunnel.bounds.Center.Y, Width, Height);
            hp = 1;
            isAlive = true;
            horizontalVelocity = (random.Next(2) == 0) ? 1 : -1;
            isFlipped = (horizontalVelocity > 0) ? false : true;
            
        }

        public void setTexture(Texture2D tex)
        {
            enemyTex = tex;
        }

        public void bulletCollision(Bullet bullet)
        {
            if (Math.Abs(bullet.position.X - hitBox.Center.X) < Width / 2 && Math.Abs(bullet.position.Y - hitBox.Center.Y) < Height / 2)
            {
                hp--;
                isAlive = false;
            }
        }
        public bool checkIsAlive()
        {
            if(!isAlive)
            {
                enemyTex = null;
                hitBox = new Rectangle();
                return false;
            }
            return true;
        }
        
        public void move(Player player)
        {
            if (hitBox.Center.X == 500 && hitBox.Center.Y == 360)
                player.incrementHP();

            if (500 != hitBox.Center.X)
                hitBox.X += (500 - hitBox.Center.X < 0) ? -1 : 1;
            if (360 != hitBox.Center.Y)
                hitBox.Y += (360 - hitBox.Y < 0) ? -1 : 1;

            
        }

        public void Draw(SpriteBatch s)
        {
            s.Draw(enemyTex, hitBox, null, Color.White, 0, new Vector2(0, 0), (isFlipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
        
    }
}
