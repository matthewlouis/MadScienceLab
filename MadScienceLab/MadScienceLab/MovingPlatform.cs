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

            // Provides a hitbox for the block - Steven
            UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position), false, false);
            //Shrink the bounding box to 0.9x of its dimensions, while maintaining the same center
            /*HitboxWidthOffset += (int)(HitboxWidth * 0.05);
            HitboxWidth = (int)(HitboxWidth*0.9);
            HitboxHeightOffset += (int)(HitboxHeight * 0.05);
            HitboxHeight = (int)(HitboxHeight * 0.9);*/
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
                bool typeAllowed = levelObject.GetType () == typeof ( BasicBlock ) || levelObject.GetType () == typeof ( Door ) || levelObject.GetType () == typeof ( Trapdoor );
                //Collide only with BasicBlocks; other objects will not influence MovingPlatform's position
                if (levelObject.isCollidable && typeAllowed && Hitbox.Intersects(levelObject.Hitbox) && levelObject != this)
                {
                    Rectangle intersect = Rectangle.Intersect(Hitbox, levelObject.Hitbox);

                    //Reverse moving direction based on this method of checking collision.
                    /*if (intersect.Width > intersect.Height) //from the top or bottom
                    {
                        if (movingDirection == UP)
                            movingDirection = DOWN;
                        else if (movingDirection == DOWN)
                            movingDirection = UP;
                    }
                    else //from the left or right
                    {
                        if (movingDirection == LEFT)
                            movingDirection = RIGHT;
                        else if (movingDirection == RIGHT)
                            movingDirection = LEFT;
                    }*/
                    /**Determining what side was hit**/
                    float wy = (levelObject.Hitbox.Width + Hitbox.Width)
                             * (((levelObject.Hitbox.Y + levelObject.Hitbox.Height) / 2) - (Hitbox.Y + Hitbox.Height) / 2);
                    float hx = (Hitbox.Height + levelObject.Hitbox.Height)
                             * (((levelObject.Hitbox.X + levelObject.Hitbox.Width) / 2) - (Hitbox.X + Hitbox.Width) / 2);

                    /*bool topCollision = (wy > hx) && (wy > -hx);
                    bool leftCollision = (wy > hx) && !(wy > -hx);
                    bool bottomCollision = !(wy > hx) && (wy > -hx);
                    bool rightCollision = !(wy > hx) && !(wy > -hx);*/

                    /*if (wy > hx)
                    {
                        //boxHitState = "Box Left";// left
                        movingDirection = RIGHT;
                        currDistance = 0;
                    }
                    if (wy > -hx)
                    {
                        //boxHitState = "Box Right";// right
                        movingDirection = LEFT;
                        currDistance = 0;
                    }*/
                    //Reverse directions based on collision with another object
                    //Given issues with quickly flipping directions due to faulty collisions,
                    //Will have to figure a workaround.
                    //Maybe not rely on collision to determine moving platform movement, and instead just activate\inactivate moving platforms ...
                    if (wy > hx)
                    {
                        if (wy > -hx)
                        {
                            if (movingDirection == UP)
                            {
                                //Position = new Vector3 ( Position.X, levelObject.Hitbox.Bottom - this.Hitbox.Height, 0 );
                                movingDirection = DOWN;
                                currDistance = 0;
                            }
                        }
                        else
                        {
                            if (movingDirection == LEFT)
                            {
                                //Position = new Vector3 ( levelObject.Hitbox.Right, Position.Y, 0 );
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
                                //Position = new Vector3 ( levelObject.Hitbox.Left - this.Hitbox.Width, Position.Y, 0 );
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



