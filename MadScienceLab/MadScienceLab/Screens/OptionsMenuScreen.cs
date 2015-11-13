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
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        // define menu entries and properties
        MenuEntry soundMenuEntry;
        MenuEntry musicMenuEntry;

        static bool sound = true;
        static bool music = false;


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            soundMenuEntry = new MenuEntry(string.Empty);
            musicMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Menu/Back");

            // Hook up menu event handlers.
            soundMenuEntry.Selected += soundMenuEntrySelected;
            musicMenuEntry.Selected += musicMenuEntrySelected;
            back.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(soundMenuEntry);
            MenuEntries.Add(musicMenuEntry);
            MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            soundMenuEntry.Text = "Sound: " + (sound ? "on" : "off");
            musicMenuEntry.Text = "Music: " + (music ? "on" : "off");
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the sound menu entry is selected.
        /// </summary>
        void soundMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            sound = !sound;

            if (!sound)
                SoundEffect.MasterVolume = 0;
            else
                SoundEffect.MasterVolume = 1;

            SetMenuEntryText();
        }

        /// <summary>
        /// Event handler for when the music menu entry is selected.
        /// </summary>
        void musicMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            music = !music;

            SetMenuEntryText();
        }




        #endregion
    }
}
