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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;

#endregion

namespace MadScienceLab
{
    /// <summary>
    /// The Level Select screen allows the user to select the level based on whether 
    /// the level has been unlocked or not.
    /// Level Select aslo indicated time trial mode and shows the time and par times for each level
    /// </summary>
    class LevelSelectScreen : MenuScreen
    {
        #region Fields
        //protected List<LevelSelectMenuEntry> menuEntries = new List<LevelSelectMenuEntry> ();

        // define menu entries and properties
        LevelSelectMenuEntry level1MenuEntry;
        LevelSelectMenuEntry level2MenuEntry;
        LevelSelectMenuEntry level3MenuEntry;
        LevelSelectMenuEntry level4MenuEntry;
        LevelSelectMenuEntry level5MenuEntry;
        LevelSelectMenuEntry level6MenuEntry;

        GameData saveGameData;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public LevelSelectScreen()
            : base("Level Select")
        {
            // get save data
            saveGameData = new GameData();
            

            // Create our menu entries.
            level1MenuEntry = new LevelSelectMenuEntry ( "Level 1" );
            level2MenuEntry = new LevelSelectMenuEntry ( "Level 2" );
            level3MenuEntry = new LevelSelectMenuEntry ( "Level 3" );
            level4MenuEntry = new LevelSelectMenuEntry ( "Level 4" );
            level5MenuEntry = new LevelSelectMenuEntry ( "Level 5" );
            level6MenuEntry = new LevelSelectMenuEntry ( "Level 6" );

            MenuEntry back = new LevelSelectMenuEntry ( "Main Menu" );

            // Hook up menu event handlers.
            level1MenuEntry.Selected += level1MenuEntrySelected;
            level2MenuEntry.Selected += level2MenuEntrySelected;
            level3MenuEntry.Selected += level3MenuEntrySelected;
            level4MenuEntry.Selected += level4MenuEntrySelected;
            level5MenuEntry.Selected += level5MenuEntrySelected;
            level6MenuEntry.Selected += level6MenuEntrySelected;
            back.Selected += OnCancel;

            // Add entries to the menu.

            MenuEntries.Add(level1MenuEntry);
            MenuEntries.Add(level2MenuEntry);
            MenuEntries.Add(level3MenuEntry);
            MenuEntries.Add(level4MenuEntry);
            MenuEntries.Add ( level5MenuEntry );
            MenuEntries.Add ( level6MenuEntry );
            MenuEntries.Add(back);
        }

        
        #endregion

        #region Update and Draw
        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected override void UpdateMenuEntryLocations ()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow ( TransitionPosition, 2 );

            // start at Y = 175; each X value is generated per entry
            Vector2 position = new Vector2 ( 0f, 310f ); //Modified from 330f

            // update each menu entry's location in turn
            for (int i = 0; i < menuEntries.Count; i++)
            {
                LevelSelectMenuEntry menuEntry = (LevelSelectMenuEntry)menuEntries[i];

                // each entry is to be centered horizontally
                position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - menuEntry.GetWidth ( this ) / 2;

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                // set the entry's position
                menuEntry.Position = position;

                // move down for the next entry the size of this entry
                position.Y += menuEntry.GetHeight ( this );
            }
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw ( GameTime gameTime )
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations ();

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            spriteBatch.Begin ();

            // Draw each menu entry in turn.
            for (int i = 0; i < base.menuEntries.Count; i++)
            {
                LevelSelectMenuEntry menuEntry = (LevelSelectMenuEntry)menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw ( this, isSelected, gameTime );
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow ( TransitionPosition, 2 );

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2 ( graphics.Viewport.Width / 2, 260 );
            Vector2 titleOrigin = font.MeasureString ( menuTitle ) / 2;
            Color titleColor = new Color ( 192, 192, 192 ) * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString ( font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0 );

            spriteBatch.End ();
        }

        #endregion

        #region Handle Input

        void level1MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(1));
        }
        

        void level2MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(2));
        }

        void level3MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(3));
        }

        void level4MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(4));
        }

        void level5MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(5));
        }

        void level6MenuEntrySelected ( object sender, PlayerIndexEventArgs e )
        {
            LoadingScreen.Load ( ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen ( 6 ) );
        }



        #endregion
    }
}
