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
        MenuEntry soundMenuEntry;
        MenuEntry musicMenuEntry;


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public LevelCompleteScreen()
            : base("LevelComplete")
        {
            
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            
        }


        #endregion

        #region Handle Input


        

        #endregion
    }
}
