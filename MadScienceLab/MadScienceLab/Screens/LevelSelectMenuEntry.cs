#region File Description
//-----------------------------------------------------------------------------
// MenuEntry.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;
#endregion

namespace MadScienceLab
{
    /// <summary>
    /// Subclass of MenuEntry used in order to set specific font sizes for the respective menu.
    /// </summary>
    class LevelSelectMenuEntry : MenuEntry
    {

        /// <summary>
        /// 
        /// </summary>
        float scalept2 = 0.9f; //Jacob: Scale size of each menu item in level select

        #region Events

        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public LevelSelectMenuEntry(string text) : base(text)
        {
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public override void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false
#if WINDOWS_PHONE
            isSelected = false;
#endif

            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? Color.Yellow : Color.White;

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;

            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = (1 + pulsate * 0.05f * selectionFade) * scalept2;

            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Vector2 origin = new Vector2 ( 0, font.LineSpacing * scalept2 / 2 );

            spriteBatch.DrawString(font, text, position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight ( MenuScreen screen )
        {
            return (int)(screen.ScreenManager.Font.LineSpacing*scalept2);
        }


        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public virtual int GetWidth ( MenuScreen screen )
        {
            return (int)(screen.ScreenManager.Font.MeasureString ( Text ).X * scalept2);
        }
        #endregion
    }
}