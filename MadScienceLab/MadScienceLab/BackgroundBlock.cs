using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    class BackgroundBlock:CellObject
    {
        public BackgroundBlock(int row, int column, Texture2D texture)
            : base(row, column, -GameConstants.SINGLE_CELL_SIZE) //calls overloaded cellobject constructor and places in background
        {
            base.Model = Game1._models["block"];
            this.Texture = texture;
        }
    }
}
