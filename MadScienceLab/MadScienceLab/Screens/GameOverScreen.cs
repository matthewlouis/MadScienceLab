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

        GameplayScreen.LevelData levelData;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameOverScreen(GameplayScreen.LevelData levelData )
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
        

        #endregion
    }
}
