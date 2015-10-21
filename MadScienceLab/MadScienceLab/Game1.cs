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
        public static Level CurrentLevel { get; private set; }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        RenderContext _renderContext;
        BaseCamera _camera;

        Level basicLevel;

        //Note: Add fields for player, background etc. here
        public static Dictionary<String, Model> _models = new Dictionary<string,Model>();
        public static Dictionary<String, Texture2D> _textures = new Dictionary<string, Texture2D>();

        Character player;
        Enemy enemy;
        
        // Debugging - Steven
        private String boxHitState = "";
        SpriteFont font;
        private Rectangle brick;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60.0f);

            Window.Title = "Group_Project";
            graphics.PreferredBackBufferWidth = GameConstants.X_RESOLUTION;
            graphics.PreferredBackBufferHeight = GameConstants.Y_RESOLUTION;
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
            _renderContext.GameTime = new GameTime();

            _renderContext.SpriteBatch = spriteBatch;
            _renderContext.GraphicsDevice = graphics.GraphicsDevice;

            // TODO: use this.Content to load your game content here
            //_models.Add("player", Content.Load<Model>("scientist"));
            _models.Add ( "BasicBlock", Content.Load<Model> ( "BasicBlock" ) );
            _models.Add("BackgroundBlock", Content.Load<Model>("BackgroundBlock"));
            _models.Add("button", Content.Load<Model>("Button"));
            _models.Add("door", Content.Load<Model>("Door"));
            _models.Add("switch", Content.Load<Model>("ToggleSwitch"));
            _models.Add("MoveableBox", Content.Load<Model>("MoveableBox"));
            _models.Add("BlockDropper", Content.Load<Model>("BlockDropper"));
            _models.Add("BlockDropper_Empty", Content.Load<Model>("BlockDropper_Empty"));
            _models.Add("block", Content.Load<Model>("block"));
            _models.Add("Turret", Content.Load<Model>("turret"));
            _models.Add("projectile", Content.Load<Model>("projectile"));

            _textures.Add("MoveableBox", Content.Load<Texture2D>("WoodPlanks_Color"));
            _textures.Add("BlockDropper", Content.Load<Texture2D>("Textures/dropper"));
            _textures.Add("BareMetal_Gray", Content.Load<Texture2D>("Textures/BareMetal_Gray"));
            _textures.Add("BrushedRoundMetal_Gray", Content.Load<Texture2D>("Textures/BrushedRoundMetal_Gray"));
            _textures.Add("DirtyMetal", Content.Load<Texture2D>("Textures/DirtyMetal"));
            _textures.Add("Fiberglass_White", Content.Load<Texture2D>("Textures/Fiberglass_White"));
            _textures.Add("MetalFloor_Gray", Content.Load<Texture2D>("Textures/MetalFloor_Gray"));
            _textures.Add("Tile_Beige", Content.Load<Texture2D>("Textures/Tile_Beige_Half"));
            _textures.Add("Tile_Blue", Content.Load<Texture2D>("Textures/Tile_Blue"));
            _textures.Add("Tile_DarkGray", Content.Load<Texture2D>("Textures/Tile_DarkGray"));
            _textures.Add("Tile_Gray", Content.Load<Texture2D>("Textures/Tile_Gray"));
            _textures.Add("WindowBlocks", Content.Load<Texture2D>("Textures/WindowBlocks"));
            _textures.Add("Tile_Fun", Content.Load<Texture2D>("Textures/Tile_Fun"));
            _textures.Add("Exit", Content.Load<Texture2D>("Textures/EXIT"));
            _textures.Add("Complete", Content.Load<Texture2D>("Textures/Complete"));
            _textures.Add("GameOver", Content.Load<Texture2D>("Textures/GameOver"));

            //loads the basic level
            basicLevel = LevelBuilder.MakeBasicLevel ();
            CurrentLevel = basicLevel; //we can handle this through render context eventually.

            player = new Character(basicLevel.PlayerPoint.X, basicLevel.PlayerPoint.Y);
            player.LoadContent(Content);
            _renderContext.Player = player;
            _camera.setFollowTarget(player);
            player.TransAccel = new Vector3(0, -GameConstants.SINGLE_CELL_SIZE * 9, 0);
            font = Content.Load<SpriteFont>("Verdana");
            _renderContext.Level = basicLevel;

            basicLevel.PopulateTypeList(_renderContext);
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                //loads the basic level
                basicLevel = LevelBuilder.MakeBasicLevel();
                CurrentLevel = basicLevel; //we can handle this through render context eventually.

                player = new Character(basicLevel.PlayerPoint.X, basicLevel.PlayerPoint.Y);
                player.LoadContent(Content);
                _renderContext.Player = player;
                _camera.setFollowTarget(player);
                player.TransAccel = new Vector3(0, -GameConstants.SINGLE_CELL_SIZE * 9, 0);
                font = Content.Load<SpriteFont>("Verdana");
                _renderContext.Level = basicLevel;

                basicLevel.PopulateTypeList(_renderContext);
            }

            if (!basicLevel.GameOver && !basicLevel.LevelOver)
            {
                _renderContext.GameTime = gameTime;
                //Update physics
                //Just player and level object physics for now
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

                base.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            player.Draw(_renderContext);
            basicLevel.Draw(_renderContext);
            
            spriteBatch.Begin();
            //spriteBatch.DrawString(font, DebugCheckPlayerBoxCollision().ToString(), new Vector2(50, 50), Color.Black);
            spriteBatch.DrawString(font, "Health: " + player.GetHealth().ToString(), new Vector2(50, 50), Color.Black);
            //spriteBatch.DrawString(font, "Velocity: " + player.TransVelocity.ToString(), new Vector2(50, 100), Color.Black);
            //spriteBatch.DrawString(font, "Acceleration: " + player.TransAccel.ToString(), new Vector2(50, 200), Color.Black);
            //spriteBatch.DrawString(font, "Box: " + brick.ToString(), new Vector2(50, 250), Color.Black);
            //spriteBatch.DrawString(font, boxHitState, new Vector2(50, 150), Color.Black);
            //spriteBatch.DrawString(font, "Projectile: " + basicLevel.Children[basicLevel.Children.Count()-1].Hitbox.ToString(), new Vector2(50, 150), Color.Black);
            //spriteBatch.DrawString(font, "Player pos: " + player.Position.ToString(), new Vector2(50, 300), Color.Black);
            spriteBatch.End();

            if (_renderContext.Level.LevelOver)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(Game1._textures["Complete"], new Vector2(GraphicsDevice.Viewport.Width/2 - Game1._textures["Complete"].Width / 2, GraphicsDevice.Viewport.Height/2 - Game1._textures["Complete"].Height / 2), Color.White);
                spriteBatch.End();
            }

            if (_renderContext.Level.GameOver)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(Game1._textures["GameOver"], new Vector2(GraphicsDevice.Viewport.Width / 2 - Game1._textures["GameOver"].Width / 2, GraphicsDevice.Viewport.Height / 2 - Game1._textures["GameOver"].Height / 2), Color.White);
                spriteBatch.End();
            }
            
            // Spritebatch changes graphicsdevice values; sets the oringinal state
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            base.Draw(gameTime);
        }

        private void UpdatePhysics(GameObject3D Object)
        {
            Object.TransVelocity += Object.TransAccel / 60; //amt. accel (where TransAccel is in seconds) per frame ...
            Object.Translate(Object.Position + Object.TransVelocity / 60);
        }
    }

}
