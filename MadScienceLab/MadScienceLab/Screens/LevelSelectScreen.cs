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
            level1MenuEntry.Selected += levelMenuEntrySelected;
            level2MenuEntry.Selected += levelMenuEntrySelected;
            level3MenuEntry.Selected += levelMenuEntrySelected;
            level4MenuEntry.Selected += levelMenuEntrySelected;
            level5MenuEntry.Selected += levelMenuEntrySelected;
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

        void levelMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen("level1"));
            
        }



        #endregion
    }
}
