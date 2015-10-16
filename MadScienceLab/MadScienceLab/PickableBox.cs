using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    public class PickableBox : CellObject
    {
        public bool isCollidable { 
            get {
                return base.isCollidable;
            }
            set
            {
                base.isCollidable = value;
            }
        }
        public PickableBox(int column, int row)
            : base(column, row)
        {
            base.Model = Game1._models["MoveableBox"];
            base.isCollidable = true;

            // Provides a hitbox for the block - Steven
            UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position), false, false);
        }
    }
}
