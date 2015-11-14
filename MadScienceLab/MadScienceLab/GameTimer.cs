using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections;
using GameStateManagement;

namespace MadScienceLab
{
    public class GameTimer
    {
        private string time;
        private Vector2 position;
        RenderContext renderContext;

        TimeSpan elapsedTime = TimeSpan.Zero;

        public GameTimer(RenderContext renderContext)
        {
            this.renderContext = renderContext;
            string time = string.Format("Time: {0}", elapsedTime);
            //time = time.Remove(time.Length - 5);
            position = new Vector2(GameConstants.X_RESOLUTION - renderContext.SpriteFont.MeasureString(time).X, 50);
        }

        public void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;
        }


        public void Draw(GameTime gameTime)
        {
            time = string.Format("Time: {0}", elapsedTime);
            time = time.Remove(time.Length - 5);
            renderContext.SpriteBatch.Begin();
            renderContext.SpriteBatch.DrawString(renderContext.SpriteFont, time, position, Color.Black);
            renderContext.SpriteBatch.End();
        }
    }
}
