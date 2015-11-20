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
    public class BasicBlock:CellObject
    {
        private Model model;

        public BasicBlock(int column, int row):base(column, row)
        {
            model = GameplayScreen._models["BasicBlock"];
            base.isCollidable = true;

            // Provides a hitbox for the block - Steven
            UpdateBoundingBox(model, Matrix.CreateTranslation(base.Position), false, false);
        }

        public override void Draw(RenderContext renderContext)
        {
            //Do nothing - don't call base
        }
    }
}
