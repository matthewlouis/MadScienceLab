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
        public static Level    CurrentLevel { get; private set; }
        private SpriteBatch    spriteBatch;
        private ContentManager content;
        private RenderContext  _renderContext;
        private BaseCamera     _camera;
        private GameTimer      _timer;
        private SoundEffect    levelMusic;
        private SpriteFont     font;
        private Level          basicLevel;

        //Note: Add fields for player, background etc. here
        public static Dictionary<String, Model> _models;
        public static Dictionary<String, Texture2D> _textures;
        public static Dictionary<String, SoundEffect> _sounds;
        public static Dictionary<string, MessageEvent> messages;

        // Player Info
        Character player;
        private bool damageTaken;

        // UI elements
        private Texture2D dummyTexture;
        private Effect healthState;
        private Rectangle healthPosition = new Rectangle(170, 125, 250, 30);
        private Rectangle gear1Position = new Rectangle(25, 25, 150, 150);
        private Rectangle gear2Position = new Rectangle(170, 35, 100, 100);
        private Rectangle healthTexturePos = new Rectangle(195, 150, 250, 30);
        private Rectangle playerGear = new Rectangle(50, 50, 150, 150);
        private Rectangle playerHealthGear = new Rectangle(195, 60, 100, 100);
        private Rectangle playerHealthCountGear = new Rectangle(190, 80, 60, 60);
        private Rectangle centerHealthGear = new Rectangle(75, 75, 100, 100);
        private List<float> healthGearAnglesList;
        private int hudtype = 1; // Only used for switching between different UI for feedback - Steven
        private int arrowBobEffect = 0;
        private int reverseNum = 1;
        private float rotationAngle;
        private int playerBobDirection = 1;

        private KeyboardState lastkey;

        // Debugging - Steven
        private float playerBob;

        //Debugging - FPS - Matt
        private FPSCounter fpsCount;
        private Random random;
        private float pauseAlpha;
        private InputAction pauseAction;
        public static bool messageActive;
        public static MessageEvent messageObj;

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

        GameData.LevelData levelData;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor. Initialize game data here
        /// </summary>
        public GameplayScreen(int levelNum)
        {
            this.levelNum = levelNum;
            // Sets level data to level and sets level par time from file.
            levelData = new GameData.LevelData(levelNum, TimeSpan.Zero);

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
            healthGearAnglesList = new List<float>();
            messageActive = false;

            
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
                _models.Add("MovingBlock", content.Load<Model>("MovingBlock"));

                _textures.Add("MoveableBox", content.Load<Texture2D>("WoodPlanks_Color"));
                _textures.Add("BlockDropper", content.Load<Texture2D>("Textures/dropper"));

                // background textures
                _textures.Add("Pipes2", content.Load<Texture2D>("Textures/Pipes2"));
                _textures.Add("Pipes1", content.Load<Texture2D>("Textures/Pipes1"));
                _textures.Add("Vent", content.Load<Texture2D>("Textures/Vent"));
                _textures.Add("Tile_Black", content.Load<Texture2D>("Textures/Tile_Black"));
                _textures.Add("Tile_Gray", content.Load<Texture2D>("Textures/Tile_Gray"));
                _textures.Add("Emcsquare", content.Load<Texture2D>("Textures/Emcsquare"));
                _textures.Add("atom1", content.Load<Texture2D>("Textures/atom1"));
                _textures.Add("atom2", content.Load<Texture2D>("Textures/atom2"));
                _textures.Add("Tile_Gray_Stain", content.Load<Texture2D>("Textures/Tile_Gray_Stain"));
                _textures.Add("Tile_Gray_Stain1", content.Load<Texture2D>("Textures/Tile_Gray_Stain1"));
                _textures.Add("Tile_Gray_Stain2", content.Load<Texture2D>("Textures/Tile_Gray_Stain2"));
                _textures.Add("Tile_Gray_Stain3", content.Load<Texture2D>("Textures/Tile_Gray_Stain3"));
                _textures.Add("Einstien", content.Load<Texture2D>("Textures/Einstien"));
                _textures.Add("chalkboard1", content.Load<Texture2D>("Textures/chalkboard1"));
                _textures.Add("chalkboard2", content.Load<Texture2D>("Textures/chalkboard2"));
                _textures.Add("chalkboard3", content.Load<Texture2D>("Textures/chalkboard3"));
                _textures.Add("chalkboard4", content.Load<Texture2D>("Textures/chalkboard4"));



                _textures.Add("Tile_White", content.Load<Texture2D>("Textures/Tile_White"));
                _textures.Add("Tile_Fun", content.Load<Texture2D>("Textures/Tile_Fun"));
                _textures.Add("Exit", content.Load<Texture2D>("Textures/EXIT"));
                _textures.Add("Complete", content.Load<Texture2D>("Textures/Complete"));
                _textures.Add("GameOver", content.Load<Texture2D>("Textures/GameOver"));
                _textures.Add("Arrow", content.Load<Texture2D>("Textures/Arrow"));
                _textures.Add("A_Button", content.Load<Texture2D>("Textures/Controller/A_Button"));
                _textures.Add("B_Button", content.Load<Texture2D>("Textures/Controller/B_Button"));
                _textures.Add("X_Button", content.Load<Texture2D>("Textures/Controller/X_Button"));
                _textures.Add("Y_Button", content.Load<Texture2D>("Textures/Controller/Y_Button"));
                _textures.Add("Back_Button", content.Load<Texture2D>("Textures/Controller/Back_Button"));
                _textures.Add("Start_Button", content.Load<Texture2D>("Textures/Controller/Start_Button"));
                //_textures.Add("Left_Button", content.Load<Texture2D>("Textures/Controller/Left_Button"));
                _textures.Add("Right_Button", content.Load<Texture2D>("Textures/Controller/Right_Button"));
                _textures.Add("DPad", content.Load<Texture2D>("Textures/Controller/DPad"));
                _textures.Add("Left_DPad", content.Load<Texture2D>("Textures/Controller/Left_DPad"));
                _textures.Add("Right_DPad", content.Load<Texture2D>("Textures/Controller/Right_DPad"));
                _textures.Add("Up_DPad", content.Load<Texture2D>("Textures/Controller/Up_DPad"));
                _textures.Add("Down_DPad", content.Load<Texture2D>("Textures/Controller/Down_DPad"));
                _textures.Add("UpDown_DPad", content.Load<Texture2D>("Textures/Controller/UpDown_DPad"));
                _textures.Add("LeftRight_DPad", content.Load<Texture2D>("Textures/Controller/LeftRight_DPad"));
                _textures.Add("Left_Stick", content.Load<Texture2D>("Textures/Controller/Left_Stick"));
                _textures.Add("Right_Stick", content.Load<Texture2D>("Textures/Controller/Right_Stick"));
                _textures.Add("Right_Trigger", content.Load<Texture2D>("Textures/Controller/Right_Trigger"));
                _textures.Add("LeftRight_Trigger", content.Load<Texture2D>("Textures/Controller/LeftRight_Trigger"));
                _textures.Add("Gear", content.Load<Texture2D>("Textures/UI/GearTest"));
                _textures.Add("LaserRed", content.Load<Texture2D>("Textures/UI/LaserRed"));
                _textures.Add("LaserOrange", content.Load<Texture2D>("Textures/UI/LaserOrange"));
                _textures.Add("LaserGreen", content.Load<Texture2D>("Textures/UI/LaserGreen"));
                _textures.Add("LightGreen", content.Load<Texture2D>("Textures/UI/LightBulbGreen"));
                _textures.Add("LightYellow", content.Load<Texture2D>("Textures/UI/LightBulbYellow"));
                _textures.Add("LightRed", content.Load<Texture2D>("Textures/UI/LightBulbRed"));
                _textures.Add("Gear2", content.Load<Texture2D>("Textures/UI/GearTestV2"));
                _textures.Add("GearHealthBar", content.Load<Texture2D>("Textures/UI/HealthBar"));
                _textures.Add("MessageBackground", content.Load<Texture2D>("Textures/message_background"));
                _textures.Add("PlayerPortrait", content.Load<Texture2D>("Textures/UI/PlayerPortrait"));
               //// _textures.Add("Explosion", content.Load<Texture2D>("Texture/explosion"));
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
                _sounds.Add("Type", content.Load<SoundEffect>("Sounds/Type"));

                _renderContext.Sounds = _sounds;

                // Loading shaders
                healthState = content.Load<Effect>("Shaders/HealthPixel");
                //loads the basic level
                basicLevel = LevelBuilder.MakeBasicLevel(levelData.currentlevelNum, _renderContext);
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
                _renderContext.MessageFont = content.Load<SpriteFont>("MessageFont");
                basicLevel.PopulateTypeList(_renderContext);

                _renderContext.Level.collidableObjects.Add(player); // Adding player to list of collidable objects - Steven

                // Single color texture for minimap rendering
                dummyTexture = new Texture2D(_renderContext.GraphicsDevice, 1, 1);
                dummyTexture.SetData(new Color[] { Color.White * 0.8f});

                _timer = new GameTimer(_renderContext);
                _renderContext.GameTimer = _timer;

                //load fps count content
                fpsCount.LoadContent(content);
                
                //load music
                levelMusic = content.Load<SoundEffect>("Songs/MusicInGameLoop");
                MusicPlayer.SetVolume(1f);
                MusicPlayer.PlaySong(levelMusic);

                Quadtree _quadtree = new Quadtree(0, _renderContext.Level.Hitbox);
                _renderContext.Quadtree = _quadtree;

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
            if (IsActive && !messageActive)
            {
                // Setting the vertical offset for bobbing images
                if (arrowBobEffect > 10)
                    reverseNum = -1;
                else if (arrowBobEffect < -10)
                    reverseNum = 1;

                arrowBobEffect += (int)(0.1f * gameTime.ElapsedGameTime.Milliseconds) * reverseNum;

                if (playerBob > 0.6f)
                    playerBobDirection = -1;
                else if (playerBob < -0.6f)
                    playerBobDirection = 1;

                playerBob += (float)gameTime.ElapsedGameTime.TotalSeconds / 5 * playerBobDirection;
                playerBob = playerBob % (MathHelper.Pi * 2);

                rotationAngle += (float)gameTime.ElapsedGameTime.TotalSeconds * 1;
                rotationAngle = rotationAngle % (MathHelper.Pi * 2);
                _renderContext.Quadtree.clear();

                foreach (CellObject obj in basicLevel.Children)
                {
                    _renderContext.Quadtree.insert(obj);
                }

                _renderContext.GameTime = gameTime;
                
                player.Update(_renderContext);
                             
                //apply gravity to pickable boxes
                foreach (CellObject levelobject in basicLevel.collidableObjects)
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
                player.InteractiveObj = null;
                player.canPlace = true;
                _renderContext.GameTime = gameTime;
                _camera.Update(_renderContext);
                basicLevel.Update(_renderContext);
                player.Update(_renderContext);

                // Check to see if the level is complete or player died game over. Pass level data to levelCompleteScreen
                if (_renderContext.Level.LevelOver)
                {
                    levelData.time = _timer.ElapsedTime.ToString ();
                    levelData.remainingHealth = _renderContext.Player.GetHealth();
                    LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                               new LevelCompleteScreen(levelData));
                }
                if (_renderContext.Level.GameOver)
                {
                    levelData.time = _timer.ElapsedTime.ToString();
                    LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                               new GameOverScreen(levelData));
                }

                _timer.Update(_renderContext.GameTime);
                //fps debug
                fpsCount.Update(gameTime);    
                //timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else //still cover messages to update
            {
                foreach (CellObject levelobject in basicLevel.Children)
                {
                    if (levelobject.GetType() == typeof(MessageEvent))
                    {
                        levelobject.Update(_renderContext);
                    }
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.H) && lastkey.IsKeyUp(Keys.H))
            {
                hudtype++;
            }

            lastkey = Keyboard.GetState();
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
            
            if (_textures.Count > 0) // Check for empty textures due to retrying level - Steven
            {
                switch (hudtype % 3)
                {
                    case 0:
                        DrawPlayerHealth(_renderContext);
                        break;
                    case 1:
                        DrawPlayerHealthV2(_renderContext);
                        break;
                    case 2:
                        DrawPlayerHealthV3(_renderContext);
                        break;
                    default:
                        DrawPlayerHealthV3(_renderContext);
                        break;
                }
                DrawInteractiveUI(_renderContext);
                DrawDebugMap(_renderContext);
            }
            spriteBatch.End();

            //fpsCount.Draw(gameTime);



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
        /// Basic placeholder UI for player health
        /// </summary>
        /// <param name="_renderContext"></param>
        public void DrawPlayerHealth(RenderContext _renderContext)
        {

            Rectangle healthTexturePos = new Rectangle(195, 150, 250, 30);
            Rectangle playerGear = new Rectangle(50, 50, 150, 150);
            Rectangle playerHealthGear = new Rectangle(195, 60, 100, 100);
            Texture2D healthTexture;
            string currentState;
            
            if (player.GetHealth() == GameConstants.HEALTH)
            {
                healthTexture = _textures["LaserGreen"];
                currentState = ":)";
            }
            else if (player.GetHealth() < GameConstants.HEALTH && player.GetHealth() > 1)
            {
                healthTexture = _textures["LaserOrange"];
                currentState = ":|";
            }
            else
            {
                healthTexture = _textures["LaserRed"];
                currentState = ":(";

            }

            Vector2 stateTextSize = font.MeasureString(currentState);
            Vector2 currentStatePos = new Vector2(playerGear.Center.X, playerGear.Center.Y);

            string currentHealth = player.GetHealth().ToString() + "/" + GameConstants.HEALTH;
            Vector2 textSize = font.MeasureString(currentHealth);
            Vector2 currentHealthPos = new Vector2(playerHealthGear.Center.X - textSize.X / 2, playerHealthGear.Center.Y - textSize.Y / 2);

            Vector2 origin = new Vector2(_textures["Gear"].Bounds.Width / 2, _textures["Gear"].Bounds.Height / 2);

            spriteBatch.DrawString(font, currentHealth, currentHealthPos, Color.Black);
            spriteBatch.DrawString(font, currentState, currentStatePos, Color.Black, MathHelper.PiOver2, new Vector2(stateTextSize.X / 2, stateTextSize.Y / 2), 1f, SpriteEffects.None, 1);
            spriteBatch.Draw(
                _textures["Gear"], 
                new Vector2(playerGear.X + 75, playerGear.Y + 75), 
                null, Color.White, rotationAngle, 
                origin, 
                (float)playerGear.Width / _textures["Gear"].Width, 
                SpriteEffects.None, 0);
            spriteBatch.Draw(
                _textures["Gear"], 
                new Vector2(playerHealthGear.X + playerHealthGear.Width / 2, playerHealthGear.Y + playerHealthGear.Height / 2), 
                null, Color.White, -rotationAngle, 
                origin, 
                (float)playerHealthGear.Width / _textures["Gear"].Width, 
                SpriteEffects.None, 0);
            spriteBatch.Draw(healthTexture, healthTexturePos, Color.White);
        }

        public void DrawPlayerHealthV2(RenderContext _renderContext)
        {
            Texture2D healthTexture;

            if (player.GetHealth() == GameConstants.HEALTH)
            {
                healthTexture = _textures["LightGreen"];
            }
            else if (player.GetHealth() < GameConstants.HEALTH && player.GetHealth() > 1)
            {
                healthTexture = _textures["LightYellow"];
            }
            else
            {
                healthTexture = _textures["LightRed"];
            }

            Vector2 origin = new Vector2(_textures["Gear2"].Bounds.Width / 2, _textures["Gear2"].Bounds.Height / 2);

            // The background the health gears rests on
            spriteBatch.Draw(_textures["GearHealthBar"], 
                new Rectangle(playerHealthCountGear.X , 
                    playerHealthCountGear.Center.Y - 15, 
                    playerHealthCountGear.Width * GameConstants.HEALTH + 5, 30), 
                    Color.White);
            float empAngle = 0f;
            // Draw the amount of gears based on max health count
            for (int i = 0; i < GameConstants.HEALTH; i++)
            {
                float angle = rotationAngle;
                Vector2 position = new Vector2(playerHealthCountGear.X + playerHealthCountGear.Width / 2 + i * (playerHealthCountGear.Width - 3), playerHealthCountGear.Y + playerHealthCountGear.Height / 2);
               
                // Reverse the spin of every odd gears
                if (i % 2 == 0)
                    angle = -(angle+2f);

                // Centers and resizes the health lights inside the health gears
                int offset = 20;

                if (i < player.GetHealth()) // Gears for health that has not taken damage
                {
                    spriteBatch.Draw(healthTexture, 
                        new Rectangle((int)position.X - offset, (int)position.Y - offset, playerHealthCountGear.Width - offset, playerHealthCountGear.Width - offset), 
                        Color.White);

                    spriteBatch.Draw(_textures["Gear2"],
                        position,
                        null, Color.White, angle,
                        origin,
                        (float)playerHealthCountGear.Width / _textures["Gear2"].Width,
                        SpriteEffects.None, 0);
                }
                else if (i == player.GetHealth()) // The gear that animates from left to right when damage is taken
                {
                    if (player.GetHealth() == 1)
                        healthTexture = _textures["LightYellow"];
                    if (player.IsInvuln())
                    {
                        spriteBatch.End();
                        healthState.Parameters["healthState"].SetValue(player.GetTimeScale());
                        spriteBatch.Begin(0, BlendState.AlphaBlend, null, null, null, healthState);
                        spriteBatch.Draw(healthTexture,
                            new Rectangle((int)(position.X - offset + 20 * player.damageDelayTime), (int)position.Y - offset, playerHealthCountGear.Width - offset, playerHealthCountGear.Width - offset),
                            Color.White);
                        spriteBatch.End();
                        spriteBatch.Begin();
                        spriteBatch.Draw(_textures["Gear2"],
                            new Vector2(position.X + 20 * player.damageDelayTime, position.Y),
                            null, Color.White, angle,
                            origin,
                            (float)playerHealthCountGear.Width / _textures["Gear2"].Width,
                            SpriteEffects.None, 0);
                        damageTaken = true;
                        empAngle = angle;
                    }
                    else
                    {
                        if (damageTaken)
                        {
                            healthGearAnglesList.Add(angle);
                            damageTaken = false;
                        }
                        spriteBatch.End();
                        healthState.Parameters["healthState"].SetValue(0);
                        spriteBatch.Begin(0, BlendState.AlphaBlend, null, null, null, healthState);
                        spriteBatch.Draw(healthTexture,
                            new Rectangle((int)(position.X - offset + 20 * player.damageDelayTime), (int)position.Y - offset, playerHealthCountGear.Width - offset, playerHealthCountGear.Width - offset),
                            Color.White);
                        spriteBatch.End();
                        spriteBatch.Begin();
                        spriteBatch.Draw(_textures["Gear2"],
                            new Vector2(position.X + 20, position.Y),
                            null, Color.White, healthGearAnglesList[GameConstants.HEALTH - i - 1],
                            origin,
                            (float)playerHealthCountGear.Width / _textures["Gear2"].Width,
                            SpriteEffects.None, 0);
                    }
                }
                else // Stop rotating the gears that represent missing health
                {
                    if (player.GetHealth() == 1)
                    healthTexture = _textures["LightYellow"];
                    spriteBatch.End();
                    healthState.Parameters["healthState"].SetValue(0);
                    spriteBatch.Begin(0, BlendState.AlphaBlend, null, null, null, healthState);
                    spriteBatch.Draw(healthTexture,
                        new Rectangle((int)(position.X - offset + 20), (int)position.Y - offset, playerHealthCountGear.Width - offset, playerHealthCountGear.Width - offset),
                        Color.White);
                    spriteBatch.End();
                    spriteBatch.Begin();
                    spriteBatch.Draw(_textures["Gear2"],
                        new Vector2(position.X + 20, position.Y),
                        null, Color.White, healthGearAnglesList[GameConstants.HEALTH - i - 1],
                        origin,
                        (float)playerHealthCountGear.Width / _textures["Gear2"].Width,
                        SpriteEffects.None, 0);
                }
            }

            // Player Portrait
            spriteBatch.Draw(_textures["LightGreen"], centerHealthGear, new Color(0.7f, 0, 0.7f));
            spriteBatch.Draw(_textures["PlayerPortrait"], centerHealthGear, Color.White);
            spriteBatch.Draw(_textures["Gear2"],
                             new Vector2(playerGear.X + 75, playerGear.Y + 75),
                             null, Color.White, rotationAngle,
                             origin,
                             (float)playerGear.Width / _textures["Gear2"].Width,
                             SpriteEffects.None, 0);
        }

        public void DrawPlayerHealthV3(RenderContext _renderContext)
        {
            Rectangle healthTexturePos = new Rectangle(195, 150, 250, 30);
            Rectangle playerGear = new Rectangle(50, 50, 150, 150);
            Rectangle playerHealthGear = new Rectangle(195, 60, 100, 100);
            Rectangle playerHealthCountGear = new Rectangle(190, 120, 60, 60);
            Rectangle centerHealthGear = new Rectangle(75, 75, 100, 100);
            Texture2D healthTexture;

            if (player.GetHealth() == GameConstants.HEALTH)
            {
                healthTexture = _textures["LightGreen"];
            }
            else if (player.GetHealth() < GameConstants.HEALTH && player.GetHealth() > 1)
            {
                healthTexture = _textures["LightYellow"];
            }
            else
            {
                healthTexture = _textures["LightRed"];
            }

            spriteBatch.End();
            healthState.Parameters["healthState"].SetValue((float)player.GetHealth() / (float)GameConstants.HEALTH);
            spriteBatch.Begin(0, BlendState.AlphaBlend, null, null, null, healthState);
            spriteBatch.Draw(healthTexture, centerHealthGear, Color.White);
            spriteBatch.End();

            Matrix matrix = Matrix.CreateTranslation(0, -centerHealthGear.Center.Y + playerBob* 10, 0) *
                            Matrix.CreateRotationX(playerBob) *
                            Matrix.CreateTranslation(0, centerHealthGear.Center.Y - playerBob * 10, 0) *
                            Matrix.CreateScale(1, 1, 0);

            spriteBatch.Begin(0, BlendState.AlphaBlend, null, null, null, null, matrix);
            spriteBatch.Draw(_textures["PlayerPortrait"], new Rectangle(centerHealthGear.X, centerHealthGear.Y, centerHealthGear.Width, centerHealthGear.Height), Color.White);
            spriteBatch.End();
            
            spriteBatch.Begin();
            Vector2 origin = new Vector2(_textures["Gear2"].Bounds.Width / 2, _textures["Gear2"].Bounds.Height / 2);
            spriteBatch.Draw(
                _textures["Gear2"],
                new Vector2(playerGear.X + 75, playerGear.Y + 75),
                null, Color.White, rotationAngle,
                origin,
                (float)playerGear.Width / _textures["Gear2"].Width,
                SpriteEffects.None, 0);
        }

        /// <summary>
        /// Used for debugging hitboxes - Steven
        /// Refactored by Jacob into an actual minimap
        /// </summary>
        /// <param name="_renderContext"></param>
        public void DrawDebugMap(RenderContext _renderContext)
        {
            if (player.MapEnabled)
            {
                //Can set size of minimap and it will scale accordingly.
                //It can be a fixed size (eg. 300x300, and the level objects would resize accordingly), or size dependent on level, etc.
                Point MinimapSize = new Point ( LevelBuilder.levelwidth*10, LevelBuilder.levelheight*10);

                const int TOP_RIGHT = 1, BOTTOM_RIGHT = 2, BOTTOM_LEFT = 3;

                //Jacob: Added a couple of customizations to the minimap. Can set it accordingly to positons of the screen.
                int MinimapPosition = TOP_RIGHT; //Can set position of minimap
                int MinimapBorder = 5; //Can also set border thickness of minimap
                int MinimapSideOffset = 10;
                Rectangle BorderBox, InnerBox;

                //Draw border
                switch (MinimapPosition) {
                    case TOP_RIGHT:
                        BorderBox = new Rectangle ( GameConstants.X_RESOLUTION - (MinimapSize.X + MinimapBorder) - MinimapSideOffset, MinimapSideOffset - MinimapBorder, (MinimapSize.X + MinimapBorder * 2), (MinimapSize.Y + MinimapBorder * 2) );
                        InnerBox = new Rectangle ( GameConstants.X_RESOLUTION - MinimapSize.X - MinimapSideOffset, MinimapSideOffset, MinimapSize.X, MinimapSize.Y );
                    break;
                    case BOTTOM_RIGHT:
                    BorderBox = new Rectangle ( GameConstants.X_RESOLUTION - (MinimapSize.X + MinimapBorder) - MinimapSideOffset, GameConstants.Y_RESOLUTION - (MinimapSize.Y + MinimapBorder) - MinimapSideOffset, (MinimapSize.X + MinimapBorder * 2), (MinimapSize.Y + MinimapBorder * 2) );
                    InnerBox = new Rectangle ( GameConstants.X_RESOLUTION - MinimapSize.X - MinimapSideOffset, GameConstants.Y_RESOLUTION - (MinimapSize.Y) - MinimapSideOffset, MinimapSize.X, MinimapSize.Y );
                        break;
                    case BOTTOM_LEFT:
                        BorderBox = new Rectangle ( MinimapSideOffset - MinimapBorder, GameConstants.Y_RESOLUTION - (MinimapSize.Y + MinimapBorder) - MinimapSideOffset, (MinimapSize.X + MinimapBorder * 2), (MinimapSize.Y + MinimapBorder * 2) );
                        InnerBox = new Rectangle ( MinimapSideOffset, GameConstants.Y_RESOLUTION - (MinimapSize.Y) - MinimapSideOffset, MinimapSize.X, MinimapSize.Y );
                        break;
                    default:
                        BorderBox = new Rectangle ( MinimapSideOffset - MinimapBorder, GameConstants.Y_RESOLUTION - (MinimapSize.Y + MinimapBorder) - MinimapSideOffset, (MinimapSize.X + MinimapBorder * 2), (MinimapSize.Y + MinimapBorder * 2) );
                        InnerBox = new Rectangle ( MinimapSideOffset, GameConstants.Y_RESOLUTION - (MinimapSize.Y) - MinimapSideOffset, MinimapSize.X, MinimapSize.Y );
                    break;
                }
                spriteBatch.Draw ( dummyTexture, BorderBox, Color.Black * 0.8f );
                spriteBatch.Draw ( dummyTexture, InnerBox, Color.LightGray * 0.8f );

                const int CELL = GameConstants.SINGLE_CELL_SIZE;
                int xLeftWall = (GameConstants.SINGLE_CELL_SIZE * LevelBuilder.startWall) - (GameConstants.X_RESOLUTION / 2);
                int xRightWall = (GameConstants.SINGLE_CELL_SIZE * (LevelBuilder.levelwidth - 1 + LevelBuilder.startWall)) - (GameConstants.X_RESOLUTION / 2); //x value derived from CellObject implementation, BaseCamera
                int yCeilingBlock = CELL * ((LevelBuilder.startFloor + 1) + LevelBuilder.levelheight - 1) - (GameConstants.Y_RESOLUTION / 2); //not
                int yFloor = (GameConstants.SINGLE_CELL_SIZE * (LevelBuilder.startFloor + 1)) - (GameConstants.Y_RESOLUTION / 2); //y value derived from CellObject implementation
                int LevelXSize = xRightWall - xLeftWall + CELL;
                int LevelYSize = yCeilingBlock - yFloor + CELL;
                foreach (CellObject obj in basicLevel.collidableObjects)
                {
                    //Move minimap to the top right of the screen
                    //convert yFloor position to the bottom of the screen, xRightWall to the right of the screen
                    //everything a fraction of (yCeilingBlock - yFloor + 1 cell) and (xRightWall - xLeftWall + 1 cell)
                    // so (converting level to minimap:) box X position = XRightScreen (ie. GameConstants.X_RESOLUTION) - (1 - fraction of that) * minimapSize.X
                    // box Y position = YTopScreen + MinimapSize - (1 - fraction of that) * minimapSize.Y

                    Rectangle box = obj.Hitbox;
                    box.Width = (box.Width * MinimapSize.X / LevelXSize); //convert width from level to minimap size
                    box.Height = (box.Height * MinimapSize.Y / LevelYSize); //convert height from level to minimap size
                    int XPositionOfLevel = (box.X - xLeftWall); //position of level when normalized as a proportion of its size
                    int YPositionOfLevel = (box.Y - (yFloor - CELL)); //returns distance of bottom (in game coords) of box from bottom of level

                    switch (MinimapPosition) {
                        case TOP_RIGHT:
                            box.X = GameConstants.X_RESOLUTION - (LevelXSize - XPositionOfLevel) * MinimapSize.X / LevelXSize - MinimapSideOffset; //convert from position in level to position in minimap
                            box.Y = MinimapSize.Y - YPositionOfLevel * MinimapSize.Y / LevelYSize - box.Height + MinimapSideOffset;
                            break;
                        case BOTTOM_RIGHT:
                            box.X = GameConstants.X_RESOLUTION - (LevelXSize - XPositionOfLevel) * MinimapSize.X / LevelXSize - MinimapSideOffset; //convert from position in level to position in minimap
                            box.Y = GameConstants.Y_RESOLUTION - YPositionOfLevel * MinimapSize.Y / LevelYSize - box.Height - MinimapSideOffset;
                            break;
                        case BOTTOM_LEFT:
                            box.X = XPositionOfLevel * MinimapSize.X / LevelXSize+ MinimapSideOffset; //convert from position in level to position in minimap
                            box.Y = GameConstants.Y_RESOLUTION - YPositionOfLevel * MinimapSize.Y / LevelYSize - box.Height -
                                MinimapSideOffset;
                            break;
                        }
/*                    box.X /= 4;
                    box.Y /= 4;
                    box.Width /= 4;
                    box.Height /= 4;
                    box.X += 400;
                    box.Y += 300;*/
                    //box.Y = GameConstants.Y_RESOLUTION - box.Y; //Jacob: convert game logic coords (+ is up, 0 is floor) to screen coords (+ is down, 0 is top)
                    if (obj.GetType() == typeof(Character))
                        spriteBatch.Draw(dummyTexture, box, Color.Blue * 0.8f);
                    else if (obj.GetType() == typeof(BasicBlock))
                        spriteBatch.Draw(dummyTexture, box, Color.DarkSlateGray * 0.8f);
                    else if (obj.GetType() == typeof(PickableBox))
                        spriteBatch.Draw(dummyTexture, box, Color.White * 0.8f);
                    else if (obj.GetType() == typeof(Door))
                        spriteBatch.Draw(dummyTexture, box, Color.Black * 0.8f);
                    else if (obj.GetType() == typeof(Button))
                        spriteBatch.Draw(dummyTexture, box, Color.ForestGreen * 0.8f);
                    else if (obj.GetType() == typeof(ToggleSwitch))
                        spriteBatch.Draw(dummyTexture, box, Color.Green * 0.8f);
                    else if (obj.GetType() == typeof(Enemy))
                    {
                        //Enemy enemy = (Enemy)obj;
                        //Rectangle e = enemy.enemyRangeL;
                        //e.Width = (e.Width * MinimapSize.X / LevelXSize); //convert width from level to minimap size
                        //e.Height = (e.Height * MinimapSize.Y / LevelYSize); //convert height from level to minimap size
                        //XPositionOfLevel = (e.X - xLeftWall); //position of level when normalized as a proportion of its size
                        //YPositionOfLevel = (e.Y - (yFloor - CELL)); //returns distance of bottom (in game coords) of box from bottom of level
                        //e.X = GameConstants.X_RESOLUTION - (LevelXSize - XPositionOfLevel) * MinimapSize.X / LevelXSize - MinimapSideOffset; //convert from position in level to position in minimap
                        //e.Y = MinimapSize.Y - YPositionOfLevel * MinimapSize.Y / LevelYSize - e.Height + MinimapSideOffset;


                        spriteBatch.Draw(dummyTexture, box, Color.Red * 0.8f);
                        //spriteBatch.Draw(dummyTexture, e, Color.Yellow * 0.8f);
                    }
                        
                    else if (obj.GetType() == typeof(LaserTurret))
                        spriteBatch.Draw(dummyTexture, box, Color.DarkRed * 0.8f);
                    else
                        spriteBatch.Draw(dummyTexture, box, Color.Purple * 0.8f);
                }
            }
        }

        /// <summary>
        /// Draws the visual feedback for the player when interacting with objects and carrying the box
        /// </summary>
        /// <param name="_renderContext"></param>
        public void DrawInteractiveUI(RenderContext _renderContext)
        {
            Character player = _renderContext.Player;

            // Provides a visual feedback if the user is able to pick up a box - Steven
            if (player.AdjacentObj != null && player.AdjacentObj.GetType() == typeof(PickableBox) && player.interactState == 0)
            {
                Vector3 screenPos = _renderContext.GraphicsDevice.Viewport.Project(
                    player.AdjacentObj.WorldPosition,
                    _renderContext.Camera.Projection, 
                    _renderContext.Camera.View, 
                    player.AdjacentObj.GetWorldMatrix());
                
                int offset = 30; // Offsets the B button and arrow to the center of the box
                
                spriteBatch.Draw(_renderContext.Textures["B_Button"], new Rectangle((int)screenPos.X - offset, (int)screenPos.Y, 48, 48), Color.White);
                // Added a bob to the arrow
                spriteBatch.Draw(_renderContext.Textures["Arrow"], new Rectangle((int)screenPos.X, (int)screenPos.Y - arrowBobEffect, 48, 48), Color.LawnGreen);
            }
             
            // Draws a B button when near switches
            else if (player.InteractiveObj != null && player.InteractiveObj.GetType() == typeof(ToggleSwitch))
            {
                Vector3 screenPos = _renderContext.GraphicsDevice.Viewport.Project(
                    player.InteractiveObj.WorldPosition,
                    _renderContext.Camera.Projection,
                    _renderContext.Camera.View,
                    player.InteractiveObj.GetWorldMatrix());
                ToggleSwitch toggleSwitch = (ToggleSwitch)player.InteractiveObj;

                // Stops drawing if the switch uses is used up
                if (player.interactState == 0 && toggleSwitch.RemainingToggles != 0 || toggleSwitch.InfinitelyToggleable)
                    spriteBatch.Draw(_renderContext.Textures["B_Button"], new Rectangle((int)screenPos.X - 24, (int)screenPos.Y - 96, 48, 48), Color.White);
            }

            // Provides visual feedback for where the box will be placed - Steven
            if (player.interactState != 0 && (!player.jumping || !player.falling))
            {
                Vector3 arrowPos;
                if (player.GetFacingDirection == 1) // Left
                    arrowPos = player.Position - new Vector3(24, 0, 0);
                else
                    arrowPos = player.Position + new Vector3(24, 0, 0);

                // Using Jacob's logic for readjusting the placed box - Steven
                float startX = (GameConstants.SINGLE_CELL_SIZE * 1) - (GameConstants.X_RESOLUTION / 2);
                float startY = (GameConstants.SINGLE_CELL_SIZE * 1) - (GameConstants.Y_RESOLUTION / 2);
                Vector3 CELLREMAINDER = new Vector3((arrowPos.X - startX) % GameConstants.SINGLE_CELL_SIZE,
                                                    (arrowPos.Y - startY) % GameConstants.SINGLE_CELL_SIZE,
                                                    arrowPos.Z);
                
                //Move positions to the nearest cell
                if (CELLREMAINDER.X < GameConstants.SINGLE_CELL_SIZE / 2)
                    arrowPos = new Vector3(arrowPos.X - CELLREMAINDER.X, arrowPos.Y, arrowPos.Z);
                else
                    arrowPos = new Vector3(arrowPos.X - CELLREMAINDER.X + GameConstants.SINGLE_CELL_SIZE, arrowPos.Y, arrowPos.Z);

                // Grabs the screen position for arrow in the world - Steven
                Matrix arrowWorldMatrix = player.GetWorldMatrix();
                arrowWorldMatrix.Translation = arrowPos;
                Vector3 screenPos = _renderContext.GraphicsDevice.Viewport.Project(player.WorldPosition,
                    _renderContext.Camera.Projection, _renderContext.Camera.View, arrowWorldMatrix);
                
                // Determines what color the arrow will be, will add logic to check whether the player can place the block - Steven
                Color color;
                
                if (player.canPlace)
                    color = Color.LawnGreen * 0.6f;
                else
                    color = Color.Red * 0.6f;

                int offset = 6; // Centers the arrow indicator to the center of a cell
                
                Vector2 origin = new Vector2(_renderContext.Textures["Arrow"].Bounds.Width / 2, _renderContext.Textures["Arrow"].Bounds.Height / 2);

                spriteBatch.Draw(_renderContext.Textures["Arrow"], new Rectangle((int)screenPos.X - offset, (int)screenPos.Y - arrowBobEffect, GameConstants.SINGLE_CELL_SIZE, GameConstants.SINGLE_CELL_SIZE), null, color,
                    0f, origin, SpriteEffects.FlipVertically, 0f);
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
                ScreenManager.AddScreen(new PauseMenuScreen(levelData.currentlevelNum), ControllingPlayer);
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
