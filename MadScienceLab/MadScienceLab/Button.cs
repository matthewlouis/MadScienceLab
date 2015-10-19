using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    class Button:CellObject
    {
        public List<Door> LinkedDoors { get; set; }
        public Boolean IsPressed { get; set; }
        private Boolean doorsToggled = false;

        public Button(int column, int row):base(column, row)
        {
            LinkedDoors = new List<Door>();
            base.Model = Game1._models["button"];
            base.isCollidable = true;
            //Translate(Position.X, Position.Y - GameConstants.SINGLE_CELL_SIZE / 2 + 1, Position.Z); //Matt: this is for offsetting the model position so it's flat on the floor

            // Provides a hitbox for the block - Steven
            UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position), false, false);

        }

        public override void Update(RenderContext renderContext)
        {
            
            if (IsPressed && doorsToggled == false)
            {
                doorsToggled = true;
                foreach (Door door in LinkedDoors)
                {
                    door.Toggle();
                }
            }
            else if (!IsPressed && doorsToggled == true)
            {
                doorsToggled = false;
                foreach (Door door in LinkedDoors)
                {
                    door.Toggle();
                }
            }
            IsPressed = false; //button resets to up unless current pressed
            base.Update(renderContext);
        }

        public override void Draw(RenderContext renderContext)
        {
            base.Draw(renderContext);
        }
    }
}
