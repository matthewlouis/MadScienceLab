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
        private static int DOOR_SPEED = 3;
        private static int DOOR_CLOSED_ZPOSITION = -GameConstants.SINGLE_CELL_SIZE + 2;

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
            if (isOpen)
            {
                //Animate opening door if not done yet.
                float nextPos = Position.Z - DOOR_SPEED;
                if (nextPos > DOOR_CLOSED_ZPOSITION)
                    this.Position = new Vector3(Position.X, Position.Y, nextPos);
                else
                    this.Position = new Vector3(Position.X, Position.Y, DOOR_CLOSED_ZPOSITION);
                isCollidable = false;
            }else
            {
                //Animate closing door if not done yet.
                float nextPos = Position.Z + DOOR_SPEED;
                if (nextPos < 0)
                    this.Position = new Vector3(Position.X, Position.Y, nextPos);
                else
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
