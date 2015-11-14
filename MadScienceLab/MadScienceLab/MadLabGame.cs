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
using GameStateManagement;

namespace MadScienceLab
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MadLabGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        ScreenFactory screenFactory;

        public static Level CurrentLevel { get; private set; }

        SpriteBatch spriteBatch;

        RenderContext _renderContext;
        BaseCamera _camera;

        Level basicLevel;

        //Note: Add fields for player, background etc. here
        public static Dictionary<String, Model> _models = new Dictionary<string, Model>();
        public static Dictionary<String, Texture2D> _textures = new Dictionary<string, Texture2D>();
        public static Dictionary<String, SoundEffect> _sounds = new Dictionary<string, SoundEffect>();

        Character player;
        Enemy enemy;

        // Debugging - Steven
        private String boxHitState = "";
        SpriteFont font;
        private Rectangle brick;

        //Debugging - FPS - Matt
        private FPSCounter fpsCount;

        /// <summary>
        /// The main game constructor
        /// </summary>
        public MadLabGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60.0f);

            // Create Frame Counter
            Components.Add(new FPSCounter(this, _renderContext));
            
            // Setup window
            Window.Title = "MadLab";
            graphics.PreferredBackBufferWidth = GameConstants.X_RESOLUTION;
            graphics.PreferredBackBufferHeight = GameConstants.Y_RESOLUTION;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();


            // Create the screen factory and add it to the Services
            screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            // Create the screen manager component.
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            
            // On Windows and Xbox we just add the initial screens
            AddInitialScreens();
        }

        private void AddInitialScreens()
        {
            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);

            // temp go to game screen
            //screenManager.AddScreen(new GameplayScreen(), null);

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);


            base.Draw(gameTime);
        }

    }
}
