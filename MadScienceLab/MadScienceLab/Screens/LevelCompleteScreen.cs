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
    class LevelCompleteScreen : MenuScreen
    {
        #region Fields

        // define menu entries and properties

        MenuEntry timeCompleteEntry;
        MenuEntry remaingingHealthEntry;
        MenuEntry mainMenuEntry;
        MenuEntry nextLevelEntry;

        GameplayScreen.LevelData levelData;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public LevelCompleteScreen(GameplayScreen.LevelData levelData )
            : base("Level Complete")
        {
            this.levelData = levelData;

            // Create our menu entries.
            string time = string.Format("Time Completed: {0}", levelData.time);
            time = time.Remove(time.Length - 8);

            timeCompleteEntry = new MenuEntry(time);
            remaingingHealthEntry = new MenuEntry("Remaining Health: " + levelData.remainingHealth.ToString());
            mainMenuEntry = new MenuEntry("Main Menu");
            nextLevelEntry = new MenuEntry("Next Level");

            // Hook up menu event handlers.
            mainMenuEntry.Selected += mainMenuEntrySelected;
            nextLevelEntry.Selected += nextLevelEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(timeCompleteEntry);
            MenuEntries.Add(remaingingHealthEntry);
            MenuEntries.Add(mainMenuEntry);

            // check to see if there are still levels left
            if (levelData.currentlevelNum < GameConstants.LEVELS)
            {
                MenuEntries.Add(nextLevelEntry);

            }
            else if (levelData.currentlevelNum == GameConstants.LEVELS)
            {
                // load game complete screen with credits if all levels are finished
                //LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                //               new GameCompleteScreen());
            }
            
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
                           new GameplayScreen(levelData.currentlevelNum++));

            
        }
        

        #endregion
    }
}
