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
    class Player
    {
        public enum Colors {red=1, blue, yellow};
        public Colors prevColor;
        public Colors currentColor = Colors.red;
        public bool isAlive = true;
        public int hitRadius;

        
        public void switchColorsForwards(){

            prevColor = currentColor;

            switch (currentColor)
            {
                case Colors.red:
                    currentColor = Colors.blue;
                    break;
                case Colors.blue:
                    currentColor = Colors.yellow;
                    break;
                case Colors.yellow:
                    currentColor = Colors.red;
                    break;
                default:
                    Console.WriteLine("OOPS");
                    break;
            }

        }

        public void switchColorsBackwards()
        {

            prevColor = currentColor;

            switch (currentColor)
            {
                case Colors.red:
                    currentColor = Colors.yellow;
                    break;
                case Colors.blue:
                    currentColor = Colors.red;
                    break;
                case Colors.yellow:
                    currentColor = Colors.blue;
                    break;
                default:
                    Console.WriteLine("OOPS");
                    break;
            }

        }

    }



}
