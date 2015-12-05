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
        public enum InteractState { HandsEmpty = 0, JustPickedUpBox = 1, AnimatingPickup = 2, CompletedPickup = 3, 
                                    StartingDropBox = 4, AnimatingDropBox = 5};

        GameAnimatedModel charModel;

        //For playing sounds
        //Dictionary<string, SoundEffect> soundFX;
        SoundEffectPlayer soundEffects;

        //properties used for picking up a box
        public CellObject InteractiveObj { get; set; }
        public CellObject AdjacentObj { get; set; }
        public PickableBox StoredBox { get; set; }
        public InteractState interactState = InteractState.HandsEmpty;
        public int pickUpAnimationAngle = 0;

        public int putDownAnimationAngle = 90;
        byte facingDirection = FACING_RIGHT;
        public byte GetFacingDirection { get { return facingDirection;  } }
        byte putFacingDirection = FACING_RIGHT; //direction when the player puts down the box
        const int FACING_LEFT = 1, FACING_RIGHT = 2;
        public bool canPlace = true;
        //For controls - stores previous state.
        GamePadState oldGamePadState;
        KeyboardState oldKeyboardState;

        // Jumping support
        public bool jumping;
        private Boolean collisionJumping = false;
        public bool falling;

        //For handling damage
        private static TimeSpan DAMAGE_DELAY = TimeSpan.FromMilliseconds(1000f);
        private static int BLINK_DELAY = 200;
        public float damageDelayTime;

        private TimeSpan timeHit = TimeSpan.Zero;
        private bool damageable = true;
        private bool damageableAnim = true;

        private const float MOVEMENT_ANIM_SPEED = 3f;

       

        // Health Support
        private int health;
        public void TakeDamage(int damage, GameTime gametime)
        {
            if (damageable) //if recently taken damage, don't take damage again
            {
                timeHit = gametime.TotalGameTime; //get time when hit
                health -= damage;
                damageable = false;
                delayDamagable = false;
                soundEffects.PlaySound("PlayerHit");
            }
        }

        private bool delayDamagable = true;

        public float GetTimeScale()
        {
            return 1 - damageDelayTime;
        }

        /// <summary>
        /// Returns the player's damagable state
        /// </summary>
        /// <returns></returns>
        public bool IsInvuln()
        {
            return !delayDamagable;
        }
        public int GetHealth()
        {
            return health;
        }

        private bool mapEnabled = false;
        // Debug map, can be used as a minimap - Steven
        public bool MapEnabled 
        {
            get { return mapEnabled; }
            set { mapEnabled = value; }
        }

        public Character(int startRow, int startCol):base(startRow, startCol)
        {
            Scale(0.06f, 0.06f, 0.06f);
            Rotate(0, 90f, 0);

            // create model with offset of position
            charModel = new GameAnimatedModel("SciTry", startRow, startCol, this);
            charModel.VerticalOffset = 22;         
            health = GameConstants.HEALTH;
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager)
        {
            base.LoadContent(contentManager);
            charModel.LoadContent(contentManager);
            charModel.SetAnimationSpeed(1.0f);
            charModel.PlayAnimation("Idle", true, 0f);
            UpdateBoundingBox(charModel.Model, Matrix.CreateTranslation(charModel.Position), true, true);
            // Overriding the hitbox size, the new model will need to be the height of the cells, for now the vamp model height is overrided - Steven
            base.HitboxWidth = 24;
            HitboxWidthOffset = 12;
            base.HitboxHeight = 48;

            //Load sound effects
            //soundFX = new Dictionary<string, SoundEffect>();
            soundEffects = new SoundEffectPlayer(this);
            soundEffects.LoadSound("BoxDrop", GameplayScreen._sounds["BoxDrop"]);
            soundEffects.LoadSound("BoxPickup", GameplayScreen._sounds["BoxPickup"]);
            soundEffects.LoadSound("Jump", GameplayScreen._sounds["Jump"]);
            soundEffects.LoadSound("Land", GameplayScreen._sounds["Land"]);
            soundEffects.LoadSound("PlayerHit", GameplayScreen._sounds["PlayerHit"]);
            soundEffects.LoadSound("ToggleSwitch", GameplayScreen._sounds["ToggleSwitch"]);

        }

        
        public override void Update(RenderContext renderContext)
        {
            //List<CellObject> returnObjs = new List<CellObject>();

            //renderContext.Quadtree.clear();
            //foreach (CellObject obj in renderContext.Level.collidableObjects)
            //{
            //    renderContext.Quadtree.insert(obj);
            //}

            //renderContext.Quadtree.retrieve(returnObjs, base.Hitbox);

            if (health <= 0)
            {
                renderContext.Level.GameOver = true;
            }

            //For temporary invincibility when recently damaged
            if (!damageable)     
            {
                damageDelayTime = (float)(renderContext.GameTime.TotalGameTime - timeHit).TotalMilliseconds / (float)DAMAGE_DELAY.TotalMilliseconds;
                if ((renderContext.GameTime.TotalGameTime - timeHit) >= DAMAGE_DELAY)
                {
                    damageable = true;
                }
                if ((renderContext.GameTime.TotalGameTime - timeHit) >= DAMAGE_DELAY - TimeSpan.FromMilliseconds(100f))
                {
                    delayDamagable = true;
                }
            }

            charModel.Update(renderContext);
            UpdatePhysics();
           
            // Quad tree collision
            //foreach (CellObject worldObject in returnObjs)
            //{
            //    if (interactState == InteractState.CompletedPickup) // Start checking for collisions for the box being carried - Steven
            //    {
            //        CheckBoxCarryCollision(renderContext, worldObject);
            //    }
            //    CheckPlayerBoxCollision(renderContext, worldObject);
            //}

            if (interactState == InteractState.CompletedPickup) // Start checking for collisions for the box being carried - Steven
            {
                CheckBoxCarryCollision(renderContext);
            }
            CheckPlayerBoxCollision(renderContext);   
            CheckPickableBoxVincity(renderContext);

            HandleInput();
            if (TransVelocity.Y >= 0)
                collisionJumping = true;
            else
                collisionJumping = false;

            // Allows for one jump and prevents jumping when falling off a brick - Steven
            //if (TransVelocity.Y != 0)
            //    jumping = true;

            if (TransVelocity.Y != 0)
                falling = true;

            //Ensures we're checking what the player is in front of each frame
            //InteractiveObj = null;

            //Code used to update any actions occurring with PickBox and PutBox.
            UpdatePickBox ();
            UpdatePutBox (renderContext);
            //update sound
            soundEffects.Update(renderContext);
            base.Update(renderContext);
        }

        private void HandleInput()
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            GamePadState currentGamePadState = GamePad.GetState(PlayerIndex.One);


            //Setting up basic controls

            // Jumping on keyboard Space or gamepad A button
            if (
                !jumping && !falling &&
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
                charModel.SetAnimationSpeed(MOVEMENT_ANIM_SPEED);
                if (interactState == Character.InteractState.CompletedPickup)
                {
                    PutBox();
                    charModel.PlayAnimation("DropBox", false, 0f);
                }
                else
                {
                    PickBox();
                    charModel.PlayAnimation("PickBox", false, 0f);
                }
            }

            //prevent the player from moving while still in box pickup/putdown animation
            bool NotActiveWithBox = interactState == InteractState.CompletedPickup || interactState == InteractState.HandsEmpty;
            if ((currentKeyboardState.IsKeyDown ( Keys.Left ) || 
                currentGamePadState.IsButtonDown(Buttons.DPadLeft) ||
                currentGamePadState.IsButtonDown(Buttons.LeftThumbstickLeft)) && NotActiveWithBox)
            {
                MoveLeft(GameConstants.MOVEAMOUNT);
            }
            else if ((currentKeyboardState.IsKeyDown ( Keys.Right ) || 
                currentGamePadState.IsButtonDown(Buttons.DPadRight) ||
                currentGamePadState.IsButtonDown(Buttons.LeftThumbstickRight)) && NotActiveWithBox)
            {
                MoveRight(GameConstants.MOVEAMOUNT);
            }
            else
            {
                Stop();
            }


            if (currentKeyboardState.IsKeyDown(Keys.M) && oldKeyboardState.IsKeyUp(Keys.M))
            {
                MapEnabled = !MapEnabled;
            }
            oldKeyboardState = currentKeyboardState;
            oldGamePadState = currentGamePadState;
        }


        //public override Rectangle Hitbox
        //{
        //    get
        //    {
        //        if (interactState == InteractState.CompletedPickup) //new hitbox if currently carrying a box
        //        {
        //            return new Rectangle((int)StoredBox.Position.X, (int)Position.Y, HitboxWidth, HitboxHeight + StoredBox.HitboxHeight);
        //        }
        //        return base.Hitbox;
        //    }
        //}


        public Rectangle CharacterHitbox
        {
            get
            {
                return base.Hitbox;
            }
        }

        public override void Draw(RenderContext renderContext)
        {
            //Create blink effect
            if (!damageable &&
                (renderContext.GameTime.TotalGameTime - timeHit).Milliseconds % BLINK_DELAY <= BLINK_DELAY/2)
            {
                return; //don't draw the player 
            }

            charModel.Draw(renderContext);
        }

        public void MoveLeft(float movementAmount)
        {
            facingDirection = FACING_LEFT;
            Rotate(0f, -90f, 0f);
            if (!jumping) //don't play run animation if jumping
            {
                charModel.SetAnimationSpeed(MOVEMENT_ANIM_SPEED);
                //if player's hands are empty, play regular run, else he's holding a box
                if(interactState == Character.InteractState.HandsEmpty)
                    charModel.PlayAnimation("Run", true, 0.2f);
                else
                    charModel.PlayAnimation("RunBox", true, 0.2f);
            }
            Vector3 newPosition = Position + new Vector3(-movementAmount, 0, 0);
            Translate(newPosition);
        }

        public void MoveRight(float movementAmount)
        {
            facingDirection = FACING_RIGHT;
            Rotate(0f, 90f, 0f);
            if (!jumping) //don't play run animation if jumping
            {
                charModel.SetAnimationSpeed(MOVEMENT_ANIM_SPEED);
                //if player's hands are empty, play regular run, else he's holding a box
                if (interactState == Character.InteractState.HandsEmpty)
                    charModel.PlayAnimation("Run", true, 0.2f);
                else
                    charModel.PlayAnimation("RunBox", true, 0.2f);
            }
            Vector3 newPosition = Position + new Vector3(movementAmount,0, 0);
            Translate(newPosition);
        }

        public void Jumping()
        {
            //handle jump movement
            //Added a bit of physics to this.
            TransVelocity = Vector3.Zero;

            jumping = true;
            base.TransVelocity += new Vector3(0, GameConstants.SINGLE_CELL_SIZE*5, 0);
            charModel.SetAnimationSpeed(MOVEMENT_ANIM_SPEED);
            //play corresponding animation if character is holding a box
            if(interactState == Character.InteractState.HandsEmpty)
                charModel.PlayAnimation("Jump",false, 0.2f);
            else
                charModel.PlayAnimation("JumpBox", false, 0.2f);
            soundEffects.PlaySound("Jump");
        }
        public void PutBox()
        {
            //if (/*player.adjacentObj == null*/) //will need a condition for when the adjacent area where the player would be trying to put the box is empty,
            //{
            float sideXPos;
            float leeway = 10; //leeway allowed to place the box away from you (ie. amount allowed to place a box into another object)
            if (facingDirection == Character.FACING_RIGHT)
            {
                sideXPos = Position.X + Hitbox.Width;
            }
            else //facing left
            {
                sideXPos = Position.X - StoredBox.Hitbox.Width + leeway * 3;
            }
            Rectangle areaSide = new Rectangle((int)sideXPos, (int)Position.Y + 2, (int)StoredBox.Hitbox.Width - (int)leeway, (int)StoredBox.Hitbox.Height);
            bool putdownable = true;
            foreach (CellObject levelObject in GameplayScreen.CurrentLevel.Children) //check to see if it has collision with anything
            {
                if (levelObject.isCollidable && areaSide.Intersects(levelObject.Hitbox) && levelObject.GetType() != typeof(MessageEvent))
                {
                    putdownable = false;
                }
                /*
                 +		areaSide	{X:-92 Y:-313 Width:38 Height:38}	Microsoft.Xna.Framework.Rectangle £¨-313 £¨top£©to -275 (bot)£©
                 +		Hitbox	{X:-112 Y:-360 Width:48 Height:48}	Microsoft.Xna.Framework.Rectangle -360 (top£© to -312 (bot)
                        So, the hitboxes (rectangles) are actually upside down - 'bot' is actually the top, 'top' is the bottom,
                 *      height increases upwards.
                 */
            }
            if (jumping || falling) //disallow putting down when jumping
                putdownable = false;
            if (putdownable)
                interactState = InteractState.StartingDropBox; //state for while the player begins putting down the box
            //}
        }

        public void PickBox()
        {
            if (interactState == InteractState.HandsEmpty/*state 0*/ /*&& !jumping && !falling*/) //Jacob: re-allowing pickup while in the air
            {
                if (AdjacentObj != null && AdjacentObj.GetType() == typeof(PickableBox) && (((PickableBox)(AdjacentObj)).IsLiftable))
                {
                    //check if there is area above the player to pick up the box
                    Rectangle areaTop = new Rectangle ( (int)Position.X, CharacterHitbox.Bottom + 1, (int)(AdjacentObj.Hitbox.Width), (int)(AdjacentObj.Hitbox.Height) );
                    bool pickuppable = true;

                    // to pick up boxes from under the other this has been commented out
                    //foreach (CellObject levelObject in GameplayScreen.CurrentLevel.Children) //check to see if it has collision with anything
                    //{
                    //    if (levelObject.isCollidable && areaTop.Intersects ( levelObject.Hitbox ))
                    //    {
                    //        pickuppable = false;
                    //    }
                    //    /*
                    //     +		The hitboxes (rectangles) are actually upside down - 'bot' is actually the top, 'top' is the bottom,
                    //     *      height increases upwards.
                    //     */
                    //}
                    //if (jumping || falling) //disallow putting down when jumping
                    //    pickuppable = false;
                    if (pickuppable) {
                        interactState = InteractState.JustPickedUpBox; //state 1
                        StoredBox = (PickableBox)AdjacentObj;
                        StoredBox.isCollidable = false;
                        soundEffects.PlaySound("BoxPickup");
                    }
                }
                else if (InteractiveObj != null && InteractiveObj.GetType() == typeof(ToggleSwitch))
                {
                    ToggleSwitch currentSwitch = (ToggleSwitch)InteractiveObj;
                    if (currentSwitch.IsToggleable && currentSwitch.IsReady)
                    {
                        soundEffects.PlaySound ( "ToggleSwitch" );
                        currentSwitch.FlickSwitch ();
                    }
                }
            }     
        }
        /// <summary>
        /// Code to update any animations occurring with PickBox.
        /// </summary>
        public void UpdatePickBox ()
        {
            if (interactState == InteractState.JustPickedUpBox) //determine initial pickup animation angle
            {
                if (StoredBox.Position.X < Position.X)
                    pickUpAnimationAngle = 180;
                else
                    pickUpAnimationAngle = 0;
                interactState = InteractState.AnimatingPickup; //state 2
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
                    StoredBox.Position = Position + new Vector3 ( CharacterHitbox.Width * (float)Math.Cos ( angleRad ), CharacterHitbox.Height * (float)Math.Sin ( angleRad ), 0f );

                    //no need to update hitbox anymore
                    /*StoredBox.Hitbox = new Rectangle ( (int)(Hitbox.Location.X + Hitbox.Width * Math.Cos ( angleRad )),
                                                     (int)(Hitbox.Location.Y + Hitbox.Height * Math.Sin ( angleRad )),
                                                     StoredBox.Hitbox.Width, StoredBox.Hitbox.Height );*/
                    // Position + new Vector3(Hitbox.Width * (float)Math.Cos(angleRad), Hitbox.Height * (float)Math.Sin(angleRad), 0f);
                    //if (storedBox.Position.X > Position.X)
                    //    storedBox.Position += new Vector3(-Hitbox.Width / 10, Hitbox.Height / 10, 0);

                    putDownAnimationAngle = 90;
                }
                else
                {
                    interactState = InteractState.CompletedPickup; //state 3
                }
            }
            else if (interactState == InteractState.CompletedPickup) //state 3s
            {
                StoredBox.Position = Position + new Vector3 ( 0, CharacterHitbox.Height, 0 );
                //no need to update hitbox anymore
                /*StoredBox.Hitbox = new Rectangle ( Hitbox.Location.X, Hitbox.Location.Y + Hitbox.Height,
                                                 storedBox.Hitbox.Width, storedBox.Hitbox.Height );*/
            }

        }
        /// <summary>
        /// Code to update any animations occurring with PutBox.
        /// </summary>
        /// <param name="renderContext"></param>
        public void UpdatePutBox (RenderContext renderContext) //need renderContext to access level for collision checking
        {
            if (interactState == InteractState.StartingDropBox) //state 4
            {
                putFacingDirection = facingDirection; //set the direction the character is facing at the point the character begins putting down the box
                interactState = InteractState.AnimatingDropBox;
            }
            if (interactState == InteractState.AnimatingDropBox) //animation state
            {
                if (putDownAnimationAngle > 0 && putDownAnimationAngle < 180)
                {
                    if (putFacingDirection == FACING_RIGHT)
                        putDownAnimationAngle -= 9;
                    else //if(putFacingDirection == FACING_LEFT)
                        putDownAnimationAngle += 9;

                    float angleRad = putDownAnimationAngle * 2 * (float)Math.PI / 360;
                    StoredBox.Position = Position + new Vector3 ( CharacterHitbox.Width * (float)Math.Cos ( angleRad ), CharacterHitbox.Height * (float)Math.Sin ( angleRad ), 0f );
                    
                    //no need to update hitbox
                    /*
                    storedBox.Hitbox = new Rectangle ( (int)(Hitbox.Location.X + Hitbox.Width * Math.Cos ( angleRad )),
                                                     (int)(Hitbox.Location.Y + Hitbox.Height * Math.Sin ( angleRad )),
                                                     storedBox.Hitbox.Width, storedBox.Hitbox.Height );
                    */

                }
                else
                {
                    interactState = InteractState.HandsEmpty;
                    StoredBox.TransVelocity = Vector3.Zero;
                    float startX = (GameConstants.SINGLE_CELL_SIZE * 1) - (GameConstants.X_RESOLUTION / 2);
                    float startY = (GameConstants.SINGLE_CELL_SIZE * 1) - (GameConstants.Y_RESOLUTION / 2);
                    Vector3 CELLREMAINDER = new Vector3 ( (StoredBox.Position.X - startX) % GameConstants.SINGLE_CELL_SIZE,
                                                        (StoredBox.Position.Y - startY) % GameConstants.SINGLE_CELL_SIZE,
                                                        StoredBox.Position.Z );
                    //Move positions to the nearest cell

                    if (CELLREMAINDER.X < GameConstants.SINGLE_CELL_SIZE / 2)
                        StoredBox.Position = new Vector3 ( StoredBox.Position.X - CELLREMAINDER.X, StoredBox.Position.Y, StoredBox.Position.Z );
                    else
                        StoredBox.Position = new Vector3 ( StoredBox.Position.X - CELLREMAINDER.X + GameConstants.SINGLE_CELL_SIZE, StoredBox.Position.Y, StoredBox.Position.Z );
                    /*if (CELLREMAINDER.Y < Game1.SINGLE_CELL_SIZE / 2)
                        storedBox.Position = new Vector3(storedBox.Position.X, storedBox.Position.Y - CELLREMAINDER.Y, storedBox.Position.Z);
                    else
                        storedBox.Position = new Vector3(storedBox.Position.X, storedBox.Position.Y + Game1.SINGLE_CELL_SIZE - CELLREMAINDER.Y, storedBox.Position.Z);
                    if (CELLREMAINDER.Z < Game1.SINGLE_CELL_SIZE / 2)
                        storedBox.Position = new Vector3(storedBox.Position.X, storedBox.Position.Y, storedBox.Position.Z - CELLREMAINDER.Z);
                    else
                        storedBox.Position = new Vector3(storedBox.Position.X, storedBox.Position.Y, storedBox.Position.Z + Game1.SINGLE_CELL_SIZE - CELLREMAINDER.Z);*/


                    //quantize position to a multiple of SINGLE_CELL_SIZE
                    StoredBox.CheckCollision ( renderContext.Level ); //check and update box position based on collision before updating any other boxes' positions
                    //remove storedBox from player
                    StoredBox.isCollidable = true;
                    StoredBox = null;
                    soundEffects.PlaySound("BoxDrop");
                }
            }
        }

        public void Stop()
        {
            charModel.SetAnimationSpeed(1.0f);
            if(interactState == Character.InteractState.HandsEmpty)
                charModel.PlayAnimation("Idle", true, 0.2f);
            else
                charModel.PlayAnimation("IdleBox", true, 0.2f);
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

        /// <summary>
        /// Checks all pickable boxes to see if player is close to the boxes - Steven
        /// </summary>
        /// <param name="renderContext"></param>
        private void CheckPickableBoxVincity(RenderContext renderContext)
        {
            if (!jumping || !falling)
            {
                foreach (CellObject worldObject in renderContext.Level.gameObjects[typeof(PickableBox)])
                {
                    // Expanding the hitbox to see if player is near the box and flatten it to prevent from being picked up from 1 row above or below
                    Rectangle expandedHitbox = worldObject.Hitbox;
                    expandedHitbox.X -= 15;
                    expandedHitbox.Width += 30;
                    expandedHitbox.Y += 5;
                    expandedHitbox.Height -= 30;
                    if (Hitbox.Intersects(expandedHitbox))
                    {
                        if (Hitbox.Center.X < expandedHitbox.Center.X && facingDirection == FACING_RIGHT)
                        {
                            AdjacentObj = worldObject;
                        }
                        else if (Hitbox.Center.X > expandedHitbox.Center.X && facingDirection == FACING_LEFT)
                        {
                            AdjacentObj = worldObject;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks the collisions for the box that the player is carrying seperately
        /// Expanding the player hitbox to be larger than the blocks cause collision issues
        /// - Steven
        /// </summary>
        /// <param name="renderContext"></param>
        private void CheckBoxCarryCollision(RenderContext renderContext)
        {
            foreach (CellObject levelObject in renderContext.Level.collidableObjects)
            {
                float sideXPos;
                float leeway = 10; //leeway allowed to place the box away from you (ie. amount allowed to place a box into another object)
                if (facingDirection == Character.FACING_RIGHT)
                {
                    sideXPos = Position.X + Hitbox.Width;
                }
                else //facing left
                {
                    sideXPos = Position.X - StoredBox.Hitbox.Width + leeway * 3;
                }
                Rectangle areaSide = new Rectangle((int)sideXPos, (int)Position.Y + 2, (int)StoredBox.Hitbox.Width - 10, (int)StoredBox.Hitbox.Height);

                if (levelObject.isCollidable && areaSide.Intersects(levelObject.Hitbox) && levelObject.GetType() != typeof(MessageEvent))
                {
                    canPlace = false;
                }

                if (levelObject.isCollidable && StoredBox.Hitbox.Intersects(levelObject.Hitbox))
                {
                    /**Determining what side was hit**/
                    float wy = (levelObject.Hitbox.Width + Hitbox.Width)
                             * (levelObject.Hitbox.Center.Y - StoredBox.Hitbox.Center.Y);
                    float hx = (Hitbox.Height + levelObject.Hitbox.Height)
                             * (levelObject.Hitbox.Center.X - StoredBox.Hitbox.Center.X);

                    Button tmpButton = levelObject as Button;
                    if (tmpButton != null) //if it is a button
                    {
                        Button button = (Button)levelObject as Button;
                        button.IsPressed = true;
                    }

                    if (!levelObject.IsPassable) //if object is not passable, handle physics issues:
                    {
                        if (wy > hx)
                        {
                            if (wy > -hx)
                            {
                                //boxHitState = "Box Top";//top
                                Position = new Vector3(Position.X, levelObject.Hitbox.Top - this.Hitbox.Height - StoredBox.Hitbox.Height - 1, 0); //clip to the top of the colliding object
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
                                Position = new Vector3(levelObject.Hitbox.Left - StoredBox.Hitbox.Width, (int)Position.Y, 0);
                                AdjacentObj = levelObject;
                            }
                        }
                    }
                    else
                    {
                        InteractiveObj = levelObject;
                    }
                }
            }
        }

        /// <summary>
        /// Checks for player collision with all collidable objects in the level - Steven
        /// </summary>
        /// <param name="renderContext"></param>
        private void CheckPlayerBoxCollision(RenderContext renderContext)
        {

            foreach (CellObject levelObject in renderContext.Level.collidableObjects)
            {
                if (levelObject.GetType() == typeof(MovingPlatform)) //default moving platforms for player to not be on the platform unless it would be found that the player were on it
                {
                    ((MovingPlatform)levelObject).PlayerOnPlatform = false;
                }
                if (levelObject.isCollidable && Hitbox.Intersects(levelObject.Hitbox))
                {
                    //renderContext.Boxhit = levelObject.Hitbox;
                    //For presentation: If Exit, display end of level text...will need to refactor to Level class later. - Matt
                    if (levelObject.GetType() == typeof(ExitBlock))
                    {
                        renderContext.Level.LevelOver = true;
                    }

                    //Trigger MessageEvents if passed over
                    if (levelObject.GetType() == typeof(MessageEvent))
                    {
                        MessageEvent msgEvent = (MessageEvent)levelObject as MessageEvent;
                        renderContext.CurrMsgEvent = msgEvent;
                        if (msgEvent.typingState == GameConstants.TYPING_STATE.NotTyped)
                            msgEvent.StartTyping ();
                    }

                    /**Determining what side was hit**/
                    float wy = (levelObject.Hitbox.Width + Hitbox.Width)
                             * (levelObject.Hitbox.Center.Y - Hitbox.Center.Y);
                    float hx = (Hitbox.Height + levelObject.Hitbox.Height)
                             * (levelObject.Hitbox.Center.X - Hitbox.Center.X);

                    if (levelObject.GetType() == typeof(Button)) //if it is a button
                    {
                        Button button = (Button)levelObject as Button;
                        button.IsPressed = true;
                    }

                    if (!levelObject.IsPassable) //if object is not passable, handle physics issues:
                    {
                        if (wy > hx)
                        {
                            if (wy > -hx)
                            {
                                //boxHitState = "Box Top";//top
                                if (Rectangle.Intersect(levelObject.Hitbox, Hitbox).Width > 2) 
                                {
                                    Position = new Vector3((int)Position.X, (int)Position.Y - 1, 0);
                                    TransVelocity = Vector3.Zero;
                                }
                            }
                            else
                            {
                                //boxHitState = "Box Left";// left
                                Position = new Vector3(levelObject.Hitbox.Right + 1 - HitboxWidthOffset, (int)Position.Y, 0);
                                AdjacentObj = levelObject;
                            }
                        }
                        else
                        {
                            if (wy > -hx)
                            {
                                //boxHitState = "Box Right";// right
                                Position = new Vector3(levelObject.Hitbox.Left - HitboxWidth - HitboxWidthOffset, (int)Position.Y, 0);
                                AdjacentObj = levelObject;
                            }
                            else
                            {
                                if (levelObject.GetType() == typeof(MovingPlatform))
                                {
                                    ((MovingPlatform)levelObject).PlayerOnPlatform = true;
                                }

                                if (levelObject.Hitbox.Y > -25)
                                {
                                    Position = new Vector3((int)Position.X, (int)levelObject.Hitbox.Bottom, 0);

                                    if (!(Rectangle.Intersect(levelObject.Hitbox, Hitbox).Width == 2 && levelObject.GetType() == typeof(PickableBox)))
                                    {
                                        TransVelocity = Vector3.Zero;
                                    }
                                }
                                else
                                {
                                    Position = new Vector3((int)Position.X, (int)levelObject.Hitbox.Bottom - 1, 0);

                                    if (!(Rectangle.Intersect(levelObject.Hitbox, Hitbox).Width == 2 && levelObject.GetType() == typeof(PickableBox)))
                                    {
                                        TransVelocity = Vector3.Zero;
                                    }
                                }
                               // if (!collisionJumping)
                                    //TransVelocity = Vector3.Zero;
                                jumping = false;
                                falling = false;
                            }
                        }
                    }
                    else
                    {
                        InteractiveObj = levelObject;
                    }
                }
            }
        }
    }
}
