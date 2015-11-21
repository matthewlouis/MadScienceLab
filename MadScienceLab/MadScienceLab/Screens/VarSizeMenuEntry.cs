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
using System.Collections.Generic;
#endregion

namespace MadScienceLab
{
    /// <summary>
    /// Subclass of MenuEntry used in order to set specific font sizes for the respective menu.
    /// </summary>
    class VarSizeMenuEntry : MenuEntry
    {

        /// <summary>
        /// Font size used for text. This is the particular modification of the MenuEntry.
        /// </summary>
        float scalept2 = 0.9f; //Jacob: Scale size of each menu item in level select

        //Set max width of the text
        int HorizWidth = 300;
        float TextWidth = 0;

        #region Events

        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public VarSizeMenuEntry(string text, float scalept2) : base(text)
        {
            this.scalept2 = scalept2;
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public override void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            TextWidth = screen.ScreenManager.Font.MeasureString ( Text ).X * scalept2;
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
            Vector2 LineHeight = new Vector2 ( 0, font.LineSpacing * scalept2 );
            List<String> lines = GetLines ( screen, HorizWidth );

            //Iterate to draw each line, centered relative to the block of text.
            for (int i = 0; i < lines.Count; i++)
            {
                Vector2 LinePos = position + LineHeight * i + new Vector2 ( (TextWidth - LineWidth ( screen, lines[i] )) / 2, 0 );
                spriteBatch.DrawString ( font, lines[i], LinePos, color, 0,
                                       origin, scale, SpriteEffects.None, 0 );
            }
        }


        /// <summary>
        /// Jacob: This breaks the text into separate lines, depending on the specified max width for the text string
        /// And that is returned.
        /// </summary>
        /// <returns></returns>
        public virtual List<String> GetLines (MenuScreen screen, int width)
        {
            TextWidth = 0;
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;
            string trimmableText = text;
            List<String> lines = new List<String>();
            while (trimmableText.Length >= 1) {
                int c = 1;
                while (c < trimmableText.Length &&
                    font.MeasureString ( trimmableText.Substring ( 0, ((trimmableText.IndexOf(" ", c+1)!=-1)?trimmableText.IndexOf(" ", c+1):trimmableText.Length) ) ).X*scalept2 < width) //so ends at trimmableText.Length or when the width when adding another char would exceed the width
                {
                    c = ((trimmableText.IndexOf ( " ", c + 1 ) != -1) ? trimmableText.IndexOf ( " ", c + 1 ) : trimmableText.Length); //Set to the next space or the end of the string if that doesn't exist.
                }
                String linestring = trimmableText.Substring ( 0, c ).TrimStart(' ');
                lines.Add(linestring);
                trimmableText = trimmableText.Substring ( c );
                if (LineWidth(screen, linestring) > TextWidth)
                    TextWidth = font.MeasureString ( linestring ).X * scalept2;
            }

            return lines;
        }

        protected virtual float LineWidth ( MenuScreen screen, String line )
        {
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;
            return font.MeasureString ( line ).X * scalept2;
        }

        /// <summary>
        /// Queries how much space this menu entry requires.
        /// It can be greater with more lines.
        /// </summary>
        public override int GetHeight ( MenuScreen screen )
        {
            int numLines = GetLines ( screen, HorizWidth ).Count;
            return (int)(numLines*screen.ScreenManager.Font.LineSpacing*scalept2);
        }


        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public override int GetWidth ( MenuScreen screen )
        {
            int numLines = GetLines ( screen, HorizWidth ).Count;
            return (int)(TextWidth);
        }
        #endregion
    }
}