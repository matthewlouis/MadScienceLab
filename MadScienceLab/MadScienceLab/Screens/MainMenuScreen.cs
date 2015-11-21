#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
#endregion

namespace MadScienceLab
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization

        MenuEntry playGameMenuEntry;
        MenuEntry LevelSelectMenuEntry;
        MenuEntry optionsMenuEntry;
        MenuEntry exitMenuEntry;

        GameData saveGameData;
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {

            // get save game
            saveGameData = new GameData();

            // Create our menu entries.
           
            if (saveGameData.saveGameData.currentlevel < GameConstants.LEVELS &&
                saveGameData.saveGameData.currentlevel > 1)
                playGameMenuEntry = new MenuEntry("Continue Game");
            else if(saveGameData.saveGameData.currentlevel == 1)
                playGameMenuEntry = new MenuEntry("Play Game");
            MenuEntry LevelSelectMenuEntry = new MenuEntry("Level Select");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            
            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            LevelSelectMenuEntry.Selected += LevelSelectMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(LevelSelectMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }


        #endregion

        #region Handle Input

        void SetMenuEntryText()
        {
            
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (saveGameData.saveGameData.currentlevel < GameConstants.LEVELS &&
                saveGameData.saveGameData.currentlevel > 1)
            {
                LoadingScreen.Load ( ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen ( saveGameData.saveGameData.currentlevel ) ); // loads next level from currentLevel
            }
            else
            {
                List<String> storytext = new List<String>();
                storytext.Add("It's a dark and stormy Friday night. Or maybe it's Saturday- when you're as mad a scientist as Dr.Frankenbuns, it's easy to lose track. Deep in the depths of his 100-floor evil laboratory, Dr.Frankenbuns is hard at work, scheming new ways to take over the world.");
                storytext.Add("Suddenly, lightning strikes the Securitron-Combobulator on the roof of the lab (it was probably a dumb place to put it.)");
                storytext.Add("The security systems have gone rogue- the very devices created to keep pesky intruders out are now keeping Dr.Frankenbuns inside. Even Doomba, his typically helpful robot vacuum cleaner, has acquired a thirst for destruction.");
                storytext.Add("One thing's for sure- it's going to be a long night.");
                ScreenManager.AddScreen ( new StoryScreen ( storytext, saveGameData ), e.PlayerIndex );
            }
        }

        /// <summary>
        /// Event handler for when the Level Select menu entry is selected.
        /// </summary>
        void LevelSelectMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new LevelSelectScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit this sample?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
