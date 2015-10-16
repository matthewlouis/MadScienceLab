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
        
        

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        RenderContext _renderContext;
        BaseCamera _camera;

        Level basicLevel;

        //Note: Add fields for player, background etc. here
        public static Dictionary<String, Model> _models = new Dictionary<string,Model>();
        public static Dictionary<String, Texture2D> _textures = new Dictionary<string, Texture2D>();

        

        Character player;
        

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
            _models.Add("Turret", Content.Load<Model>("turret"));

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
            player.TransAccel = new Vector3(0, -GameConstants.SINGLE_CELL_SIZE * 9, 0);
            font = Content.Load<SpriteFont>("Verdana");
            _renderContext.Level = basicLevel;

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

            _renderContext.GameTime = gameTime;
            //Update physics
            //Just player and level object physics for now
            UpdatePhysics ( player );
            player.Update(_renderContext);
                        
            player.AdjacentObj = null; //reset to null after checking PickBox, and before the adjacentObj is updated

            _renderContext.GameTime = gameTime;
            _camera.Update(_renderContext);
            basicLevel.Update(_renderContext);
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

            player.Draw(_renderContext);
            basicLevel.Draw(_renderContext);
            /*
            spriteBatch.Begin();
            spriteBatch.DrawString(font, DebugCheckPlayerBoxCollision().ToString(), new Vector2(50, 50), Color.Black);
            spriteBatch.DrawString(font, "Velocity: " + player.TransVelocity.ToString(), new Vector2(50, 100), Color.Black);
            spriteBatch.DrawString(font, "Acceleration: " + player.TransAccel.ToString(), new Vector2(50, 200), Color.Black);
            spriteBatch.DrawString(font, "Box: " + brick.ToString(), new Vector2(50, 250), Color.Black);
            spriteBatch.DrawString(font, boxHitState, new Vector2(50, 150), Color.Black);
            spriteBatch.End();*/

            // Spritebatch changes graphicsdevice values; sets the oringinal state
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            base.Draw(gameTime);
        }



        /// <summary>
        /// Checks which side the intersect occured on the player and handles it
        /// </summary>
        

        public void PutBox()
        {
            //if (/*player.adjacentObj == null*/) //will need a condition for when the adjacent area where the player would be trying to put the box is empty,
            //{
                player.interactState = Character.InteractState.StartingDropBox; //state for while the player begins putting down the box
            //}
        }

        
    }

}
