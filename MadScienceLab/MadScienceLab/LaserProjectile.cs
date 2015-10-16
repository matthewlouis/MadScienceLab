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
            BoundingBox box = UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position));
            Vector3 size = box.Max - box.Min;
            base.Hitbox = new Rectangle((int)Position.X, (int)Position.Y, (int)size.X, (int)size.Y);

        }

        public override void Update(RenderContext renderContext)
        {
            

            base.Update(renderContext);
        }
        
    }
}
