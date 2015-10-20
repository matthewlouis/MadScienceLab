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
        public bool IsLiftable
        {
            //requires that nothing (eg. boxes) are on top of it
            //as a simplification, not necessarily the arc to pick up the box would be the required area, but just a box worth of area above the box needs to not have any non-wall            objects (ie. BasicBlocks） (which would be objects that would exert a gravitational and frictional force on the box)
            get
            {
                Rectangle areaAbove = new Rectangle((int)Position.X, (int)Position.Y + Hitbox.Height, (int)Hitbox.Width, (int)Hitbox.Height);
                bool pickuppable = true;
                foreach (CellObject levelObject in Game1.CurrentLevel.Children) //check to see if it has collision with anything
                {
                    if (levelObject.isCollidable && levelObject.GetType() != typeof(BasicBlock) && areaAbove.Intersects(levelObject.Hitbox))
                    {
                        pickuppable = false;
                    }
                }
                return pickuppable;
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
