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

        //For resetting non-toggleable switches
        private static TimeSpan TOGGLE_DELAY = TimeSpan.FromMilliseconds(750);
        private TimeSpan waitTime = TimeSpan.Zero;
        public int TimesCanBeToggled { get; set; }
        private bool ready = true;

        public ToggleSwitch(int column, int row, Boolean toggleable, int timesCanBeToggled = 1):base(column, row)
        {
            LinkedDoors = new List<SwitchableObject>();
            base.Model = GameplayScreen._models["switch"];
            base.isCollidable = true;
            base.IsPassable = true;
            Toggleable = toggleable;
            Translate(Position.X, Position.Y - GameConstants.SINGLE_CELL_SIZE / 2, Position.Z); //Matt: this is for offsetting the model position so it's flat on the floor
            TimesCanBeToggled = timesCanBeToggled;

            // Matt- Steven's code for auto-calculating hitbox not working for this, so I've hardcoded it here.
            HitboxHeight = HitboxWidth = GameConstants.SINGLE_CELL_SIZE;
        }

        public void FlickSwitch()
        {
            if (ready && TimesCanBeToggled > 0) //if ready to be switched, and still has toggles left
            {
                if (IsSwitched)
                {
                    IsSwitched = false;
                }
                else
                {
                    IsSwitched = true;
                }
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

                if (!Toggleable) //If this is a non-toggle switch, implement logic to reset back to original position
                {
                    ready = false;
                    waitTime = renderContext.GameTime.TotalGameTime; //get time when switched
                    TimesCanBeToggled--; //reduce toggle count
                }
            }
            else if (!IsSwitched && doorsToggled == true)
            {
                Scale(1f, 1f, 1f); //FLIP MODEL BACK

                doorsToggled = false;

                if (Toggleable)
                {
                    foreach (SwitchableObject door in LinkedDoors)
                    {
                        door.Toggle(renderContext);
                    }
                }
            }

            //Checks if enough time has passed to switch back to original position
            if (!ready &&
                renderContext.GameTime.TotalGameTime - waitTime >= TOGGLE_DELAY)
            {
                ready = true;
                IsSwitched = false;
            }
            base.Update(renderContext);
        }

        public override void Draw(RenderContext renderContext)
        {
            if (IsSwitched) //Because we're flipping model to show change, we have to draw from inside out.
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
