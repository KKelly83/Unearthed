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
    class Camera
    {
        Rectangle cameraPosition; //stores the position of the Camera

        public Camera(Rectangle cameraRect)
        {
            cameraPosition = cameraRect;
        }

        //returns the position of the camera
        public Rectangle getCameraPosition()
        {
            return cameraPosition;
        }

        //increment the camera's x
        public void incrementCameraX(int offset)
        {
            cameraPosition.X += offset;
        }
        //increment the camera's y
        public void incrementCameraY(int offset)
        {
            cameraPosition.Y += offset;
        }
    }
}
