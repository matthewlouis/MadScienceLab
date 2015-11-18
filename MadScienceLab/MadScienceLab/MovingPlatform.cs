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
<<<<<<< HEAD
        const GameConstants.DIRECTION //making shorthand for each direction
            LEFT = GameConstants.DIRECTION.Left, 
            RIGHT = GameConstants.DIRECTION.Right, 
            DOWN = GameConstants.DIRECTION.Down, 
            UP = GameConstants.DIRECTION.Up;

        public float maxDistance {get; set;}
        float currDistance = 0;
        public GameConstants.DIRECTION facingDirection, movingDirection;
=======
        const int FACING_LEFT = 1, FACING_RIGHT = 2, FACING_BOTTOM = 3, FACING_TOP = 4;
        byte facingDirection = FACING_RIGHT;

        public float maxDistance {get; set;}
        float currDistance = 0;
        public bool movingLeft = false;
>>>>>>> refs/remotes/origin/master
        public bool PlayerOnPlatform = false;


        public MovingPlatform(int column, int row)
            : base(column, row)
        {
            base.Model = GameplayScreen._models["BasicBlock"];
            base.isCollidable = true;
            maxDistance = 2 * GameConstants.SINGLE_CELL_SIZE; //default distance
<<<<<<< HEAD
            this.facingDirection = RIGHT;
            this.movingDirection = RIGHT; //default direction
=======
>>>>>>> refs/remotes/origin/master

            // Provides a hitbox for the block - Steven
            UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position), false, false);
        }

        public override void Update(RenderContext renderContext)
        {
            CheckEnemyBoxCollision(renderContext);
            if (currDistance > maxDistance)
            {
<<<<<<< HEAD
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
=======
                movingLeft = !movingLeft;
>>>>>>> refs/remotes/origin/master
                currDistance = 0;
            }
            base.Update(renderContext);
        }

<<<<<<< HEAD
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
=======
        public void MoveLeft(RenderContext renderContext, float movementAmount)
        {
            facingDirection = FACING_LEFT;
            Rotate(0f, -90f, 0f);
            Vector3 newPosition = Position + new Vector3(-movementAmount, 0, 0);
>>>>>>> refs/remotes/origin/master
            currDistance += movementAmount;
            Translate(newPosition);

            //if player is on the platform, move the player just as much as the platform does
            if (PlayerOnPlatform)
            {
<<<<<<< HEAD
                Vector3 newPlayerPosition = renderContext.Player.Position + PositionChange;
=======
                Vector3 newPlayerPosition = renderContext.Player.Position + new Vector3(-movementAmount, 0, 0);
                renderContext.Player.Translate(newPlayerPosition);
            }
        }

        public void MoveRight(RenderContext renderContext, float movementAmount)
        {
            facingDirection = FACING_RIGHT;
            Rotate(0f, 90f, 0f);
            Vector3 newPosition = Position + new Vector3(movementAmount, 0, 0);
            currDistance += movementAmount;
            Translate(newPosition);

            //if player is on the platform, move the player just as much as the platform does
            if (PlayerOnPlatform)
            {
                Vector3 newPlayerPosition = renderContext.Player.Position + new Vector3(movementAmount, 0, 0);
>>>>>>> refs/remotes/origin/master
                renderContext.Player.Translate(newPlayerPosition);
            }
        }

        private void CheckEnemyBoxCollision(RenderContext renderContext)
        {
            foreach (CellObject levelObject in renderContext.Level.Children)
            {

                if (levelObject.isCollidable && Hitbox.Intersects(levelObject.Hitbox))
                {
                    /**Determining what side was hit**/
                    float wy = (levelObject.Hitbox.Width + Hitbox.Width)
                             * (((levelObject.Hitbox.Y + levelObject.Hitbox.Height) / 2) - (Hitbox.Y + Hitbox.Height) / 2);
                    float hx = (Hitbox.Height + levelObject.Hitbox.Height)
                             * (((levelObject.Hitbox.X + levelObject.Hitbox.Width) / 2) - (Hitbox.X + Hitbox.Width) / 2);

<<<<<<< HEAD
                    Move(renderContext, movingDirection, GameConstants.MOVEAMOUNT); //Move box in the respective direction

                    /*if (wy > hx)
                    {
                        //boxHitState = "Box Left";// left
                        movingDirection = RIGHT;
=======
                    if (movingLeft)
                    {
                        MoveLeft(renderContext, GameConstants.MOVEAMOUNT);

                    }
                    else
                    {
                        MoveRight(renderContext, GameConstants.MOVEAMOUNT);
                    }

                    if (wy > hx)
                    {
                        //boxHitState = "Box Left";// left
                        movingLeft = false;
>>>>>>> refs/remotes/origin/master
                        currDistance = 0;
                    }
                    if (wy > -hx)
                    {
                        //boxHitState = "Box Right";// right
<<<<<<< HEAD
                        movingDirection = LEFT;
                        currDistance = 0;
                    }*/
                    //Reverse directions based on collision with another object
                    if (wy > hx)
                    {
                        if (movingDirection == UP)
                        {
                            movingDirection = DOWN;
                            currDistance = 0;
                        }
                        if (movingDirection == LEFT)
                        {
                            movingDirection = RIGHT;
                            currDistance = 0;
                        }
                    }
                    
                    else if (wy > -hx)
                    {
                        if (movingDirection == RIGHT)
                        {
                            movingDirection = LEFT;
                            currDistance = 0;
                        }
                        if (movingDirection == DOWN)
                        {
                            movingDirection = UP;
                            currDistance = 0;
                        }
                    }
                    
=======
                        movingLeft = true;
                        currDistance = 0;
                    }
>>>>>>> refs/remotes/origin/master



                }

            }
        }
    }
}



