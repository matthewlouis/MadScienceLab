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
        private GameAnimatedModel[] animmodel = new GameAnimatedModel[2];

        //For resetting non-toggleable switches
        private static TimeSpan TOGGLE_DELAY = TimeSpan.FromMilliseconds(300);
        private static TimeSpan READY_DELAY = TimeSpan.FromMilliseconds(750);
        private TimeSpan waitTime = TimeSpan.Zero;
        public int RemainingToggles { get; set; }
        private bool ready = true;

        public ToggleSwitch(int column, int row, Boolean toggleable, int timesCanBeToggled = 99):base(column, row)
        {
            LinkedDoors = new List<SwitchableObject>();
            base.isCollidable = true;
            base.IsPassable = true;
            Toggleable = toggleable;

            //Load both green (full) and red (empty) models
            animmodel[0] = new GameAnimatedModel("LeverFull", column, row, this);
            animmodel[1] = new GameAnimatedModel("LeverEmpty", column, row, this);

            Scale(72f, 72f, 48f);
            RemainingToggles = timesCanBeToggled;

            // Matt- Steven's code for auto-calculating hitbox not working for this, so I've hardcoded it here.
            HitboxHeight = HitboxWidth = GameConstants.SINGLE_CELL_SIZE;
        }

        public void FlickSwitch()
        {
            if (ready && RemainingToggles > 0) //if ready to be switched, and still has toggles left
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
            if (ready && IsSwitched)
            {
                if (!Toggleable || RemainingToggles == 1) //if single use switch or last toggle
                    animmodel[0] = animmodel[1]; //switch to red model

                animmodel[0].PlayAnimation("Pull", false, 0f);

                foreach (SwitchableObject door in LinkedDoors)
                {
                    door.Toggle(renderContext);
                }

                IsSwitched = false;
                ready = false;
                waitTime = renderContext.GameTime.TotalGameTime; //get time when switched
                if(Toggleable)
                    RemainingToggles--; //reduce toggle count
            }

            //Checks if enough time has passed to switch back to original position
            if (!ready)
            {
                if (renderContext.GameTime.TotalGameTime - waitTime >= TOGGLE_DELAY)
                {
                    animmodel[0].PlayAnimation("Release", false, 0f);

                    if (Toggleable && renderContext.GameTime.TotalGameTime - waitTime >= READY_DELAY)
                    {
                        ready = true;
                    }
                }              
            }
            animmodel[0].Update(renderContext);
            base.Update(renderContext);
        }

        public override void Draw(RenderContext renderContext)
        {
            animmodel[0].Draw(renderContext);
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager)
        {
            base.LoadContent(contentManager);
            animmodel[0].LoadContent(contentManager);
            animmodel[1].LoadContent(contentManager);

            // Provides a hitbox for the block - Steven
            UpdateBoundingBox(animmodel[0].Model, Matrix.CreateTranslation(animmodel[0].Position), true, false);
        }
    }
}
