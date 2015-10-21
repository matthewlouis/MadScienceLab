using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    class ToggleSwitch:CellObject
    {
        public List<SwitchableObject> LinkedDoors { get; set; }
        public Boolean Toggleable { get; private set; }
        public Boolean IsSwitched { get; set; }
        private Boolean doorsToggled = false;

        public ToggleSwitch(int column, int row, Boolean toggleable):base(column, row)
        {
            LinkedDoors = new List<SwitchableObject>();
            base.Model = Game1._models["switch"];
            base.isCollidable = true;
            base.IsPassable = true;
            Toggleable = toggleable;
            Translate(Position.X, Position.Y - GameConstants.SINGLE_CELL_SIZE / 2, Position.Z); //Matt: this is for offsetting the model position so it's flat on the floor

            // Matt- Steven's code for auto-calculating hitbox not working for this, so I've hardcoded it here.
            HitboxHeight = HitboxWidth = GameConstants.SINGLE_CELL_SIZE;
        }

        public void FlickSwitch()
        {
            if (IsSwitched && Toggleable)
            {
                IsSwitched = false;
            }
            else
            {
                IsSwitched = true;
            }
        }



        public override void Update(RenderContext renderContext)
        {
            if (IsSwitched && doorsToggled == false)
            {
                Scale(-1f, 1f, 1f); //FLIP MODEL

                doorsToggled = true;
                foreach (SwitchableObject door in LinkedDoors)
                {
                    door.Toggle(renderContext);
                }
            }
            else if (!IsSwitched && doorsToggled == true)
            {
                Scale(1f, 1f, 1f); //FLIP MODEL BACK

                doorsToggled = false;
                foreach (SwitchableObject door in LinkedDoors)
                {
                    door.Toggle(renderContext);
                }
            }
            base.Update(renderContext);
        }

        public override void Draw(RenderContext renderContext)
        {
            if (IsSwitched)
            {
                RasterizerState prevState = renderContext.GraphicsDevice.RasterizerState;
                RasterizerState state = new RasterizerState();
                state.CullMode = CullMode.None;
                renderContext.GraphicsDevice.RasterizerState = state;
                base.Draw(renderContext);
                renderContext.GraphicsDevice.RasterizerState = prevState;
            }else
                base.Draw(renderContext);

        }
    }
}
