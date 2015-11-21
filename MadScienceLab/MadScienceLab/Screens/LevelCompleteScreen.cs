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
using GameStateManagement;

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
        MenuEntry blankEntry;
        MenuEntry mainMenuEntry;
        MenuEntry continueEntry;

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

            currentLevelEntry = new MenuEntry("\""+GameConstants.LEVELNAMES[levelData.currentlevelNum-1]+"\"");
            //timeCompleteEntry = new MenuEntry(time);
            remaingingHealthEntry = new MenuEntry("Remaining Health: " + levelData.remainingHealth.ToString());
            levelScoreEntry = new MenuEntry("Level Score: " + levelScore);
            highScoreEntry = new MenuEntry("Game Score: " + gameData.saveGameData.score.ToString());
            blankEntry = new MenuEntry ( "" );
            continueEntry = new MenuEntry("Continue");
            mainMenuEntry = new MenuEntry ( "Back to Main Menu" );

            // Hook up menu event handlers.
            mainMenuEntry.Selected += mainMenuEntrySelected;
            continueEntry.Selected += continueEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(currentLevelEntry);
            //MenuEntries.Add(timeCompleteEntry);
            MenuEntries.Add(remaingingHealthEntry);
            MenuEntries.Add(levelScoreEntry);
            MenuEntries.Add ( highScoreEntry );
            MenuEntries.Add ( blankEntry );
            MenuEntries.Add ( continueEntry );
            
            /*
            // check to see if there are still levels left before adding nextLevelEntry
            if (levelData.currentlevelNum < GameConstants.LEVELS)
            {
            }
            // load credits
            else if (levelData.currentlevelNum == GameConstants.LEVELS)
            {
                // load game complete screen with credits if all levels are finished
                //LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                //               new GameCompleteScreen());
            }
            */


            MenuEntries.Add(mainMenuEntry);

            //Select the first selectable entry
            while (menuEntries[selectedEntry].HasNoHandle) {
                selectedEntry++;
            }



        }

        #endregion

        #region Handle Input
        /*
        public override void HandleInput ( GameTime gameTime, InputState input )
        {
            // For input tests we pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            PlayerIndex playerIndex;

            // Jacob: Move to the previous menu entry that can be selected
            if (menuUp.Evaluate ( input, ControllingPlayer, out playerIndex ))
            {
                do
                {
                    selectedEntry--;
                    if (selectedEntry < 0)
                        selectedEntry = menuEntries.Count - 1;
                }
                while (menuEntries[selectedEntry].HasNoHandle);
            }

            // Jacob: Move to the next menu entry that can be selected
            if (menuDown.Evaluate ( input, ControllingPlayer, out playerIndex ))
            {
                do
                {   
                    selectedEntry++;
                    if (selectedEntry >= menuEntries.Count)
                        selectedEntry = 0;
                }
                while (menuEntries[selectedEntry].HasNoHandle);
            }

            if (menuSelect.Evaluate ( input, ControllingPlayer, out playerIndex ))
            {
                OnSelectEntry ( selectedEntry, playerIndex );
            }
            else if (menuCancel.Evaluate ( input, ControllingPlayer, out playerIndex ))
            {
                OnCancel ( playerIndex );
            }
        }
        */
        /// <summary>
        /// As the main menu screen was removed from the stack due to LoadingScreen,
        /// Make this the action of Cancel, re-instating the menu this way.
        /// </summary>
        /// <param name="playerIndex"></param>
        protected override void OnCancel ( PlayerIndex playerIndex )
        {
            LoadingScreen.Load ( ScreenManager, false, null, new BackgroundScreen (),
                                                               new MainMenuScreen () );
        }

        void mainMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                               new MainMenuScreen());
        }

        void continueEntrySelected(object sender, PlayerIndexEventArgs e)
        {

            // check to see if there are still levels left before going to the next level screen
            if (levelData.currentlevelNum < GameConstants.LEVELS)
            {
                ScreenManager.AddScreen ( new NextLevelScreen ( this.levelData ), e.PlayerIndex );
            }
            // load credits if there would be no next level
            else if (levelData.currentlevelNum == GameConstants.LEVELS)
            {
                // load game complete screen with credits if all levels are finished
                //LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                //               new GameCompleteScreen());
                List<String> StoryText = new List<String> ();
                List<String> TitleText = new List<String> ();
                StoryText.Add ( "Jarred Jardine\nSteven Chen\nLee Ji\nJacob Lim\nMatthew Moldowan" );
                TitleText.Add ( "Credits" );
                ScreenManager.AddScreen ( new StoryScreen ( StoryText, TitleText, 0.9f, gameData, new BackgroundScreen (), new MainMenuScreen () ), e.PlayerIndex );
            }
        }
        

        #endregion
    }
}
