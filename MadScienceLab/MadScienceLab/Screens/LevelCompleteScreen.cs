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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

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
        MenuEntry currentLevelEntry;
        MenuEntry timeCompleteEntry;
        MenuEntry remaingingHealthEntry;
        MenuEntry levelScoreEntry;
        MenuEntry highScoreEntry;
        MenuEntry mainMenuEntry;
        MenuEntry nextLevelEntry;

        GameData.LevelData levelData;
        GameData gameData;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public LevelCompleteScreen(GameData.LevelData levelData )
            : base("Level Complete")
        {
            this.levelData = levelData;
            gameData = new GameData();
            if (gameData.saveGameData.levelCompleted < levelData.currentlevelNum)
                gameData.saveGameData.levelCompleted = levelData.currentlevelNum;
            gameData.saveGameData.levelData[levelData.currentlevelNum-1].time = levelData.time.ToString();

            int levelScore = 0;
            levelScore += (int)GameConstants.SCORE.levelComplete; // add score for completing level
            // Score for health
            if(levelData.remainingHealth == GameConstants.HEALTH)
            {
                levelScore += (int)GameConstants.SCORE.fullHealth;
            }
            if (levelData.remainingHealth == GameConstants.HEALTH-1)
            {
                levelScore += (int)GameConstants.SCORE.medHealth;
            }
            if (levelData.remainingHealth == GameConstants.HEALTH-2)
            {
                levelScore += (int)GameConstants.SCORE.lowHealth;
            }

            //if(new TimeSpan( levelData.time < levelData.levelParTime)
            //{
            //    score += GameConstants.SCORE.parTimeComplete;
            //}
            

            gameData.saveGameData.score += levelScore;
            if(gameData.saveGameData.levelData[levelData.currentlevelNum - 1].levelHighScore < levelScore)
                gameData.saveGameData.levelData[levelData.currentlevelNum - 1].levelHighScore = levelScore;
            gameData.GetContainer();
            gameData.Save();

            // Create our menu entries.
            string time = string.Format("Time Completed: {0}", levelData.time);
            time = time.Remove(time.Length - 8);

            currentLevelEntry = new MenuEntry("Level " + levelData.currentlevelNum);
            timeCompleteEntry = new MenuEntry(time);
            remaingingHealthEntry = new MenuEntry("Remaining Health: " + levelData.remainingHealth.ToString());
            levelScoreEntry = new MenuEntry("Level Score: " + levelScore);
            highScoreEntry = new MenuEntry("Game Score: " + gameData.saveGameData.score.ToString());
            mainMenuEntry = new MenuEntry("Main Menu");
            nextLevelEntry = new MenuEntry("Next Level");

            // Hook up menu event handlers.
            mainMenuEntry.Selected += mainMenuEntrySelected;
            nextLevelEntry.Selected += nextLevelEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(currentLevelEntry);
            MenuEntries.Add(timeCompleteEntry);
            MenuEntries.Add(remaingingHealthEntry);
            MenuEntries.Add(levelScoreEntry);
            MenuEntries.Add(highScoreEntry);
            
            // check to see if there are still levels left before adding nextLevelEntry
            if (levelData.currentlevelNum < GameConstants.LEVELS)
            {
                MenuEntries.Add(nextLevelEntry);
            }
            // load credits
            else if (levelData.currentlevelNum == GameConstants.LEVELS)
            {
                // load game complete screen with credits if all levels are finished
                //LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                //               new GameCompleteScreen());
            }



            MenuEntries.Add(mainMenuEntry);



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
                           new GameplayScreen(++levelData.currentlevelNum));            
        }
        

        #endregion
    }
}
