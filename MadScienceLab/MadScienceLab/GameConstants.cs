using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    public static class GameConstants
    {
        // resolution 
        public const int X_RESOLUTION = 1280;
        public const int Y_RESOLUTION = 720;
        
        // world cell size
        public const int SINGLE_CELL_SIZE = 48;
        public const float MIN_Z = 0.1f;
        public const float MAX_Z = 5000;

        // player
        public const float MOVEAMOUNT = 2f;
        public const int PLAYER_DAMAGE = 1;

        // projectile 
        public enum DIRECTION { pointLeft = -1, pointRight = 1 }
        public const int PROJECTILE_X_VELOCITY = 10;

        // physics
    }
}
