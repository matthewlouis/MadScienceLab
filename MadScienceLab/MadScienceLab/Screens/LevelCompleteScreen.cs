﻿#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

#endregion

namespace MadScienceLab
{
    /// <summary>
    /// The level complete screen shows statistcs of complete level
    /// allows the player to return to select level screen or progress to
    /// the next level directly
    /// </summary>
    class LevelCompleteScreen : MenuScreen
    {
        #region Fields

        // define menu entries and properties


        MenuEntry mainMenuEntry;
        MenuEntry nextLevelEntry;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public LevelCompleteScreen(GameplayScreen.LevelData levelData )
            : base("Level Complete")
        {
            // Create our menu entries.

            mainMenuEntry = new MenuEntry("Main Menu");
            nextLevelEntry = new MenuEntry("Next Level");

            // Hook up menu event handlers.
            mainMenuEntry.Selected += mainMenuEntrySelected;
            nextLevelEntry.Selected += nextLevelEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(mainMenuEntry);
            MenuEntries.Add(nextLevelEntry);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            
        }




        #endregion

        #region Handle Input

        void mainMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                               new MainMenuScreen());
        }

        void nextLevelEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(1));
        }
        

        #endregion
    }
}
