using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    /**
     * This is the basic block object that will make up floors, walls etc of our level.
     */
    class BasicBlock:CellObject
    {
        public BasicBlock(int column, int row):base(column, row)
        {
            base.Model = Game1._models["BasicBlock"];
            base.isCollidable = true;

            // Provides a hitbox for the block - Steven
            UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position), false, false);
        }
    }
}
