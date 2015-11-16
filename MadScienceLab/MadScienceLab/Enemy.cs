using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections;


namespace MadScienceLab
{
    class Enemy : CellObject
    {
        const int FACING_LEFT = 1, FACING_RIGHT = 2;
        byte facingDirection = FACING_RIGHT;
        int attackRange = 4;

        bool movestate = false;
        private SoundEffectPlayer soundEffects;

        private GameAnimatedModel animmodel;

        public Enemy(int column, int row)
            : base(column, row)
        {
            animmodel = new GameAnimatedModel("Doomba", column, row, this);
            animmodel.PlayAnimation("Move", true, 0f);
            base.isCollidable = true;
            Scale(48f, 48f, 48f);
            Position = new Vector3(Position.X, Position.Y - 18, Position.Z);
            MoveLeft(1f);
        }

        public override void LoadContent(ContentManager contentManager)
        {
            animmodel.LoadContent(contentManager);
            // Provides a hitbox for the block - Steven
            UpdateBoundingBox(animmodel.Model, Matrix.CreateTranslation(base.Position), false, false);

            soundEffects = new SoundEffectPlayer(this);
            SoundEffect sound = contentManager.Load<SoundEffect>("Sounds/DoombaLoop");
            soundEffects.LoadSound("Roomba", contentManager.Load<SoundEffect>("Sounds/DoombaLoop"));
            soundEffects.PlayAndLoopSound("Roomba");
            base.LoadContent(contentManager);
        }

        public override void Update(RenderContext renderContext)
        {
            checkEnemyBoxCollision(renderContext);
            soundEffects.Update(renderContext);
            animmodel.Update(renderContext);
            base.Update(renderContext);
        }

        public override void Draw(RenderContext renderContext)
        {
            animmodel.Draw(renderContext);
        }

        public void MoveLeft(float movementAmount)
        {
            facingDirection = FACING_LEFT;
            Vector3 newPosition = Position + new Vector3(-movementAmount, 0, 0);
            Rotate(0, -90f, 0);
            Translate(newPosition);
        }

        public void MoveRight(float movementAmount)
        {
            facingDirection = FACING_RIGHT;
            Vector3 newPosition = Position + new Vector3(movementAmount, 0, 0);
            Rotate(0, 90f, 0);
            Translate(newPosition);
        }

        private void checkEnemyBoxCollision(RenderContext renderContext)
        {
            foreach (CellObject levelObject in renderContext.Level.Children)
            {
                if (levelObject.isCollidable && Hitbox.Intersects(levelObject.Hitbox))
                {
                    

                    if ((Position.Y - HitboxHeight / 2) <= renderContext.Player.Position.Y
                        && (Position.Y - HitboxHeight / 2) >= renderContext.Player.Position.Y - renderContext.Player.HitboxHeight
                        && (Position.X + HitboxWidth) >= (renderContext.Player.Position.X - (HitboxWidth * attackRange))
                        && (Position.X + HitboxWidth) <= (renderContext.Player.Position.X + renderContext.Player.HitboxWidth))
                    {
                        MoveRight(GameConstants.MOVEAMOUNT * 2);
                    }
                    else if ((Position.Y - HitboxHeight / 2) <= renderContext.Player.Position.Y
                        && (Position.Y - HitboxHeight / 2) >= renderContext.Player.Position.Y - renderContext.Player.HitboxHeight
                        && Position.X <= (renderContext.Player.Position.X + renderContext.Player.HitboxWidth + (HitboxWidth * attackRange))
                        && Position.X >= (renderContext.Player.Position.X + renderContext.Player.HitboxWidth))
                    {
                        MoveLeft(GameConstants.MOVEAMOUNT * 2);
                    }
                    else
                    {
                        /**Determining what side was hit**/
                        float wy = (levelObject.Hitbox.Width + Hitbox.Width)
                                 * (((levelObject.Hitbox.Y + levelObject.Hitbox.Height) / 2) - (Hitbox.Y + Hitbox.Height) / 2);
                        float hx = (Hitbox.Height + levelObject.Hitbox.Height)
                                 * (((levelObject.Hitbox.X + levelObject.Hitbox.Width) / 2) - (Hitbox.X + Hitbox.Width) / 2);

                        if (movestate)
                        {
                            MoveLeft(GameConstants.MOVEAMOUNT);

                        }
                        else
                        {
                            MoveRight(GameConstants.MOVEAMOUNT);
                        }

                        if (wy > hx)
                        {
                            //boxHitState = "Box Left";// left
                            movestate = false;
                        }
                        if (wy > -hx)
                        {
                            //boxHitState = "Box Right";// right
                            movestate = true;
                        }
                    }

                }

            }
        }
    }
}



