using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        //Default width/height of screen.
        public const int X_RESOLUTION = 1280;
        public const int Y_RESOLUTION = 720;
        public const int SINGLE_CELL_SIZE = 48;
        public const float MIN_Z = 0.1f;
        public const float MAX_Z = 5000;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        RenderContext _renderContext;
        BaseCamera _camera;

        Level basicLevel;

        //Note: Add fields for player, background etc. here
        public static Dictionary<String, Model> _models = new Dictionary<string,Model>();
        public static Dictionary<String, Texture2D> _textures = new Dictionary<string, Texture2D>();

        //For controls - stores previous state.
        GamePadState oldGamePadState;
        KeyboardState oldKeyboardState;

        Character player;
        const float MOVEAMOUNT = 2f;

        // Debugging - Steven
        private Boolean collisionJumping = false;
        private String boxHitState = "";
        SpriteFont font;
        private Rectangle brick;
        private bool jumping;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60.0f);

            Window.Title = "Group_Project";
            graphics.PreferredBackBufferWidth = X_RESOLUTION;
            graphics.PreferredBackBufferHeight = Y_RESOLUTION;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _renderContext = new RenderContext();
            _camera = new BaseCamera();
            _camera.Translate(new Vector3(0, 0, 10));
            _renderContext.Camera = _camera;
            


            //Init controls
            oldGamePadState = GamePad.GetState(PlayerIndex.One);
            oldKeyboardState = Keyboard.GetState();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _renderContext.SpriteBatch = spriteBatch;
            _renderContext.GraphicsDevice = graphics.GraphicsDevice;

            // TODO: use this.Content to load your game content here
            //_models.Add("player", Content.Load<Model>("scientist"));
            _models.Add ( "BasicBlock", Content.Load<Model> ( "BasicBlock" ) );
            _models.Add("button", Content.Load<Model>("Button"));
            _models.Add("door", Content.Load<Model>("Door"));
            _models.Add("switch", Content.Load<Model>("Switch"));
            _models.Add("MoveableBox", Content.Load<Model>("MoveableBox"));
            _models.Add("block", Content.Load<Model>("block"));

            _textures.Add("MoveableBox", Content.Load<Texture2D>("WoodPlanks_Color"));
            _textures.Add("clay_blue", Content.Load<Texture2D>("blockTextures/clay_blue"));
            _textures.Add("clay_cyan", Content.Load<Texture2D>("blockTextures/clay_cyan"));
            _textures.Add("clay_silver", Content.Load<Texture2D>("blockTextures/clay_silver"));
            _textures.Add("clay_white", Content.Load<Texture2D>("blockTextures/clay_white"));
            _textures.Add("quartz", Content.Load<Texture2D>("blockTextures/quartz"));
            _textures.Add("stone", Content.Load<Texture2D>("blockTextures/stone"));

            //loads the basic level
            basicLevel = LevelBuilder.MakeBasicLevel ();

            player = new Character(basicLevel.PlayerPoint.X, basicLevel.PlayerPoint.Y);
            player.LoadContent(Content);
            _renderContext.Player = player;
            _camera.setFollowTarget(player);
            player.TransAccel = new Vector3 ( 0, -SINGLE_CELL_SIZE * 9, 0 );
            font = Content.Load<SpriteFont>("Verdana");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            _renderContext.GameTime = gameTime;
            //Update physics
            //Just player and level object physics for now
            UpdatePhysics ( player );
            player.Update(_renderContext);
            CheckPlayerBoxCollision();

            
            //Calls to control methods
            UpdateGamePad();
            UpdateKeyboard();
            player.AdjacentObj = null; //reset to null after checking PickBox, and before the adjacentObj is updated
            CheckPlayerBoxCollision();
            if (player.TransVelocity.Y >= 0)
                collisionJumping = true;
            else
                collisionJumping = false;

            if (player.TransVelocity.Y > 0)
                jumping = true;
            
            
            if (DebugCheckPlayerBoxCollision() && !collisionJumping)
            {
                player.Position = new Vector3((int)player.Position.X, brick.Top + SINGLE_CELL_SIZE - 1, 0);
                player.TransVelocity = Vector3.Zero;
                jumping = false;
            }


            // TODO: Add your update logic here
            _renderContext.GameTime = gameTime;
            _camera.Update(_renderContext);
            basicLevel.Update(_renderContext);
            // update player£¨not included in basicLevel)
            player.Update(_renderContext);


            base.Update(gameTime);
        }

        /// <summary>
        /// This updates the physics of te player.
        /// </summary>
        /// <param name="Object"></param>
        private void UpdatePhysics (GameObject3D Object)
        {
            Object.TransVelocity += Object.TransAccel / 60; //amt. accel (where TransAccel is in seconds) per frame ...
            Object.Translate(Object.Position+Object.TransVelocity / 60);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            player.Draw(_renderContext);
            
            

            basicLevel.Draw(_renderContext);
            
            /*
            spriteBatch.Begin();
            spriteBatch.DrawString(font, DebugCheckPlayerBoxCollision().ToString(), new Vector2(50, 50), Color.Black);
            spriteBatch.DrawString(font, player.TransVelocity.ToString(), new Vector2(50, 100), Color.Black);
            spriteBatch.DrawString(font, boxHitState, new Vector2(50, 150), Color.Black);
            spriteBatch.End();
            
            // Spritebatch changes graphicsdevice values; sets the oringinal state
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;*/
            
            spriteBatch.Begin();
            spriteBatch.DrawString(font, DebugCheckPlayerBoxCollision().ToString(), new Vector2(50, 50), Color.Black);
            spriteBatch.DrawString(font, "Velocity: " + player.TransVelocity.ToString(), new Vector2(50, 100), Color.Black);
            spriteBatch.DrawString(font, "Acceleration: " + player.TransAccel.ToString(), new Vector2(50, 200), Color.Black);
            spriteBatch.DrawString(font, "Box: " + brick.ToString(), new Vector2(50, 250), Color.Black);
            spriteBatch.DrawString(font, boxHitState, new Vector2(50, 150), Color.Black);
            spriteBatch.End();

            // Spritebatch changes graphicsdevice values; sets the oringinal state
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            base.Draw(gameTime);
        }
        private Boolean DebugCheckPlayerBoxCollision()
        {
            player.Hitbox = new Rectangle((int)player.Position.X, (int)player.Position.Y, SINGLE_CELL_SIZE, SINGLE_CELL_SIZE);
            foreach (CellObject brick in basicLevel.Children)
            {
                if (player.Hitbox.Intersects(brick.Hitbox) && brick.isCollidable)
                {
                    this.brick = brick.Hitbox;
                    return true;
                }
            }
            return false;
        }
        private void CheckPlayerBoxCollision()
        {
            player.Hitbox = new Rectangle((int)player.Position.X, (int)player.Position.Y, SINGLE_CELL_SIZE, SINGLE_CELL_SIZE);
            foreach (CellObject levelObject in basicLevel.Children)
            {
                if (levelObject.isCollidable && player.Hitbox.Intersects(levelObject.Hitbox))
                {
                    /**Determining what side was hit**/
                    float wy = (levelObject.Hitbox.Width + player.Hitbox.Width)
                             * (((levelObject.Hitbox.Y + levelObject.Hitbox.Height) / 2) - (player.Hitbox.Y + player.Hitbox.Height) / 2);
                    float hx = (player.Hitbox.Height + levelObject.Hitbox.Height)
                             * (((levelObject.Hitbox.X + levelObject.Hitbox.Width) / 2) - (player.Hitbox.X + player.Hitbox.Width) / 2);

                    Button tmpButton = levelObject as Button;
                    if (tmpButton != null) //if it is a button
                    {
                        Button button = (Button)levelObject as Button;
                        button.IsPressed = true;
                        break;
                    }

                    if (wy > hx)
                    {
                        if (wy > -hx)
                        {
                            boxHitState = "Box Top";//top
                            player.Position = new Vector3((int)player.Position.X, (int)player.Position.Y - 1, 0);
                            player.TransVelocity = Vector3.Zero;
                        }
                        else
                        {
                            boxHitState = "Box Left";// left
                            player.Position = new Vector3(levelObject.Hitbox.Right, (int)player.Position.Y, 0);
                            player.AdjacentObj = levelObject;
                        }
                    }
                    else
                    {
                        if (wy > -hx)
                        {
                            boxHitState = "Box Right";// right
                            player.Position = new Vector3(levelObject.Hitbox.Left - SINGLE_CELL_SIZE, (int)player.Position.Y, 0);
                            player.AdjacentObj = levelObject;
                        }
                        else
                        {
                            boxHitState = "Box Bottem";//bottem
                             player.Position = new Vector3((int)player.Position.X, (int)levelObject.Hitbox.Bottom - 1, 0);
                            jumping = false;
                        }
                    }
                    break;
                }
                boxHitState = "No box";

            }
        }

        public void PutBox()
        {
            //if (/*player.adjacentObj == null*/) //will need a condition for when the adjacent area where the player would be trying to put the box is empty,
            //{
                player.interactState = Character.InteractState.StartingDropBox; //state for while the player begins putting down the box
            //}
        }

        public void InteractWithObject()
        {
            if (player.interactState == Character.InteractState.HandsEmpty && player.AdjacentObj != null)
            {
                if (player.AdjacentObj.GetType() == typeof(PickableBox))
                {
                    player.interactState = Character.InteractState.JustPickedUpBox;
                    player.StoredBox = (PickableBox)player.AdjacentObj;
                    player.StoredBox.isCollidable = false;
                }
                else if (player.AdjacentObj.GetType() == typeof(Switch))
                {
                    Switch currentSwitch = (Switch)player.AdjacentObj;
                    currentSwitch.FlickSwitch();
                }
            }
        }

        private void UpdateKeyboard()
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            //Setting up basic controls
            if (currentKeyboardState.IsKeyDown(Keys.Space) &&
                oldKeyboardState.IsKeyUp(Keys.Space) && !jumping)
            {
                //handle jump movement
                //Added a bit of physics to this.
                jumping = true;
                player.TransVelocity += new Vector3(0, SINGLE_CELL_SIZE * 10, 0);
            }
            if (currentKeyboardState.IsKeyDown(Keys.Z) &&
                oldKeyboardState.IsKeyUp(Keys.Z))
            {
                if(player.interactState == Character.InteractState.CompletedPickup)
                {
                    PutBox();
                }
                else
                    InteractWithObject();
                //handle pick up box
            }
            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                player.MoveLeft(MOVEAMOUNT);
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                player.MoveRight(MOVEAMOUNT);
            }
            else
            {
                player.Stop();
            }

            // Allows the game to exit
            if (currentKeyboardState.IsKeyDown(Keys.Escape))
                this.Exit();

            oldKeyboardState = currentKeyboardState;
        }

        private void UpdateGamePad()
        {
            GamePadState currentGamePadState = GamePad.GetState(PlayerIndex.One);

            //Setting up basic controls
            if (currentGamePadState.Buttons.A == ButtonState.Pressed &&
                oldGamePadState.Buttons.A != ButtonState.Pressed)
            {
                //handle jump movement
                player.TransAccel = new Vector3(0, -SINGLE_CELL_SIZE * 9, 0);
                player.TransVelocity += new Vector3(0, SINGLE_CELL_SIZE * 5, 0);
            }
            if (currentGamePadState.Buttons.X == ButtonState.Pressed &&
                oldGamePadState.Buttons.X != ButtonState.Pressed)
            {
                //handle pick up box
            }
            if (currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                //handle left movement
                player.MoveLeft(MOVEAMOUNT);
            }
            else if (currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                //handle right right movement
                player.MoveRight(MOVEAMOUNT);
            }
            

            // Allows the game to exit
            if (currentGamePadState.Buttons.Back == ButtonState.Pressed)
                this.Exit();

            oldGamePadState = currentGamePadState;
        }
    }

}
