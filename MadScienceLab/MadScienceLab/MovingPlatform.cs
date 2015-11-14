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
        const int FACING_LEFT = 1, FACING_RIGHT = 2, FACING_BOTTOM = 3, FACING_TOP = 4;
        byte facingDirection = FACING_RIGHT;

        public float maxDistance {get; set;}
        float currDistance = 0;
        public bool movingLeft = false;
        public bool PlayerOnPlatform = false;


        public MovingPlatform(int column, int row)
            : base(column, row)
        {
            base.Model = GameplayScreen._models["BasicBlock"];
            base.isCollidable = true;
            maxDistance = 2 * GameConstants.SINGLE_CELL_SIZE; //default distance

            // Provides a hitbox for the block - Steven
            UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position), false, false);
        }

        public override void Update(RenderContext renderContext)
        {
            CheckEnemyBoxCollision(renderContext);
            if (currDistance > maxDistance)
            {
                movingLeft = !movingLeft;
                currDistance = 0;
            }
            base.Update(renderContext);
        }

        public void MoveLeft(RenderContext renderContext, float movementAmount)
        {
            facingDirection = FACING_LEFT;
            Rotate(0f, -90f, 0f);
            Vector3 newPosition = Position + new Vector3(-movementAmount, 0, 0);
            currDistance += movementAmount;
            Translate(newPosition);

            //if player is on the platform, move the player just as much as the platform does
            if (PlayerOnPlatform)
            {
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
                        currDistance = 0;
                    }
                    if (wy > -hx)
                    {
                        //boxHitState = "Box Right";// right
                        movingLeft = true;
                        currDistance = 0;
                    }



                }

            }
        }
    }
}



