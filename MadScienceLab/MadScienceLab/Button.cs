using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace MadScienceLab
{
    class Button:CellObject
    {
        public List<SwitchableObject> LinkedDoors { get; set; }
        public Boolean IsPressed { get; set; }
        private Boolean doorsToggled = false;
        private SoundEffectPlayer soundEffects;

        public Button(int column, int row):base(column, row)
        {
            LinkedDoors = new List<SwitchableObject>();
            base.Model = Game1._models["button"];
            base.isCollidable = true;
            base.IsPassable = true;
            HitboxHeightOffset = 10;
            Translate(Position.X, Position.Y - GameConstants.SINGLE_CELL_SIZE / 2 + 1, Position.Z); //Matt: this is for offsetting the model position so it's flat on the floor

            // Provides a hitbox for the block - Steven
            UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position), false, false);

        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager)
        {
            soundEffects = new SoundEffectPlayer(this);
            soundEffects.LoadSound("Button", Game1._sounds["Button"]);
            base.LoadContent(contentManager);
        }

        public override void Update(RenderContext renderContext)
        {
            
            if (IsPressed && doorsToggled == false)
            {
                soundEffects.PlaySound("Button");
                doorsToggled = true;
                foreach (SwitchableObject door in LinkedDoors)
                {
                    door.Toggle(renderContext);
                }
            }
            else if (!IsPressed && doorsToggled == true)
            {
                soundEffects.PlaySound("Button");
                doorsToggled = false;
                foreach (SwitchableObject door in LinkedDoors)
                {
                    door.Toggle(renderContext);
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
