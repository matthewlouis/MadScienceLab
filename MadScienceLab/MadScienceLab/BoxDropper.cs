﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MadScienceLab
{
    class BoxDropper:SwitchableObject
    {
        public int NumberOfBoxes { get; private set; }
        public bool IsReady { 
            get {
                PickableBox newBox = new PickableBox(new Vector2(this.Position.X, this.Position.Y - GameConstants.SINGLE_CELL_SIZE)); //used just for the purposes of getting PickableBox's bounding box.
                Rectangle areaBelow = new Rectangle((int)Position.X, (int)Position.Y - newBox.Hitbox.Height, (int)newBox.Hitbox.Width, (int)newBox.Hitbox.Height);
                bool ready = true;
                foreach (CellObject levelObject in GameplayScreen.CurrentLevel.Children) //check to see if it has collision with anything
                {
                    if (levelObject.isCollidable && levelObject.GetType() != typeof(BasicBlock) && areaBelow.Intersects(levelObject.Hitbox))
                    {
                        ready = false;
                    }
                }
                return ready;
            }
        }
        private int ReservedBoxes;
        private int row, column;

        private GameAnimatedModel[] animmodel = new GameAnimatedModel[2];

        public BoxDropper(int column, int row, int numberOfBoxes)
            : base(column, row)
        {
            this.row = row;
            this.column = column;
            this.ReservedBoxes = 0;
            base.Model = GameplayScreen._models["BlockDropper"];

            //Set up animations
            animmodel[0] = new GameAnimatedModel("BoxDropperAnimated", column, row, this);
            animmodel[1] = new GameAnimatedModel("BoxDropperEmptyAnimated", column, row, this);
            animmodel[0].SetAnimationSpeed(1.2f);
            animmodel[1].SetAnimationSpeed(1.2f);

            Scale(12f, 12f, 12f);
            NumberOfBoxes = numberOfBoxes;
            isCollidable = true;
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager)
        {
            base.LoadContent(contentManager);

            //Loads both empty and full models
            animmodel[0].LoadContent(contentManager);
            animmodel[1].LoadContent(contentManager);

            UpdateBoundingBox(animmodel[0].Model, Matrix.CreateTranslation(animmodel[0].Position), false, false);
        }

        public override void Update(RenderContext renderContext)
        {
            animmodel[0].Update(renderContext);
            base.Update(renderContext);
        }

        //Drops a box as soon as it can.
        public override void Toggle(RenderContext renderContext)
        {
            if (NumberOfBoxes == 0) //if empty don't do anything
            {
                return;
            }

            if (NumberOfBoxes > 0)
            {
                ReservedBoxes++;
                NumberOfBoxes--;

                if(NumberOfBoxes == 0)         //if now empty
                    animmodel[0] = animmodel[1]; //replace model with empty one
            }
            animmodel[0].PlayAnimationOnceNoLoop("Drop", 0f);
        }

        public override void Draw(RenderContext renderContext)
        {
            animmodel[0].Draw(renderContext);
        }

        public override void Update(RenderContext renderContext)
        {
            if (IsReady && ReservedBoxes > 0)
            {
                //Creates new PickableBox underneath dropper.
                PickableBox newBox = new PickableBox(new Vector2(this.Position.X, this.Position.Y - GameConstants.SINGLE_CELL_SIZE));
                renderContext.Level.AddChild(newBox);
                ReservedBoxes--;
            }
            base.Update(renderContext);
        }
    }
}
