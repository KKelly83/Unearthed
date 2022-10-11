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
    class Bullet
    {
        public float rotation; //the direction the bullet is facing (used for determining where it will shoot);
        public Vector2 position; //sets the position of the bullet, and used to move it;
        public Bullet(float r, Vector2 p)
        {
            rotation = r;
            position = p;
        }
        public void moveBullet(float speed) //used to move the bullet based on the position of the mouse
        {
            Vector2 velocity = new Vector2((float)Math.Cos(rotation),(float)Math.Sin(rotation)) * speed; //creates a new Vector2 that uses the rotation of the bullet to create a velocity
            position = position + velocity; //moves the bullet with the velocity made from the direction and the speed
        }
        public void setRotation(float f) //used to set/change the roation of the bullet
        {
            rotation = MathHelper.ToRadians(f);
        }
    }
}
