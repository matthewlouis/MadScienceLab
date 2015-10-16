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

namespace MadScienceLab
{
    public class Character: CellObject
    {
        GameAnimatedModel charModel;

        //properties used for picking up a box
        public CellObject adjacentObj { get; set; }
        public PickableBox storedBox { get; set; }
        public int pickUpState = 0;
        public int pickUpAnimationAngle = 0;

        public int putDownAnimationAngle = 90;
        byte facingDirection = FACING_RIGHT;
        byte putFacingDirection = FACING_RIGHT; //direction when the player puts down the box
        const int FACING_LEFT = 1, FACING_RIGHT = 2;

        public Character(int startRow, int startCol):base(startRow, startCol)
        {
            charModel = new GameAnimatedModel("Vampire", startRow, startCol);
            charModel.VerticalOffset = 22;
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager)
        {
            base.LoadContent(contentManager);
            charModel.LoadContent(contentManager);
            charModel.PlayAnimation("Idle");
        }

        
        public override void Update(RenderContext renderContext)
        {
            charModel.Update(renderContext);

            PickBox();
            PutBox();

            base.Update(renderContext);
        }

        public override void Draw(RenderContext renderContext)
        {
            charModel.Draw(renderContext);
        }

        public void MoveLeft(float movementAmount)
        {
            facingDirection = FACING_LEFT;
            Rotate(0f, -90f, 0f);
            charModel.PlayAnimation("Run");
            Vector3 newPosition = Position + new Vector3(-movementAmount, 0, 0);
            Translate(newPosition);
        }

        public void MoveRight(float movementAmount)
        {
            facingDirection = FACING_RIGHT;
            Rotate(0f, 90f, 0f);
            charModel.PlayAnimation("Run");
            Vector3 newPosition = Position + new Vector3(movementAmount,0, 0);
            Translate(newPosition);
        }


        public void PutBox()
        {
            if (pickUpState == 4)
            {
                putFacingDirection = facingDirection; //set the direction the character is facing at the point the character begins putting down the box
                pickUpState = 5;
            }
            if (pickUpState == 5)
            {
                if (putDownAnimationAngle > 0 && putDownAnimationAngle < 180)
                {
                    if (putFacingDirection == FACING_RIGHT)
                        putDownAnimationAngle -= 9;
                    else //if(putFacingDirection == FACING_LEFT)
                        putDownAnimationAngle += 9;

                    float angleRad = putDownAnimationAngle * 2 * (float)Math.PI / 360;
                    storedBox.Position = Position + new Vector3(Hitbox.Width * (float)Math.Cos(angleRad), Hitbox.Height * (float)Math.Sin(angleRad), 0f);
                    storedBox.Hitbox = new Rectangle((int)(Hitbox.Location.X + Hitbox.Width * Math.Cos(angleRad)),
                                                     (int)(Hitbox.Location.Y + Hitbox.Height * Math.Sin(angleRad)),
                                                     storedBox.Hitbox.Width, storedBox.Hitbox.Height);

                }
                else
                {
                    pickUpState = 0;
                    //remove storedBox from player
                    storedBox.isCollidable = true;
                    storedBox = null;
                }
            }
        }

        public void PickBox()
        {

            if (pickUpState == 1) //determine initial pickup animation angle
            {
                if (storedBox.Position.X < Position.X)
                    pickUpAnimationAngle = 180;
                else
                    pickUpAnimationAngle = 0;
                pickUpState = 2;
            }
            if (pickUpState == 2) //update box position
            {
                if (pickUpAnimationAngle != 90)
                { //endpoint is 90
                    if (pickUpAnimationAngle > 90)
                    {
                        pickUpAnimationAngle -= 9; //from left
                    }
                    else
                        pickUpAnimationAngle += 9; //from right

                    float angleRad = pickUpAnimationAngle * 2 * (float)Math.PI / 360;
                    storedBox.Position = Position + new Vector3(Hitbox.Width * (float)Math.Cos(angleRad), Hitbox.Height * (float)Math.Sin(angleRad), 0f);
                    storedBox.Hitbox = new Rectangle((int)(Hitbox.Location.X + Hitbox.Width * Math.Cos(angleRad)),
                                                     (int)(Hitbox.Location.Y + Hitbox.Height * Math.Sin(angleRad)),
                                                     storedBox.Hitbox.Width, storedBox.Hitbox.Height);
                    // Position + new Vector3(Hitbox.Width * (float)Math.Cos(angleRad), Hitbox.Height * (float)Math.Sin(angleRad), 0f);
                    //if (storedBox.Position.X > Position.X)
                    //    storedBox.Position += new Vector3(-Hitbox.Width / 10, Hitbox.Height / 10, 0);

                    putDownAnimationAngle = 90; 
                }
                else
                {
                    pickUpState = 3;
                }
            }
            else if (pickUpState == 3)
            {
                storedBox.Position = Position + new Vector3(0, Hitbox.Height, 0);
                storedBox.Hitbox = new Rectangle(Hitbox.Location.X, Hitbox.Location.Y + Hitbox.Height,
                                                 storedBox.Hitbox.Width, storedBox.Hitbox.Height);
            }
 
        }

        public void Stop()
        {
            charModel.PlayAnimation("Idle");
        }
    }
}
