﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    class Switch:CellObject
    {
        List<Door> LinkedDoors { get; set; }
        public Boolean Toggleable { get; private set; }
        public Boolean IsSwitched { get; set; }
        private Boolean doorsToggled = false;

        public Switch(int column, int row, Boolean toggleable):base(column, row)
        {
            base.Model = Game1._models["switch"];
            base.isCollidable = true;
            Toggleable = toggleable;
            Rotate(0f, 90f, 0);
            Translate(Position.X, Position.Y - GameConstants.SINGLE_CELL_SIZE / 2 + 1, Position.Z); //Matt: this is for offsetting the model position so it's flat on the floor

            // Provides a hitbox for the block - Steven
            BoundingBox box = UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position));
            Vector3 size = box.Max - box.Min;
            base.Hitbox = new Rectangle((int)Position.X, (int)Position.Y, (int)size.X, (int)size.Y);
        }

        public void FlickSwitch()
        {
            if (IsSwitched && Toggleable)
            {
                Console.Out.WriteLine("Flicked Off");
                IsSwitched = false;
            }
            else
            {
                Console.Out.WriteLine("Flicked On");
                IsSwitched = true;
            }
        }



        public override void Update(RenderContext renderContext)
        {
            if (IsSwitched && doorsToggled == false)
            {
                doorsToggled = true;
                foreach (Door door in LinkedDoors)
                {
                    door.Toggle();
                }
            }
            else if (!IsSwitched && doorsToggled == true)
            {
                doorsToggled = false;
                foreach (Door door in LinkedDoors)
                {
                    door.Toggle();
                }
            }
            base.Update(renderContext);
        }
    }
}
