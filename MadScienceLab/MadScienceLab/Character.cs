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

namespace MadScienceLab
{
    public class Character: CellObject
    {
        public enum InteractState { HandsEmpty = 0, JustPickedUpBox = 1, AnimatingPickup = 2, CompletedPickup = 3, 
                                    StartingDropBox = 4, AnimatingDropBox = 5};

        GameAnimatedModel charModel;

        //properties used for picking up a box
        public CellObject AdjacentObj { get; set; }
        public PickableBox StoredBox { get; set; }
        public InteractState interactState = InteractState.HandsEmpty;
        public int pickUpAnimationAngle = 0;

        public int putDownAnimationAngle = 90;
        byte facingDirection = FACING_RIGHT;
        byte putFacingDirection = FACING_RIGHT; //direction when the player puts down the box
        const int FACING_LEFT = 1, FACING_RIGHT = 2;

        //For controls - stores previous state.
        GamePadState oldGamePadState;
        KeyboardState oldKeyboardState;

        private bool jumping;
        private Boolean collisionJumping = false;


        public Character(int startRow, int startCol):base(startRow, startCol)
        {
            // create model with offset of position
            charModel = new GameAnimatedModel("Vampire", startRow, startCol);
            charModel.VerticalOffset = 22;         
            Rotate(0f, 90f, 0f);
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager)
        {
            base.LoadContent(contentManager);
            charModel.LoadContent(contentManager);
            charModel.PlayAnimation("Idle");
            UpdateBoundingBox(charModel.Model, Matrix.CreateTranslation(charModel.Position), true, false);
            // Overriding the hitbox size - Steven
            base.Width = 48;
            base.Height = 48;
        }

        
        public override void Update(RenderContext renderContext)
        {
            
            charModel.Update(renderContext);
            UpdatePhysics();
            CheckPlayerBoxCollision(renderContext);

            HandleInput();
            if (TransVelocity.Y >= 0)
                collisionJumping = true;
            else
                collisionJumping = false;

            // Allows for one jump and prevents jumping when falling off a brick - Steven
            if (TransVelocity.Y != 0)
                jumping = true;


            /*
            if (DebugCheckPlayerBoxCollision() && !collisionJumping)
            {
                Position = new Vector3((int)Position.X, brick.Top + GameConstants.SINGLE_CELL_SIZE - 1, 0);
                TransVelocity = Vector3.Zero;
                jumping = false;
            }*/

            


            PickBox();
            PutBox();

            base.Update(renderContext);
        }

        private void HandleInput()
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            GamePadState currentGamePadState = GamePad.GetState(PlayerIndex.One);


            //Setting up basic controls

            // Jumping on keyboard Space or gamepad A button
            if (!jumping && 
                ((currentKeyboardState.IsKeyDown(Keys.Space) &&
                oldKeyboardState.IsKeyUp(Keys.Space)) || 
                (currentGamePadState.Buttons.A == ButtonState.Pressed &&
                oldGamePadState.Buttons.A != ButtonState.Pressed)))
            {
                Jumping();
            }

            if ((currentKeyboardState.IsKeyDown(Keys.F) &&
                oldKeyboardState.IsKeyUp(Keys.F)) || 
                (currentGamePadState.Buttons.B == ButtonState.Pressed &&
                oldGamePadState.Buttons.B != ButtonState.Pressed))
            {
                if (interactState == Character.InteractState.CompletedPickup)
                {
                    PutBox();
                }
                else
                    InteractWithObject();  
                //handle pick up box
                }
            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                MoveLeft(GameConstants.MOVEAMOUNT);
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                MoveRight(GameConstants.MOVEAMOUNT);
            }
            else
            {
                Stop();
            }

            oldKeyboardState = currentKeyboardState;
            oldGamePadState = currentGamePadState;
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

        public void Jumping()
        {
            //handle jump movement
            //Added a bit of physics to this.
            jumping = true;
            base.TransVelocity += new Vector3(0, GameConstants.SINGLE_CELL_SIZE * 10, 0);
            charModel.PlayAnimation("Jump",false, 0.2f);
        }
        public void PutBox()
        {
            if (interactState == InteractState.StartingDropBox)
            {
                putFacingDirection = facingDirection; //set the direction the character is facing at the point the character begins putting down the box
                interactState = InteractState.AnimatingDropBox;
            }
            if (interactState == InteractState.AnimatingDropBox)
            {
                if (putDownAnimationAngle > 0 && putDownAnimationAngle < 180)
                {
                    if (putFacingDirection == FACING_RIGHT)
                        putDownAnimationAngle -= 9;
                    else //if(putFacingDirection == FACING_LEFT)
                        putDownAnimationAngle += 9;

                    float angleRad = putDownAnimationAngle * 2 * (float)Math.PI / 360;
                    StoredBox.Position = Position + new Vector3(Hitbox.Width * (float)Math.Cos(angleRad), Hitbox.Height * (float)Math.Sin(angleRad), 0f);
                }
                else
                {
                    interactState = InteractState.HandsEmpty;
                    //remove storedBox from player
                    StoredBox.isCollidable = true;
                    StoredBox = null;
                }
            }
        }

        public void PickBox()
        {

            if (interactState == InteractState.JustPickedUpBox) //determine initial pickup animation angle
            {
                if (StoredBox.Position.X < Position.X)
                    pickUpAnimationAngle = 180;
                else
                    pickUpAnimationAngle = 0;
                interactState = InteractState.AnimatingPickup;
            }
            if (interactState == InteractState.AnimatingPickup) //update box position
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
                    StoredBox.Position = Position + new Vector3(Hitbox.Width * (float)Math.Cos(angleRad), Hitbox.Height * (float)Math.Sin(angleRad), 0f);
                    // Position + new Vector3(Hitbox.Width * (float)Math.Cos(angleRad), Hitbox.Height * (float)Math.Sin(angleRad), 0f);
                    //if (storedBox.Position.X > Position.X)
                    //    storedBox.Position += new Vector3(-Hitbox.Width / 10, Hitbox.Height / 10, 0);

                    putDownAnimationAngle = 90; 
                }
                else
                {
                    interactState = InteractState.CompletedPickup;
                }
            }
            else if (interactState == InteractState.CompletedPickup)
            {
                StoredBox.Position = Position + new Vector3(0, Hitbox.Height, 0);
            }
 
        }



        public void Stop()
        {
            charModel.PlayAnimation("Idle");
        }

        public void InteractWithObject()
        {
            if (interactState == Character.InteractState.HandsEmpty && AdjacentObj != null)
            {
                if (AdjacentObj.GetType() == typeof(PickableBox))
                {
                    interactState = Character.InteractState.JustPickedUpBox;
                    StoredBox = (PickableBox)AdjacentObj;
                    StoredBox.isCollidable = false;
                }
                else if (AdjacentObj.GetType() == typeof(Switch))
                {
                    Switch currentSwitch = (Switch)AdjacentObj;
                    currentSwitch.FlickSwitch();
                }
            }
        }

        /// <summary>
        /// This updates the physics of te player.
        /// </summary>
        /// <param name="Object"></param>
        public void UpdatePhysics()
        {
            TransVelocity += TransAccel / 60; //amt. accel (where TransAccel is in seconds) per frame ...
            Translate(Position + TransVelocity / 60);
        }

        private void CheckPlayerBoxCollision(RenderContext renderContext)
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

                    Button tmpButton = levelObject as Button;
                    if (tmpButton != null) //if it is a button
                    {
                        Console.Out.WriteLine("Pressed");
                        Button button = (Button)levelObject as Button;
                        button.IsPressed = true;
                    }
                    if (wy > hx)
                    {
                        if (wy > -hx)
                        {
                            //boxHitState = "Box Top";//top
                            Position = new Vector3((int)Position.X, (int)Position.Y - 1, 0);
                            TransVelocity = Vector3.Zero;
                        }
                        else
                        {
                            //boxHitState = "Box Left";// left
                            Position = new Vector3(levelObject.Hitbox.Right + 1, (int)Position.Y, 0);
                            AdjacentObj = levelObject;
                        }
                    }
                    else
                    {
                        if (wy > -hx)
                        {
                            //boxHitState = "Box Right";// right

                            Position = new Vector3(levelObject.Hitbox.Left - GameConstants.SINGLE_CELL_SIZE, (int)Position.Y, 0);

                            Position = new Vector3(levelObject.Hitbox.Left - Width, (int)Position.Y, 0);

                            AdjacentObj = levelObject;
                        }
                        else
                        {
                            Position = new Vector3((int)Position.X, (int)levelObject.Hitbox.Bottom - 1, 0);
                            if (!collisionJumping)
                                TransVelocity = Vector3.Zero;
                            jumping = false;
                        }
                    }
                }
            }
        }

        /*   /// <summary>
   /// Only used for debugging purposes
   /// </summary>
   /// <returns></returns>
   private Boolean DebugCheckPlayerBoxCollision()
   {

       foreach (CellObject brick in basicLevel.Children)
       {
           if (player.Hitbox.Intersects(brick.Hitbox) && brick.isCollidable)
           {
               this.brick = brick.Hitbox;
               return true;
           }
       }
       return false;
   }*/
    }
}
