using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    class Door:CellObject
    {
        private Boolean isOpen;
        public Door(int column, int row, Boolean isOpen):base(column, row)
        {
            base.Model = Game1._models["door"];
            base.isCollidable = true;
            base.Rotate(0f, 90f, 0f);

            this.isOpen = isOpen;

            // Provides a hitbox for the block - Steven
            BoundingBox box = UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position));
            Vector3 size = box.Max - box.Min;
            base.Hitbox = new Rectangle((int)Position.X, (int)Position.Y, (int)size.X, (int)size.Y);
        }

        public override void Update(RenderContext renderContext)
        {
            if(isOpen){
                this.Position = new Vector3(Position.X, Position.Y, -Game1.SINGLE_CELL_SIZE + 2);
                isCollidable = false;
            }else
            {
                this.Position = new Vector3(Position.X, Position.Y, 0);
                isCollidable = true;
            }
            base.Update(renderContext);
        }

        public override void Draw(RenderContext _renderContext)
        {
            //Jacob: These lines don't seem to be used anymore.
            //var transforms = new Matrix[model.Bones.Count];
            //model.CopyAbsoluteBoneTransformsTo(transforms);

            base.Draw(_renderContext);
        }

        public void Toggle()
        {
            if (isOpen)
                isOpen = false;
            else
                isOpen = true;
        }
    }
}
