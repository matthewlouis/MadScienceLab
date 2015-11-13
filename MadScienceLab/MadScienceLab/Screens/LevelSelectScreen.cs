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
        MenuEntry levelMenuEntry;

        static bool sound = true;
        static bool music = false;

        
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public LevelSelectScreen()
            : base("LevelSelect")
        {
            // Create our menu entries.
            levelMenuEntry = new MenuEntry("Level 1");


            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Main Menu");

            // Hook up menu event handlers.
            levelMenuEntry.Selected += levelMenuEntrySelected;
            back.Selected += OnCancel;

            // Add entries to the menu.

            MenuEntries.Add(levelMenuEntry);
            MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            levelMenuEntry.Text = ("Level 1");
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
