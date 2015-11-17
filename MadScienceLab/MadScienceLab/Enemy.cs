﻿using System;
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
        float movementAmount = GameConstants.MOVEAMOUNT;

        bool movestate = true;
        private SoundEffectPlayer soundEffects;

        private GameAnimatedModel animmodel;

        public Enemy(int column, int row)
            : base(column, row)
        {
            animmodel = new GameAnimatedModel("Doomba", column, row, this);
            animmodel.PlayAnimation("Move", true, 0f);

            isCollidable = false;

            Scale(48f, 48f, 48f);
            Position = new Vector3(Position.X, Position.Y - 18, Position.Z);
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

            base.HitboxHeight = 48;
            base.HitboxHeight = 12;
            base.HitboxHeightOffset = 18;
        }

        public override void Update(RenderContext renderContext)
        {

            List<CellObject> returnObjs = new List<CellObject>();
            renderContext.Quadtree.clear();
            foreach (CellObject obj in renderContext.Level.collidableObjects)
            {
                    renderContext.Quadtree.insert(obj);
            }

            renderContext.Quadtree.retrieve(returnObjs, Hitbox);

            if (movestate)
            {
                MoveLeft(GameConstants.MOVEAMOUNT);
            }
            else
            {
                MoveRight(GameConstants.MOVEAMOUNT);
            }

            CheckEnemyCollision(renderContext, returnObjs);
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

        public void CheckEnemyCollision(RenderContext renderContext, List<CellObject> returnObjs)
        {
            // if collision with player, handle
            /* if(this.Hitbox.Intersects(renderContext.Player.Hitbox))
             {
                 active = false;
                 TransVelocity = Vector3.Zero;
                 renderContext.Player.SetHealth(GameConstants.PLAYER_DAMAGE);
             }*/

            // Quad tree collison checks - Steven
            foreach (CellObject worldObject in returnObjs)
            {
                if (!worldObject.IsPassable && Hitbox.Intersects(worldObject.Hitbox))
                {
                    if (worldObject.GetType() == typeof(Character))
                    {
                        renderContext.Player.TakeDamage(GameConstants.PLAYER_DAMAGE, renderContext.GameTime);
                        TransVelocity = Vector3.Zero;
                    }
                    else
                    {
                        TransVelocity = Vector3.Zero;
                    }

                    if (worldObject.GetType() == typeof(BasicBlock) ||
                        worldObject.GetType() == typeof(PickableBox)) ;
                    {
                        /**Determining what side was hit**/

                        // what does this section of code do?? used to figure which side was hit, top, bottom, left, right
                        // what diffines the left side from the right side? if statements below
                        float wy = (worldObject.Hitbox.Width - Hitbox.Width)
                                 * (worldObject.Hitbox.Center.Y - Hitbox.Center.Y);
                        float hx = (Hitbox.Height + worldObject.Hitbox.Height)
                                 * (worldObject.Hitbox.Center.X - Hitbox.Center.X);
                        
                        //// Follow character enemy on left
                        //if (Position.Y <= renderContext.Player.Position.Y
                        //    && Position.Y >= renderContext.Player.Position.Y - renderContext.Player.HitboxHeight
                        //    && (Position.X + HitboxWidth) <= (renderContext.Player.Position.X + renderContext.Player.HitboxWidth))
                        //{
                        //    movestate = true;
                        //}

                        //// Follow character enemy on right
                        //else if (Position.Y <= renderContext.Player.Position.Y
                        //    && Position.Y >= renderContext.Player.Position.Y - renderContext.Player.HitboxHeight
                        //    && Position.X >= renderContext.Player.Position.X + renderContext.Player.HitboxWidth)
                        //{
                        //    movestate = false;
                        //}



                        if (wy > hx)
                        {
                            //boxHitState = "Box Left";// left
                            movestate = false;
                        } else if (wy > -hx)
                        {
                            //    //boxHitState = "Box Right";// right
                            movestate = true;
                        }

                    }

                    //Position += TransVelocity;
                }
            }
        }

            

        private void checkEnemyBoxCollision(RenderContext renderContext)
        {
            //foreach (CellObject levelObject in renderContext.Level.collidableObjects)
            //{
    
            //    if (levelObject.isCollidable && Hitbox.Intersects(levelObject.Hitbox))
            //    {
            //        /**Determining what side was hit**/
            //        float wy = (levelObject.Hitbox.Width + Hitbox.Width)
            //                 * (levelObject.Hitbox.Center.Y - Hitbox.Center.Y);
            //        float hx = (Hitbox.Height + levelObject.Hitbox.Height)
            //                 * (levelObject.Hitbox.Center.X - Hitbox.Center.X);

            //        if (!levelObject.IsPassable) //if object is not passable, handle physics issues:
            //        {
            //            if (wy > hx)
            //            {
            //                //boxHitState = "Box Left";// left
            //                MoveRight(GameConstants.MOVEAMOUNT);                       
            //            }
            //            else
            //            {
            //                if (wy > -hx)
            //                {
            //                    //boxHitState = "Box Right";// right
            //                    MoveLeft(GameConstants.MOVEAMOUNT);
            //                }

            //            }
            //        }
            //    }
            //}

            foreach (CellObject worldObject in renderContext.Level.collidableObjects)
            {                
                if (worldObject.isCollidable && Hitbox.Intersects(worldObject.Hitbox)
                    && worldObject.GetType() != typeof(Enemy))
                {
                    /**Determining what side was hit**/
                    float wy = (worldObject.Hitbox.Width + Hitbox.Width)
                             * (worldObject.Hitbox.Center.Y - Hitbox.Center.Y);
                    float hx = (Hitbox.Height + worldObject.Hitbox.Height)
                             * (worldObject.Hitbox.Center.X - Hitbox.Center.X);
                    // Follow character enemy on left
                    if (Position.Y <= renderContext.Player.Position.Y
                        && Position.Y >= renderContext.Player.Position.Y - renderContext.Player.HitboxHeight
                        && (Position.X + HitboxWidth) <= (renderContext.Player.Position.X + renderContext.Player.HitboxWidth))
                    {
                        MoveRight(GameConstants.MOVEAMOUNT * 2);
                    }

                    // Follow character enemy on right
                    else if ( Position.Y <= renderContext.Player.Position.Y
                        && Position.Y >= renderContext.Player.Position.Y - renderContext.Player.HitboxHeight
                        && Position.X >= renderContext.Player.Position.X + renderContext.Player.HitboxWidth)
                    {
                        MoveLeft(GameConstants.MOVEAMOUNT * 2);
                    }
                    else
                    {
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



