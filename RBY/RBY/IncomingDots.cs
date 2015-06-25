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

namespace RBY
{
    struct IncomingDots
    {
        public enum possibleColors { red=1, blue, yellow, purple, green, orange }
        public possibleColors color;
        public float velocity;
        public Vector2 direction;
        public Vector2 spawnPosition;

        public int hitRadius;

        public int spawnCoordX;
        public int spawnCoordY;

        public bool hasBeenPlaced;
        public bool isAlive;

        public void Update(float delta)
        {
            direction.Normalize();
            spawnPosition += direction * velocity * delta;
        }

    }
}
