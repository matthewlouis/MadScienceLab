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
        const int FACING_LEFT = 1, FACING_RIGHT = 2;
        byte facingDirection = FACING_RIGHT;

        float maxDistance = 4 * GameConstants.SINGLE_CELL_SIZE;
        float currDistance = 0;
        public bool movingLeft = false;

        public MovingPlatform(int column, int row)
            : base(column, row)
        {
            base.Model = Game1._models["BasicBlock"];
            base.isCollidable = true;

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

        public void MoveLeft(float movementAmount)
        {
            facingDirection = FACING_LEFT;
            Rotate(0f, -90f, 0f);
            Vector3 newPosition = Position + new Vector3(-movementAmount, 0, 0);
            currDistance += movementAmount;
            Translate(newPosition);
        }

        public void MoveRight(float movementAmount)
        {
            facingDirection = FACING_RIGHT;
            Rotate(0f, 90f, 0f);
            Vector3 newPosition = Position + new Vector3(movementAmount, 0, 0);
            currDistance += movementAmount;
            Translate(newPosition);
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
                        MoveLeft(GameConstants.MOVEAMOUNT);

                    }
                    else
                    {
                        MoveRight(GameConstants.MOVEAMOUNT);
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



