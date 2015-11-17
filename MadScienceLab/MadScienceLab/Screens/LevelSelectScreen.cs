#region File Description
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
using System.Collections.Generic;

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

        // define menu entries and properties
        MenuEntry level1MenuEntry;
        MenuEntry level2MenuEntry;
        MenuEntry level3MenuEntry;
        MenuEntry level4MenuEntry;
        MenuEntry level5MenuEntry;
        
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public LevelSelectScreen()
            : base("Level Select")
        {
            // Create our menu entries.
            level1MenuEntry = new MenuEntry("Level 1");
            level2MenuEntry = new MenuEntry("Level 2");
            level3MenuEntry = new MenuEntry("Level 3");
            level4MenuEntry = new MenuEntry("Level 4");
            level5MenuEntry = new MenuEntry("Level 5");

            MenuEntry back = new MenuEntry("Main Menu");

            // Hook up menu event handlers.
            level1MenuEntry.Selected += level1MenuEntrySelected;
            level2MenuEntry.Selected += level2MenuEntrySelected;
            level3MenuEntry.Selected += level3MenuEntrySelected;
            level4MenuEntry.Selected += level4MenuEntrySelected;
            level5MenuEntry.Selected += level5MenuEntrySelected;
            back.Selected += OnCancel;

            // Add entries to the menu.

            MenuEntries.Add(level1MenuEntry);
            MenuEntries.Add(level2MenuEntry);
            MenuEntries.Add(level3MenuEntry);
            MenuEntries.Add(level4MenuEntry);
            MenuEntries.Add(level5MenuEntry);
            MenuEntries.Add(back);
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



        #endregion
    }
}
