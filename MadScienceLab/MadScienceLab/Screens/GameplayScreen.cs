#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections;
using GameStateManagement;
using MadScienceLab;
#endregion

namespace MadScienceLab
{
    /// <summary>
    /// This screen implements the actual game logic.
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        public static Level CurrentLevel { get; private set; }

        SpriteBatch spriteBatch;

        ContentManager content;

        RenderContext _renderContext;
        BaseCamera _camera;
        GameTimer _timer;

        Level basicLevel;

        //Note: Add fields for player, background etc. here
        public static Dictionary<String, Model> _models;
        public static Dictionary<String, Texture2D> _textures;
        public static Dictionary<String, SoundEffect> _sounds;

        Character player;

        // Debugging - Steven
        SpriteFont font;

        //Debugging - FPS - Matt
        private FPSCounter fpsCount;

        Random random;

        float pauseAlpha;

        InputAction pauseAction;

        // Level selected string to build level
        int levelNum;


        // struct holds the level data passed to be displayed on level complete screen and stored in save file
        public struct LevelData
        {
            public int currentlevelNum; // number of  current level
            public TimeSpan time; // time recorded
            public TimeSpan levelParTime; // current level par time
            public int remainingHealth;

            public LevelData(int levelNum, TimeSpan levelParTime)
            {
                this.currentlevelNum = levelNum;
                time = TimeSpan.Zero;
                this.levelParTime = levelParTime;
                remainingHealth = 3;
            }
        }

        LevelData levelData;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor. Initialize game data here
        /// </summary>
        public GameplayScreen(int levelNum)
        {
            this.levelNum = levelNum;


            // transition time used for screen transitions
            TransitionOnTime = TimeSpan.FromSeconds(1);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);

            // initialize rendercontext and camera
            _renderContext = new RenderContext();
            _camera = new BaseCamera();
            _camera.Translate(new Vector3(0, 0, 10));
            _renderContext.Camera = _camera;

            // initialize models,textures,sounds
            _models = new Dictionary<string, Model>();
            _textures = new Dictionary<string, Texture2D>();
            _sounds = new Dictionary<string, SoundEffect>();

            random = new Random();
            //init fps counter
            fpsCount = new FPSCounter(_renderContext);
            Quadtree _quadtree = new Quadtree(0, new Rectangle(0, 0, GameConstants.X_RESOLUTION, GameConstants.X_RESOLUTION));
            _renderContext.Quadtree = _quadtree;
        }


        /// <summary>
        /// Load graphics, sounds content for the game. Create the level
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                // Create a new SpriteBatch, which can be used to draw textures.
                spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);
                _renderContext.GameTime = new GameTime();

                _renderContext.SpriteBatch = spriteBatch;
                _renderContext.GraphicsDevice = ScreenManager.GraphicsDevice;

                //Set up basic effect for drawing background
                _renderContext.BasicEffect = new BasicEffect(_renderContext.GraphicsDevice);
                


                // TODO: use this.Content to load your game content here
                //_models.Add("player", Content.Load<Model>("scientist"));
                _models.Add("BasicBlock", content.Load<Model>("BasicBlock"));
                _models.Add("BackgroundBlock", content.Load<Model>("BackgroundBlock"));
                _models.Add("button", content.Load<Model>("Button"));
                _models.Add("door", content.Load<Model>("Door"));
                _models.Add("switch", content.Load<Model>("ToggleSwitch"));
                _models.Add("MoveableBox", content.Load<Model>("MoveableBox"));
                _models.Add("BlockDropper", content.Load<Model>("BlockDropper"));
                _models.Add("BlockDropper_Empty", content.Load<Model>("BlockDropper_Empty"));
                _models.Add("block", content.Load<Model>("block"));
                _models.Add("Turret", content.Load<Model>("turret"));
                _models.Add("projectile", content.Load<Model>("projectile"));
                _models.Add("ExitBlock", content.Load<Model>("ExitBlock"));

                _textures.Add("MoveableBox", content.Load<Texture2D>("WoodPlanks_Color"));
                _textures.Add("BlockDropper", content.Load<Texture2D>("Textures/dropper"));
                _textures.Add("BareMetal_Gray", content.Load<Texture2D>("Textures/BareMetal_Gray"));
                _textures.Add("BrushedRoundMetal_Gray", content.Load<Texture2D>("Textures/BrushedRoundMetal_Gray"));
                _textures.Add("DirtyMetal", content.Load<Texture2D>("Textures/DirtyMetal"));
                _textures.Add("Fiberglass_White", content.Load<Texture2D>("Textures/Fiberglass_White"));
                _textures.Add("MetalFloor_Gray", content.Load<Texture2D>("Textures/MetalFloor_Gray"));
                _textures.Add("Tile_Beige", content.Load<Texture2D>("Textures/Tile_Beige_Half"));
                _textures.Add("Tile_Blue", content.Load<Texture2D>("Textures/Tile_Blue"));
                _textures.Add("Tile_DarkGray", content.Load<Texture2D>("Textures/Tile_DarkGray"));
                _textures.Add("Tile_Gray", content.Load<Texture2D>("Textures/Tile_Gray"));
                _textures.Add("WindowBlocks", content.Load<Texture2D>("Textures/WindowBlocks"));
                _textures.Add("Tile_Fun", content.Load<Texture2D>("Textures/Tile_Fun"));
                _textures.Add("Exit", content.Load<Texture2D>("Textures/EXIT"));
                _textures.Add("Complete", content.Load<Texture2D>("Textures/Complete"));
                _textures.Add("GameOver", content.Load<Texture2D>("Textures/GameOver"));
                _renderContext.Textures = _textures;

                //Loads sound references
                _sounds.Add("BoxDrop", content.Load<SoundEffect>("Sounds/BoxDrop"));
                _sounds.Add("BoxPickup", content.Load<SoundEffect>("Sounds/BoxPickup"));
                _sounds.Add("Button", content.Load<SoundEffect>("Sounds/Button"));
                _sounds.Add("DoombaLoop", content.Load<SoundEffect>("Sounds/DoombaLoop"));
                _sounds.Add("Jump", content.Load<SoundEffect>("Sounds/Jump"));
                _sounds.Add("Land", content.Load<SoundEffect>("Sounds/Land"));
                _sounds.Add("LaserShoot", content.Load<SoundEffect>("Sounds/LaserShoot"));
                _sounds.Add("LaserWhirLoop", content.Load<SoundEffect>("Sounds/LaserWhirLoop"));
                _sounds.Add("PlayerHit", content.Load<SoundEffect>("Sounds/PlayerHit"));
                _sounds.Add("ToggleSwitch", content.Load<SoundEffect>("Sounds/ToggleSwitch"));

                //loads the basic level
                basicLevel = LevelBuilder.MakeBasicLevel(levelNum);
                basicLevel.setBackgroundBuffer(_renderContext); //Matt: need to do this now to draw background properly

                CurrentLevel = basicLevel; //we can handle this through render context eventually.
                basicLevel.LoadContent(content);

                player = new Character(basicLevel.PlayerPoint.X, basicLevel.PlayerPoint.Y);
                player.LoadContent(content);
                _renderContext.Player = player;
                _camera.setFollowTarget(player);
                player.TransAccel = new Vector3(0, -GameConstants.SINGLE_CELL_SIZE * 9, 0);
                font = content.Load<SpriteFont>("Verdana");
                _renderContext.Level = basicLevel;
                _renderContext.SpriteFont = font;
                basicLevel.PopulateTypeList(_renderContext);

                _renderContext.Level.collidableObjects.Add(player); // Adding player to list of collidable objects - Steven

                _timer = new GameTimer(_renderContext);
                _renderContext.GameTimer = _timer;


                // Sets level data to level and sets level par time from file.
                levelData = new LevelData(levelNum, TimeSpan.Zero);

                //load fps count content
                fpsCount.LoadContent(content);
                
                // if game takes long to load. Simulate load by delaying for a
                // while, giving you a chance to admire the beautiful loading screen.
                Thread.Sleep(500);
                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }

        }


        public override void Deactivate()
        {
            
            
            base.Deactivate();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            content.Unload();

        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);
            if (IsActive)
            {
                _renderContext.Quadtree.clear();

                foreach (CellObject obj in basicLevel.Children)
                {
                    _renderContext.Quadtree.insert(obj);
                }

                _renderContext.GameTime = gameTime;
                
                player.Update(_renderContext);
                             
                //apply gravity to pickable boxes
                foreach (CellObject levelobject in basicLevel.Children)
                {
                    //check collision - cease gravity if colliding with another box below - then apply physics
                    if (levelobject.GetType() == typeof(PickableBox) && levelobject.isCollidable)
                    {
                        UpdatePhysics(levelobject);
                        levelobject.TransAccel = new Vector3(0, -GameConstants.SINGLE_CELL_SIZE * 9, 0);
                        levelobject.CheckCollision(basicLevel);
                    }
                }
                             
                player.AdjacentObj = null; //reset to null after checking PickBox, and before the adjacentObj is updated

                _renderContext.GameTime = gameTime;
                _camera.Update(_renderContext);
                basicLevel.Update(_renderContext);
                player.Update(_renderContext);

                // Check to see if the level is complete or player died game over. Pass level data to levelCompleteScreen
                if (_renderContext.Level.LevelOver)
                {
                    levelData.time = _timer.ElapsedTime;
                    LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                               new LevelCompleteScreen(levelData));
                }
                if (_renderContext.Level.GameOver)
                {
                    levelData.time = _timer.ElapsedTime;
                    LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                               new GameOverScreen(levelData));
                }

                _timer.Update(_renderContext.GameTime);
                //fps debug
                fpsCount.Update(gameTime);    
                //timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Spritebatch changes graphicsdevice values; sets the oringinal state
            ScreenManager.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            ScreenManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ScreenManager.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            player.Draw(_renderContext);
            basicLevel.Draw(_renderContext);
            
            spriteBatch.Begin();
            //spriteBatch.DrawString(font, DebugCheckPlayerBoxCollision().ToString(), new Vector2(50, 50), Color.Black);
            spriteBatch.DrawString(font, "Health: " + player.GetHealth().ToString(), new Vector2(50, 50), Color.Black);
            //spriteBatch.DrawString(font, "Time: " + timer, new Vector2(300, 50), Color.Black);
            //spriteBatch.DrawString(font, "Velocity: " + player.TransVelocity.ToString(), new Vector2(50, 100), Color.Black);
            //spriteBatch.DrawString(font, "Acceleration: " + player.TransAccel.ToString(), new Vector2(50, 200), Color.Black);
            //spriteBatch.DrawString(font, "Box: " + brick.ToString(), new Vector2(50, 250), Color.Black);
            //spriteBatch.DrawString(font, boxHitState, new Vector2(50, 150), Color.Black);
            //spriteBatch.DrawString(font, "Projectile: " + basicLevel.Children[basicLevel.Children.Count()-1].Hitbox.ToString(), new Vector2(50, 150), Color.Black);
            //spriteBatch.DrawString(font, "Player pos: " + player.Position.ToString(), new Vector2(50, 300), Color.Black);
            spriteBatch.End();

            //fpsCount.Draw(gameTime);
            fpsCount.Draw(gameTime);
            _timer.Draw(_renderContext.GameTime);
            // Spritebatch changes graphicsdevice values; sets the oringinal state
            ScreenManager.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            ScreenManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ScreenManager.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }

        }

        #endregion

        private void UpdatePhysics(GameObject3D Object)
        {
            Object.TransVelocity += Object.TransAccel / 60; //amt. accel (where TransAccel is in seconds) per frame ...
            Object.Translate(Object.Position + Object.TransVelocity / 60);
        }
    }
}
