#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace MadScienceLab
{
    /// <summary>
    /// The level complete screen shows statistcs of complete level
    /// allows the player to return to select level screen or progress to
    /// the next level directly
    /// </summary>
    class GameOverScreen : MenuScreen
    {
        #region Fields

        // define menu entries and properties


        MenuEntry mainMenuEntry;
        MenuEntry retryLevelEntry;

        #endregion

        #region Initialization

        GameData.LevelData levelData;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameOverScreen(GameData.LevelData levelData )
            : base("Game Over")
        {
            // Create our menu entries.
            retryLevelEntry = new MenuEntry("Retry Level");
            mainMenuEntry = new MenuEntry("Main Menu");

            // Hook up menu event handlers.
            mainMenuEntry.Selected += mainMenuEntrySelected;
            retryLevelEntry.Selected += retryLevelEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(retryLevelEntry);
            MenuEntries.Add(mainMenuEntry);
            

            this.levelData = levelData;
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;
            Texture2D actionButton = ScreenManager.ActionButtonTexture;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < base.menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, isSelected, gameTime);
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 260);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            //Draw select and back buttons
            spriteBatch.Draw(actionButton, new Rectangle(960, 540, 50, 50), Color.White);
            spriteBatch.DrawString(font, "Select", new Vector2(1020, 550), Color.White);

            spriteBatch.End();
        }


        #endregion

        #region Handle Input

        void mainMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                               new MainMenuScreen());
        }

        void retryLevelEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(levelData.currentlevelNum));
        }

        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            //Do nothing.
        }

        #endregion
    }
}
