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

        private const int SLIME_RANDOMNESS = 10;
        private static Random random;

        public BasicBlock(int column, int row):base(column, row)
        {
            model = GameplayScreen._models["BasicBlock"];
            base.isCollidable = true;
            //Scale(12f, 12f, 12f);

            //randomly show slime
            if(random == null)
                random = new Random(SLIME_RANDOMNESS); //seeding with const so levels always look the same

            if (random.Next(SLIME_RANDOMNESS) == 1)
            {
                Rotate(0f, 180f, 0f);
            }

            // Provides a hitbox for the block - Steven
            UpdateBoundingBox(model, Matrix.CreateTranslation(base.Position), false, false);
            HitboxHeight = 48;
            HitboxWidth = 48;
        }

        public override void Draw(RenderContext renderContext)
        {
            //Do nothing - don't call base
        }
    }
}
