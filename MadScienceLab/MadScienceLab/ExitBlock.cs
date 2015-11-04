using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MadScienceLab
{
    class ExitBlock:CellObject
    {
        public ExitBlock(int row, int column)
            : base(row, column, -GameConstants.SINGLE_CELL_SIZE) //calls overloaded cellobject constructor and places in background
        {
            base.Model = GameplayScreen._models["BackgroundBlock"];
            Scale(48f, 48f, 48f);
            Rotate(90f, 0f, 0f);
            this.Texture = GameplayScreen._textures["Exit"];
            isCollidable = true;
        }
    }
}
