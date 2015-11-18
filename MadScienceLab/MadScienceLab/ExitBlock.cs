using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    class ExitBlock:CellObject
    {
        public ExitBlock(int row, int column)
            : base(row, column) //calls overloaded cellobject constructor and places in background
        {
            base.Model = GameplayScreen._models["ExitBlock"];
            Scale(48f, 48f, 48f);
            Position = new Vector3(Position.X, Position.Y - 2, Position.Z - 27);
            HitboxHeightOffset = 2;
            HitboxHeight = GameConstants.SINGLE_CELL_SIZE;
            HitboxWidth = GameConstants.SINGLE_CELL_SIZE;
            isCollidable = true;
        }
    }
}
