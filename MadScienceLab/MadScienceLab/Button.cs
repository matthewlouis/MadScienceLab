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
        private GameAnimatedModel animmodel;

        public Button(int column, int row):base(column, row)
        {
            LinkedDoors = new List<SwitchableObject>();
            animmodel = new GameAnimatedModel("Button", column, row, this);
            base.isCollidable = true;
            base.IsPassable = true;
            HitboxHeightOffset = 17;
            Translate(Position.X, Position.Y - GameConstants.SINGLE_CELL_SIZE/2 + 1, Position.Z); //Matt: this is for offsetting the model position so it's flat on the floor
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager)
        {
            base.LoadContent(contentManager);
            soundEffects = new SoundEffectPlayer(this);
            soundEffects.LoadSound("Button", GameplayScreen._sounds["Button"]);
            animmodel.LoadContent(contentManager);

            // Provides a hitbox for the block - Steven
            UpdateBoundingBox(animmodel.Model, Matrix.CreateTranslation(animmodel.Position), true, false);
        }

        public override void Update(RenderContext renderContext)
        {
            
            //Handle pressing of the button.
            if (IsPressed && doorsToggled == false)
            {
                animmodel.PlayAnimation("Press", false, 0f);
                soundEffects.PlaySound("Button");
                doorsToggled = true;
                foreach (SwitchableObject door in LinkedDoors)
                {
                    door.Toggle(renderContext);
                }
            }
            else if (!IsPressed && doorsToggled == true)
            {
                animmodel.PlayAnimation("Pop", false, 0f);
                soundEffects.PlaySound("Button");
                doorsToggled = false;
                foreach (SwitchableObject door in LinkedDoors)
                {
                    door.Toggle(renderContext);
                }
            }
            IsPressed = false; //button resets to up unless current pressed
            animmodel.Update(renderContext);
            base.Update(renderContext);
        }

        public override void Draw(RenderContext renderContext)
        {
           animmodel.Draw(renderContext);
        }
    }
}
