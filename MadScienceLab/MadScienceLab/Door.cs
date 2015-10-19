using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    class Door:SwitchableObject
    {
        private Boolean isOpen;
        public Door(int column, int row, Boolean isOpen):base(column, row)
        {
            base.Model = Game1._models["door"];
            base.isCollidable = true;
            base.Rotate(0f, 90f, 0f);

            this.isOpen = isOpen;
            UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position), true, true);
        }

        public override void Update(RenderContext renderContext)
        {
            
            if(isOpen){
                this.Position = new Vector3(Position.X, Position.Y, -GameConstants.SINGLE_CELL_SIZE + 2);
                isCollidable = false;
            }else
            {
                this.Position = new Vector3(Position.X, Position.Y, 0);
                isCollidable = true;
            }
            base.Update(renderContext);
        }

        public override void Toggle(RenderContext renderContext)
        {
            if (isOpen)
                isOpen = false;
            else
                isOpen = true;
        }
    }
}
