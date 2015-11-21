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
    class StoryScreen : MenuScreen
    {
        #region Fields
        //protected List<LevelSelectMenuEntry> menuEntries = new List<LevelSelectMenuEntry> ();

        // define menu entries and properties
        VarSizeMenuEntry StorylineEntry;
        VarSizeMenuEntry BlankEntry;
        VarSizeMenuEntry ContinueEntry;
        List<String> Storyline;
        List<String> StoryTitles;
        float fontScale;
        GameScreen[] DestScreensToLoad;
        bool lastStoryScreen;

        GameData saveGameData;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public StoryScreen ( List<String> storytext, List<String> titletext, float fontScale, GameData saveData, params GameScreen[] DestScreensToLoad )
            : base(titletext[0])
        {
            saveGameData = saveData;
            this.DestScreensToLoad = DestScreensToLoad;
            this.fontScale = fontScale;

            //SpriteFont font = ScreenManager.Font; //Get font of the ScreenManager
            // Create our menu entries.
            Storyline = storytext;
            StoryTitles = titletext;

            StorylineEntry = new VarSizeMenuEntry ( Storyline[0] , fontScale);
            BlankEntry = new VarSizeMenuEntry ( " ", (fontScale * 5/6));
            MenuEntry ContinueEntry = new VarSizeMenuEntry ( "Continue", 1f);

            // Hook up menu event handlers.
            ContinueEntry.Selected += continueSelected;

            // Add entries to the menu.
            MenuEntries.Add ( StorylineEntry );
            MenuEntries.Add ( BlankEntry );
            MenuEntries.Add ( ContinueEntry );
            //Select the first selectable entry
            while (menuEntries[selectedEntry].HasNoHandle)
            {
                selectedEntry++;
            }
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
            Vector2 position = new Vector2 ( 0f, 310f ); //Modified from 330f, which was the original value

            // update each menu entry's location in turn
            int i;
            for (i=0; i < menuEntries.Count; i++) //iterate for all entries except for [i].
            {
                MenuEntry menuEntry = menuEntries[i];

                // each entry is to be centered horizontally, but not in LevelSelect - alignment is to be left.
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
                /*LevelSelectMenuEntry menuBackEntry = (LevelSelectMenuEntry)menuEntries[i];

                // the Back is to be centered horizontally.
                position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - menuBackEntry.GetWidth ( this ) / 2;

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                // set the entry's position
                menuBackEntry.Position = position;

                // move down for the next entry the size of this entry
                position.Y += menuBackEntry.GetHeight ( this );*/
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
                MenuEntry menuEntry = menuEntries[i];

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

        void continueSelected(object sender, PlayerIndexEventArgs e)
        {
            List<String> RestOfStoryline = new List<String> ( Storyline );
            List<String> RestOfTitles = new List<String> ( StoryTitles );
            RestOfStoryline.RemoveAt ( 0 );
            RestOfTitles.RemoveAt ( 0 );
            if (RestOfStoryline.Count == 0)
            {
                LoadingScreen.Load ( ScreenManager, true, e.PlayerIndex,
                               DestScreensToLoad ); // loads next level from currentLevel
            }
            else
            {
                ScreenManager.AddScreen ( new StoryScreen ( RestOfStoryline, RestOfTitles, fontScale, saveGameData, this.DestScreensToLoad ), e.PlayerIndex );
            }
        }

        protected override void OnCancel ( PlayerIndex playerIndex )
        {
            //Do nothing. The player should go through the story without skipping or going back.
        }

        #endregion
    }
}
