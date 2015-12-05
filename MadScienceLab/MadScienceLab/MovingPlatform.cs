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
    class MovingPlatform : CellObject
    {
        const GameConstants.DIRECTION //making shorthand for each direction
            LEFT = GameConstants.DIRECTION.Left, 
            RIGHT = GameConstants.DIRECTION.Right, 
            DOWN = GameConstants.DIRECTION.Down, 
            UP = GameConstants.DIRECTION.Up;

        public float maxDistance {get; set;}
        float currDistance = 0;
        public GameConstants.DIRECTION facingDirection, movingDirection;
        public bool PlayerOnPlatform = false;


        public MovingPlatform(int column, int row)
            : base(column, row)
        {
            base.Model = GameplayScreen._models["MovingBlock"];
            base.isCollidable = true;
            maxDistance = 2 * GameConstants.SINGLE_CELL_SIZE; //default distance
            this.facingDirection = RIGHT;
            this.movingDirection = RIGHT; //default direction
            Scale(1, 0.25f, 1);
            Translate(Position.X, Position.Y + 18, Position.Z);

            // Provides a hitbox for the moving platform
            UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position), false, false);
            HitboxHeight = 12;
            HitboxHeightOffset = 18;
            
        }

        public override void Update(RenderContext renderContext)
        {
            CheckPlatformBoxCollision(renderContext);
            if (currDistance > maxDistance)
            {
                //reverse direction if exceeding distance
                if (movingDirection == LEFT)
                {
                    movingDirection = RIGHT;
                }
                else if (movingDirection == RIGHT)
                {
                    movingDirection = LEFT;
                }
                else if (movingDirection == UP)
                {
                    movingDirection = DOWN;
                }
                else if (movingDirection == DOWN)
                {
                    movingDirection = UP;
                }
                currDistance = 0;
            }
            Move ( renderContext, movingDirection, GameConstants.MOVEAMOUNT ); //Move box in the respective direction
            base.Update(renderContext);
        }

        /// <summary>
        /// Move the box in the current moveDir.
        /// </summary>
        /// <param name="renderContext"></param>
        /// <param name="moveDir"></param>
        /// <param name="movementAmount"></param>
        public void Move(RenderContext renderContext, GameConstants.DIRECTION moveDir, float movementAmount)
        {
            facingDirection = moveDir;
            //Rotate(0f, -90f, 0f); //No need to rotate moving platform to set its direction
            Vector3 newPosition;
            Vector3 PositionChange;

            //Get position change depending on direction
            if(moveDir == LEFT)
                PositionChange = new Vector3(-movementAmount, 0, 0);
            else if (moveDir == RIGHT)
                PositionChange = new Vector3(movementAmount, 0, 0);
            else if (moveDir == UP)
                PositionChange = new Vector3(0, movementAmount, 0);
            else// if (moveDir == DOWN)
                PositionChange = new Vector3(0, -movementAmount, 0);

            newPosition = Position + PositionChange;
            currDistance += movementAmount;
            Translate(newPosition);

            //if player is on the platform, move the player just as much as the platform does
            if (PlayerOnPlatform)
            {
                Vector3 newPlayerPosition = renderContext.Player.Position + PositionChange;
                renderContext.Player.Translate(newPlayerPosition);
            }
        }

        /// <summary>
        /// Note: As of 11/18/15, the current version of the collision code has some bugs. It sometimes detects boxes next to it as being eg. collidable from the bottom.
        /// </summary>
        /// <param name="renderContext"></param>
        private void CheckPlatformBoxCollision(RenderContext renderContext)
        {
            foreach (CellObject levelObject in renderContext.Level.Children)
            {
                bool typeAllowed = levelObject.GetType () == typeof ( BasicBlock ) || levelObject.GetType () == typeof ( MovingPlatform ) || levelObject.GetType () == typeof ( Door ) || levelObject.GetType () == typeof ( Trapdoor );

                //Collide only with objects under typeAllowed; other objects will not influence MovingPlatform's position
                if (levelObject.isCollidable && typeAllowed && Hitbox.Intersects(levelObject.Hitbox) && levelObject != this)
                {
                    Rectangle intersect = Rectangle.Intersect(Hitbox, levelObject.Hitbox);

                    // Reverse moving direction based on this method of checking collision.
                    /**Determining what side was hit**/
                    float wy = (levelObject.Hitbox.Width + Hitbox.Width)
                             * (((levelObject.Hitbox.Y + levelObject.Hitbox.Height) / 2) - (Hitbox.Y + Hitbox.Height) / 2);
                    float hx = (Hitbox.Height + levelObject.Hitbox.Height)
                             * (((levelObject.Hitbox.X + levelObject.Hitbox.Width) / 2) - (Hitbox.X + Hitbox.Width) / 2);

                    // Reverse directions based on collision with another object
                    // Given issues with quickly flipping directions due to faulty collisions,
                    // Will have to figure a workaround.
                    // Maybe not rely on collision to determine moving platform movement, and instead just activate\inactivate moving platforms ...
                    if (wy > hx)
                    {
                        if (wy > -hx)
                        {
                            if (movingDirection == UP)
                            {
                                movingDirection = DOWN;
                                currDistance = 0;
                            }
                        }
                        else
                        {
                            if (movingDirection == LEFT)
                            {
                                movingDirection = RIGHT;
                                currDistance = 0;
                            }
                        }
                    }

                    else //Right, down
                    {
                        if (wy > -hx)
                        {
                            if (movingDirection == RIGHT)
                            {
                                movingDirection = LEFT;
                                currDistance = 0;
                            }
                        }
                        else
                        {
                            if (movingDirection == DOWN)
                            {
                                Position = new Vector3 ( Position.X, levelObject.Hitbox.Y + this.Hitbox.Height, 0 ); //clip to the top of the colliding object
                                movingDirection = UP;
                                currDistance = 0;
                            }
                        }
                    }


                }

            }
        }
    }
}



