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
    class NextLevelScreen : MenuScreen
    {
        #region Fields

        // define menu entries and properties
        MenuEntry levelNumEntry;
        MenuEntry currentLevelEntry;
        MenuEntry blankEntry;
        MenuEntry mainMenuEntry;
        MenuEntry scoreScreenEntry;
        MenuEntry nextLevelEntry;

        GameData.LevelData levelData;
        GameData gameData;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public NextLevelScreen(GameData.LevelData levelData )
            : base("Next Level")
        {
            this.levelData = levelData;
            // Create our menu entries.
            
            levelNumEntry = new MenuEntry("Level "+(levelData.currentlevelNum+1)+":");
            currentLevelEntry = new MenuEntry("\""+GameConstants.LEVELNAMES[levelData.currentlevelNum+1]+"\"");
            //timeCompleteEntry = new MenuEntry(time);
            blankEntry = new MenuEntry ( "" );
            nextLevelEntry = new MenuEntry ( "Start" );
            scoreScreenEntry = new MenuEntry ( "Score Screen" );
            mainMenuEntry = new MenuEntry ( "Main Menu" );

            // Hook up menu event handlers.
            mainMenuEntry.Selected += mainMenuEntrySelected;
            scoreScreenEntry.Selected += scoreScreenEntrySelected;
            nextLevelEntry.Selected += nextLevelEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add ( levelNumEntry );
            MenuEntries.Add(currentLevelEntry);
            MenuEntries.Add ( blankEntry );
            MenuEntries.Add ( blankEntry );
            
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


            MenuEntries.Add ( scoreScreenEntry );
            MenuEntries.Add(mainMenuEntry);

            //Select the first selectable entry
            while (menuEntries[selectedEntry].HasNoHandle) {
                selectedEntry++;
            }



        }

        #endregion

        #region Handle Input

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
        /// <summary>
        /// Cancel exits the Next Level screen.
        /// </summary>
        /// <param name="playerIndex"></param>
        protected override void OnCancel ( PlayerIndex playerIndex )
        {
            ExitScreen ();
        }

        void mainMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                               new MainMenuScreen());
        }
        void scoreScreenEntrySelected ( object sender, PlayerIndexEventArgs e )
        {
            ExitScreen ();
        }

        void nextLevelEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                           new GameplayScreen(++levelData.currentlevelNum));            
        }
        

        #endregion
    }
}
