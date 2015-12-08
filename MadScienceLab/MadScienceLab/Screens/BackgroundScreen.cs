#region File Description
//-----------------------------------------------------------------------------
// BackgroundScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections;
using GameStateManagement;
using MadScienceLab;
#endregion

namespace MadScienceLab
{
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    class BackgroundScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        Texture2D backgroundTexture;
        Texture2D menuBackground;
        Texture2D madLabText;
        Rectangle madLabTextSize;
        Rectangle menuSize;
        Texture2D gear1;
        Texture2D gear2;
        Texture2D gear3;
        Texture2D gear4;
        Texture2D gear5;
        Texture2D gear6;
        Texture2D gear7;
        Texture2D gear8;
        float rotationAngle = 0f;
        Vector2 origin;
        Vector2 gear1pos;
        Vector2 gear2pos;
        Vector2 gear3pos;
        Vector2 gear4pos;
        Vector2 gear5pos;
        Vector2 gear6pos;
        Vector2 gear7pos;
        Vector2 gear8pos;
        float elaspedTime;
        private SoundEffect menuSong;
        Effect ripple;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                backgroundTexture = content.Load<Texture2D>("Textures/UI/Background");
                menuBackground = content.Load<Texture2D>("Textures/UI/MenuItemBackground");
                madLabText = content.Load<Texture2D>("Textures/UI/MadLabText");
                float scale = 1.8f;
                Vector2 menuBounds = new Vector2((int)(menuBackground.Width * scale), (int)(menuBackground.Height * scale));
                menuSize = new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width / 2 - (int)menuBounds.X / 2, ScreenManager.GraphicsDevice.Viewport.Height / 2 - (int)menuBounds.Y / 2 + 20, (int)menuBounds.X, (int)menuBounds.Y);
                scale = 1.5f;
                Vector2 madLabTextBounds = new Vector2((int)(madLabText.Width * scale), (int)(madLabText.Height * scale));
                madLabTextSize = new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width / 2 - (int)madLabTextBounds.X / 2, 40, (int)madLabTextBounds.X, (int)madLabTextBounds.Y);
                gear1 = content.Load<Texture2D>("Textures/UI/bg_0000_Layer-1");
                gear2 = content.Load<Texture2D>("Textures/UI/bg_0002_Layer-3");
                gear3 = content.Load<Texture2D>("Textures/UI/bg_0001_Layer-2");
                gear4 = content.Load<Texture2D>("Textures/UI/bg_0003_Layer-4");
                gear5 = content.Load<Texture2D>("Textures/UI/bg_0004_Layer-5");
                gear6 = content.Load<Texture2D>("Textures/UI/bg_0005_Layer-6");
                gear7 = content.Load<Texture2D>("Textures/UI/bg_0006_Layer-7");
                gear8 = content.Load<Texture2D>("Textures/UI/bg_0007_Layer-8");
                gear1pos = new Vector2(160, 330);
                gear2pos = new Vector2(gear1pos.X + 180, gear1pos.Y - 85);
                gear3pos = new Vector2(gear2pos.X + 5, gear2pos.Y + 195);
                gear4pos = new Vector2(gear3pos.X + 195, gear3pos.Y - 35);
                gear5pos = new Vector2(gear4pos.X + 145, gear4pos.Y + 133);
                gear6pos = new Vector2(gear5pos.X + 120, gear5pos.Y - 160);
                gear7pos = new Vector2(gear6pos.X + 140, gear6pos.Y - 140);
                gear8pos = new Vector2(gear7pos.X + 155, gear7pos.Y + 125);
                menuSong = content.Load<SoundEffect>("Songs/MusicMenuLoop");
                origin = new Vector2(gear1.Bounds.Center.X, gear1.Bounds.Center.Y);
                ripple = content.Load<Effect>("Shaders/Ripple");
                MusicPlayer.Stop();
                MusicPlayer.SetVolume(0.4f);
                MusicPlayer.PlaySong(menuSong);
            }
        }



        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void Unload()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            ripple.Parameters["fT"].SetValue((float)gameTime.TotalGameTime.TotalSeconds / 2);
            elaspedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elaspedTime > 1000)
            {
                rotationAngle += (float)gameTime.ElapsedGameTime.TotalSeconds * 1;
                rotationAngle = rotationAngle % (MathHelper.Pi * 2);
                if (elaspedTime > 2000)
                {
                    elaspedTime = 0;
                }
            }
            base.Update(gameTime, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            spriteBatch.Draw(gear1, gear1pos, null, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha), rotationAngle, origin, 1.2f, SpriteEffects.None, 0);
            spriteBatch.Draw(gear2, gear2pos, null, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha), -rotationAngle, origin, 1.2f, SpriteEffects.None, 0);
            spriteBatch.Draw(gear3, gear3pos, null, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha), rotationAngle, origin, 1.2f, SpriteEffects.None, 0);
            spriteBatch.Draw(gear4, gear4pos, null, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha), -rotationAngle, origin, 1.2f, SpriteEffects.None, 0);
            spriteBatch.Draw(gear5, gear5pos, null, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha), rotationAngle, origin, 1.2f, SpriteEffects.None, 0);
            spriteBatch.Draw(gear6, gear6pos, null, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha), -rotationAngle, origin, 1.2f, SpriteEffects.None, 0);
            spriteBatch.Draw(gear7, gear7pos, null, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha), rotationAngle, origin, 1.2f, SpriteEffects.None, 0);
            spriteBatch.Draw(gear8, gear8pos, null, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha), -rotationAngle, origin, 1.2f, SpriteEffects.None, 0);
            spriteBatch.Draw(menuBackground, menuSize, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            spriteBatch.End();

            spriteBatch.Begin(0, BlendState.AlphaBlend, null, null, null, ripple);
            spriteBatch.Draw(madLabText, madLabTextSize,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            spriteBatch.End();
        }


        #endregion
    }
}
