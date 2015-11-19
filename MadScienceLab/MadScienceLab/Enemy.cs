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
    public class Enemy : CellObject
    {
        private GameConstants.POINTDIR direction;
        public GameConstants.POINTDIR Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
            }
        }

        const int FACING_LEFT = 1, FACING_RIGHT = 2;
        byte facingDirection = FACING_RIGHT;
        int attackRange = 4;
        float movementAmount = 1f;
        float speed = 1f;

        bool movestate = true;
        private SoundEffectPlayer soundEffects;

        private GameAnimatedModel animmodel;

        // Sonar fire handling
        int elapsedFireTime = 0;
        int firingDelay = 400;

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
            base.HitboxWidth = 48;
            base.HitboxHeight = 20;
            base.HitboxHeightOffset = 20;
            soundEffects = new SoundEffectPlayer(this);
            SoundEffect sound = contentManager.Load<SoundEffect>("Sounds/DoombaLoop");
            soundEffects.LoadSound("Roomba", contentManager.Load<SoundEffect>("Sounds/DoombaLoop"));
            soundEffects.PlayAndLoopSound("Roomba");
            base.LoadContent(contentManager);

            // generate random direction and initialize
            Random rand = new Random();
            if (rand.Next(0, 1) == 0)
                direction = GameConstants.POINTDIR.pointLeft;
            else
                direction = GameConstants.POINTDIR.pointRight;
           
            
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

            if (direction == GameConstants.POINTDIR.pointLeft)
            {
                MoveLeft(movementAmount * speed);
            }
            else if(direction == GameConstants.POINTDIR.pointRight)
            {
                MoveRight(movementAmount * speed);

            }

            FireSonar(renderContext);
           
            //CheckEnemyCollision(renderContext, returnObjs);
            CheckPlayerNearby(renderContext);
            CheckEnemyBoxCollision(renderContext);
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
            //facingDirection = FACING_LEFT;
            Vector3 newPosition = Position + new Vector3(-movementAmount, 0, 0);
            Rotate(0, -90f, 0);
            Translate(newPosition);
        }

        public void MoveRight(float movementAmount)
        {
            //facingDirection = FACING_RIGHT;
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
                        movestate = !movestate;
                    }

                    if (worldObject.GetType() == typeof(BasicBlock) ||
                        worldObject.GetType() == typeof(PickableBox)) 
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
                            direction = GameConstants.POINTDIR.pointLeft;

                        }
                        if (wy > -hx)
                        {
                            //boxHitState = "Box Right";// right
                            movestate = true;
                            direction = GameConstants.POINTDIR.pointRight;
                        }

                    }

                    //Position += TransVelocity;
                }
            }
        }

        private void CheckEnemyBoxCollision(RenderContext renderContext)
        {
            
            foreach (CellObject levelObject in renderContext.Level.collidableObjects)
            {                
                if ((levelObject.isCollidable && Hitbox.Intersects(levelObject.Hitbox)
                    && levelObject.GetType() != typeof(ToggleSwitch)) || 
                    (Hitbox.Intersects(levelObject.Hitbox) && levelObject.GetType() == typeof(Character)))
                {
                    if(levelObject.GetType() == typeof(Character))
                    {
                        renderContext.Player.TakeDamage(GameConstants.PLAYER_DAMAGE, renderContext.GameTime);
                        movestate = !movestate;
                        return;
                    }
                    float wy = (levelObject.Hitbox.Width + Hitbox.Width)
                            * (levelObject.Hitbox.Center.Y - Hitbox.Center.Y);
                    float hx = ((Hitbox.Height) + levelObject.Hitbox.Height)
                             * (levelObject.Hitbox.Center.X - Hitbox.Center.X);

                    if (wy > hx)
                    {
                        //boxHitState = "Box Left";// left
                        movestate = false;
                        direction = GameConstants.POINTDIR.pointRight;

                    }
                    if (wy > -hx)
                    {
                        //boxHitState = "Box Right";// right
                        movestate = true;
                        direction = GameConstants.POINTDIR.pointLeft;
                    }

                    //if (Position.Y <= renderContext.Player.Position.Y
                    //    && Position.Y >= renderContext.Player.Position.Y - renderContext.Player.HitboxHeight
                    //    && (Position.X + HitboxWidth) <= (renderContext.Player.Position.X))
                    //{
                    //    if (movestate)
                    //    {
                    //        MoveLeft(GameConstants.MOVEAMOUNT);
                    //    }
                    //    if (!movestate)
                    //    {
                    //        MoveRight(GameConstants.MOVEAMOUNT);
                    //    }
                    //    if (wy > hx)
                    //    {
                    //        //boxHitState = "Box Left";// left
                    //        movestate = false;
                    //    }
                    //    if (wy > -hx)
                    //    {
                    //        //boxHitState = "Box Right";// right
                    //        movestate = true;
                    //    }
                    //    else
                    //    {
                    //        MoveRight(GameConstants.MOVEAMOUNT * 2);
                    //    }
                    //}

                    //// Follow character enemy on right
                    //else if ( Position.Y <= renderContext.Player.Position.Y
                    //    && Position.Y >= renderContext.Player.Position.Y - renderContext.Player.HitboxHeight
                    //    && Position.X >= renderContext.Player.Position.X + renderContext.Player.HitboxWidth)
                    //{
                    //    if (movestate)
                    //    {
                    //        MoveLeft(GameConstants.MOVEAMOUNT);
                    //    }
                    //    if (!movestate)
                    //    {
                    //        MoveRight(GameConstants.MOVEAMOUNT);
                    //    }
                    //    if (wy > hx)
                    //    {
                    //        //boxHitState = "Box Left";// left
                    //        movestate = false;
                    //    }
                    //    if (wy > -hx)
                    //    {
                    //        //boxHitState = "Box Right";// right
                    //        movestate = true;
                    //    }
                    //    else
                    //    {
                    //        MoveLeft(GameConstants.MOVEAMOUNT * 2);
                    //    }
                    //}
                    //else
                    //{
                    //    if (movestate)
                    //    {
                    //        MoveLeft(GameConstants.MOVEAMOUNT);
                    //    }
                    //    if(!movestate)
                    //    {
                    //        MoveRight(GameConstants.MOVEAMOUNT);
                    //    }
                    //    if (wy > hx)
                    //    {
                    //        //boxHitState = "Box Left";// left
                    //        movestate = false;
                    //    }
                    //    if (wy > -hx)
                    //    {
                    //        //boxHitState = "Box Right";// right
                    //        movestate = true;
                    //    }

                    //}
                }
            }
        }

        private void CheckPlayerNearby(RenderContext renderContext)
        {
            //if ((renderContext.Player.Position.X - Position.X) > GameConstants.SINGLE_CELL_SIZE * 3)
            //    movestate = false;
        }

        // trying ray casting but it not that straight forward
        //void LineOfSight(RenderContext renderContext)
        //{
        //    Vector3 playerPos = renderContext.Player.Position;
        //    Rectangle playerHitBox = renderContext.Player.Hitbox;
        //    BoundingBox playerBoundingBox = new BoundingBox(new Vector3(playerPos.X, playerPos.Y, 0), new Vector3(playerPos.X, playerPos.Y, 0)); // try convert rectagle to bounding box
        //    Ray ray;
        //    ray.Position = Position;
        //    Vector3 intersect = ray.Intersects(playerBoundingBox);

        //    Vector3 enemyPosition = Position;
        //    Vector3 playerPosition = Position;
        //    foreach (CellObject levelObject in renderContext.Level.collidableObjects)
        //    {
        //        //Check linexObstacle(if your using rectangle's look up linexRectangle, for all simple shapes, their's simple formula, for complex shapes, you simple loop though all the edges, and check linexPlane(in 2D, it's basically linexline collision).)
        //        if (/*intersect check goes here*/)
        //            playerPosition = Intesect location; //(if we just care if the other tank can't see the player, you could return false for a line of sight function here.
        //    }
        //    return playerPosition == PlayerPosition ? 1 : 0;
        //}

        /// <summary>
        /// Fires "Sonar" to check whether the character is near by/
        /// </summary>
        /// <param name="renderContext"></param>
        private void FireSonar(RenderContext renderContext)
        {
            // fire projectile using a delay
            elapsedFireTime += renderContext.GameTime.ElapsedGameTime.Milliseconds;
            if (elapsedFireTime > firingDelay)
            {
                elapsedFireTime = 0;
                //projectiles.Add(new LaserProjectile(CellNumber.X, CellNumber.Y, direction));
                EnemySonar sonar = new EnemySonar(new Vector2(Position.X, Position.Y), direction, this);
                //LaserProjectile projectile = new LaserProjectile(CellNumber.X, CellNumber.Y, direction);
                //Position the projectile according to the position of the turret
                //Actually, not necessary.
                /*float projectileX;
                if (direction == GameConstants.DIRECTION.pointLeft)
                    projectileX = this.Hitbox.Left;
                else
                    projectileX = this.Hitbox.Right - projectile.Hitbox.Width / 2;
                projectile.Translate ( projectileX, this.Hitbox.Top, this.zPosition ); //position the projectile to be a position relative to the turret
                 * */

                renderContext.Level.AddChild(sonar);
            }
        }

        //Speed up and chase player
        public void AttackMode()
        {
            animmodel.SetAnimationSpeed(3f);
            speed = 3f;
        }

        //Back to normal
        public void StandDown()
        {
            animmodel.SetAnimationSpeed(1.0f);
            speed = 1.0f;
        }
    }


}



