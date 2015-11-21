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
        Rectangle quadRect;
        public static Level CurrentLevel { get; private set; }

        SpriteBatch spriteBatch;

        ContentManager content;

        RenderContext _renderContext;
        BaseCamera _camera;
        GameTimer _timer;
        int bob = 0;
        int num = 1;
        private SoundEffect levelMusic;

        Level basicLevel;

        //Note: Add fields for player, background etc. here
        public static Dictionary<String, Model> _models;
        public static Dictionary<String, Texture2D> _textures;
        public static Dictionary<String, SoundEffect> _sounds;

        Character player;

        // Debugging - Steven
        SpriteFont font;
        List<GameObject3D> debugHitbox;
        Texture2D dummyTexture;

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
                _textures.Add("BareMetal_Gray", content.Load<Texture2D>("Textures/BareMetal_Gray"));
                _textures.Add("BrushedRoundMetal_Gray", content.Load<Texture2D>("Textures/BrushedRoundMetal_Gray"));
                _textures.Add("DirtyMetal", content.Load<Texture2D>("Textures/DirtyMetal"));
                _textures.Add("Fiberglass_White", content.Load<Texture2D>("Textures/Fiberglass_White"));
                _textures.Add("MetalFloor_Gray", content.Load<Texture2D>("Textures/MetalFloor_Gray"));
                _textures.Add("Tile_Beige", content.Load<Texture2D>("Textures/Tile_Beige"));
                _textures.Add("Tile_Blue", content.Load<Texture2D>("Textures/Tile_Blue"));
                _textures.Add("Tile_DarkGray", content.Load<Texture2D>("Textures/Tile_DarkGray"));
                _textures.Add("Tile_Gray", content.Load<Texture2D>("Textures/Tile_Gray"));
                _textures.Add("WindowBlocks", content.Load<Texture2D>("Textures/WindowBlocks"));
                _textures.Add("Tile_Fun", content.Load<Texture2D>("Textures/Tile_Fun"));
                _textures.Add("Exit", content.Load<Texture2D>("Textures/EXIT"));
                _textures.Add("Complete", content.Load<Texture2D>("Textures/Complete"));
                _textures.Add("GameOver", content.Load<Texture2D>("Textures/GameOver"));
                _textures.Add("Arrow", content.Load<Texture2D>("Textures/Arrow"));
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
                basicLevel = LevelBuilder.MakeBasicLevel(levelData.currentlevelNum);
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

                // Debugging - Steven
                debugHitbox = new List<GameObject3D>();
                debugHitbox.AddRange(_renderContext.Level.collidableObjects);
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
            if (IsActive)
            {
                if (bob > 10)
                    num = -1;
                else if (bob < -10)
                    num = 1;

                bob += (int)(0.1f * gameTime.ElapsedGameTime.Milliseconds) * num;

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
                    levelData.time = _timer.ElapsedTime.ToString ();
                    levelData.remainingHealth = _renderContext.Player.GetHealth();
                    LoadingScreen.Load(ScreenManager, false, null, new LevelCompleteBackgroundScreen(),
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
            if (_renderContext.Player.AdjacentObj != null && _renderContext.Player.AdjacentObj.GetType() == typeof(PickableBox) 
                && _renderContext.Player.interactState == 0)
            {
                Vector3 screenPos = _renderContext.GraphicsDevice.Viewport.Project(_renderContext.Player.AdjacentObj.WorldPosition,
                    _renderContext.Camera.Projection, _renderContext.Camera.View, _renderContext.Player.AdjacentObj.GetWorldMatrix());
                spriteBatch.Draw(_textures["Arrow"], new Rectangle((int)screenPos.X - 24, (int)screenPos.Y - bob, 48, 48), Color.LawnGreen);
            }
            if (_renderContext.Player.interactState != 0 && (!_renderContext.Player.jumping || !_renderContext.Player.falling))
            {
                Vector3 arrowPos; 
                if (_renderContext.Player.GetFacingDirection == 1)
                    arrowPos = _renderContext.Player.Position - new Vector3(24, 0, 0);
                else
                    arrowPos = _renderContext.Player.Position + new Vector3(24, 0, 0);



                //if (_renderContext.Player.GetFacingDirection == 1)
                //    arrowPos += new Vector3(0, 0, _renderContext.Player.Position.X % GameConstants.SINGLE_CELL_SIZE);
                //else
                //    arrowPos -= new Vector3(0, 0, (_renderContext.Player.Position.X) % GameConstants.SINGLE_CELL_SIZE - 48);

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

                Matrix arrowWorldMatrix = _renderContext.Player.GetWorldMatrix();
                arrowWorldMatrix.Translation = arrowPos;
                Vector3 screenPos = _renderContext.GraphicsDevice.Viewport.Project(_renderContext.Player.WorldPosition,
                    _renderContext.Camera.Projection, _renderContext.Camera.View, arrowWorldMatrix);
                Color color;
 
                color = Color.LawnGreen;

                spriteBatch.Draw(_textures["Arrow"], new Rectangle((int)screenPos.X - 6, (int)screenPos.Y - bob, 48, 48), null, color,
                    0f, new Vector2(_textures["Arrow"].Bounds.Width /2, _textures["Arrow"].Bounds.Height /2), SpriteEffects.FlipVertically, 0f);
            }

            //spriteBatch.DrawString(font, "Time: " + timer, new Vector2(300, 50), Color.Black);
            //spriteBatch.DrawString(font, "Velocity: " + player.TransVelocity.ToString(), new Vector2(50, 100), Color.Black);
            //spriteBatch.DrawString(font, "Acceleration: " + player.TransAccel.ToString(), new Vector2(50, 200), Color.Black);
            //spriteBatch.DrawString(font, "Box: " + brick.ToString(), new Vector2(50, 250), Color.Black);
            //if (_renderContext.CurrMsgEvent != null)
            //    spriteBatch.DrawString(font, "MessageEvent: " + _renderContext.CurrMsgEvent.typedMessage, new Vector2(50, 250), Color.Black);
            //spriteBatch.DrawString(font, boxHitState, new Vector2(50, 150), Color.Black);
            //spriteBatch.DrawString(font, "Projectile: " + basicLevel.Children[basicLevel.Children.Count()-1].Hitbox.ToString(), new Vector2(50, 150), Color.Black);
            //spriteBatch.DrawString(font, "Player pos: " + player.Position.ToString(), new Vector2(50, 300), Color.Black);
            spriteBatch.End();

            //Jacob: Added some tweaks to minimap positioning.
            if (player.MapEnabled)
            {
                spriteBatch.Begin ();
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
                        spriteBatch.Draw(dummyTexture, box, Color.Red * 0.8f);
                    else if (obj.GetType() == typeof(LaserTurret))
                        spriteBatch.Draw(dummyTexture, box, Color.DarkRed * 0.8f);
                    else
                        spriteBatch.Draw(dummyTexture, box, Color.Purple * 0.8f);
                }

                Rectangle qtbox = quadRect;
                
                qtbox.X /= 2;
                qtbox.Y /= 2;
                qtbox.Width /= 2;
                qtbox.Height /= 2;
                qtbox.X += 400;
                qtbox.Y += 500;
                spriteBatch.Draw(dummyTexture, qtbox, Color.Brown * 0.8f);
                //Console.WriteLine(qtbox.ToString());
                if (_renderContext.QuadtreeDebug != null)
                foreach (CellObject qBox in _renderContext.QuadtreeDebug)
                {
                    Rectangle box = qBox.Hitbox;
                    box.X /= 2;
                    box.Y /= 2;
                    box.Width /= 2;
                    box.Height /= 2;
                    box.X += 400;
                    box.Y += 500;
                    spriteBatch.Draw(dummyTexture, box, Color.Black * 0.8f);
                    
                }

                Rectangle lbox = _renderContext.Level.Hitbox;
                lbox.X /= 2;
                lbox.Y /= 2;
                lbox.Width /= 2;
                lbox.Height /= 2;
                lbox.X += 400;
                lbox.Y += 500;
                spriteBatch.Draw(dummyTexture, lbox, Color.Black * 0.8f);


                int i = 0;
                //foreach (Rectangle box in _renderContext.BoxesHit)
                //{

                //    Rectangle boxhit = box;
                //    boxhit.X += 400;
                //    boxhit.Y -= 500;
                //    boxhit.X /= 2;
                //    boxhit.Y /= -2;
                //    boxhit.Width /= 2;
                //    boxhit.Height /= 2;
                //    if (i == 0)
                //        spriteBatch.Draw(dummyTexture, boxhit, Color.Red * 0.8f);
                //    if (i == 1)
                //        spriteBatch.Draw(dummyTexture, boxhit, Color.Purple * 0.8f);
                //    if (i == 2)
                //        spriteBatch.Draw(dummyTexture, boxhit, Color.Blue * 0.8f);
                //    i++;
                //}
                    spriteBatch.End();
            }
            //Console.WriteLine(_renderContext.Player.Position.ToString());
            //fpsCount.Draw(gameTime);
            fpsCount.Draw(gameTime);
            //_timer.Draw(_renderContext.GameTime);
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
