using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    class LaserProjectile:CellObject
    {
        
        public LaserProjectile(int column, int row):base(column, row)
        {
            // hitbox for collision
            UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position), false, false);
        }

        public override void Update(RenderContext renderContext)
        {
            base.Update(renderContext);
        }
        
    }
}
