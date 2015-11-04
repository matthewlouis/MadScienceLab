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
            base.Model = GameplayScreen._models["door"];
            base.isCollidable = true;

            this.isOpen = isOpen;
            UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position), false, false);

            HitboxWidthOffset = GameConstants.SINGLE_CELL_SIZE / 2;
        }

        //Makes door unCollidable when open
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

        //Opens or closes the door depending on current state
        public override void Toggle(RenderContext renderContext)
        {
            if (isOpen)
                isOpen = false;
            else
                isOpen = true;
        }
    }
}
