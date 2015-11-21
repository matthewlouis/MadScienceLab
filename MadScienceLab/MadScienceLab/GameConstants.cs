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
        public const int HEALTH = 3;

        // projectile 
        public enum POINTDIR { pointLeft = -1, pointRight = 1 } //Jacob: Renamed this to POINTDIR, as I decided to use DIRECTION for all 4 directions
        public const int PROJECTILE_X_VELOCITY = 10;

        // platform/enemy
        public enum DIRECTION { Left = 1, Down = 2, Up = 3, Right = 4 }

        // physics

        // message events
        public enum TYPING_STATE { NotTyped = 1, Typing = 2, DoneTyping = 3 }

        // sound
        public const int DISTANCE_SCALE = 4;

        // levels
        public const int LEVELS = 6;
        public static string[] LEVELNAMES = { "Storage Room", "Security Front", "Lab Hallway", "Top-Secret Exit", "Elevator to B1", "Exit Shaft" };

        /// <summary>
        /// used for scoring the player at the end of a level
        /// </summary>
        public enum SCORE { fullHealth = 50, medHealth = 30, lowHealth = 10, parTimeComplete = 50, levelComplete = 100 }      
    }
}
