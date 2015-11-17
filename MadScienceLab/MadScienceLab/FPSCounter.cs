using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

/**
 * 
 */
namespace MadScienceLab
{
    public class FPSCounter : DrawableGameComponent
        {
            SpriteBatch spriteBatch;
            SpriteFont spriteFont;
            RenderContext renderContext;

            int frameRate = 0;
            int frameCounter = 0;
            TimeSpan elapsedTime = TimeSpan.Zero;


            public FPSCounter(RenderContext renderContext)
                : base(renderContext.Game)
            {
                this.renderContext = renderContext;
            }


            public void LoadContent(ContentManager content)
            {
                    spriteBatch = new SpriteBatch(renderContext.GraphicsDevice);
                    spriteFont = content.Load<SpriteFont>("Font");
            }


            public override void Update(GameTime gameTime)
            {
                elapsedTime += gameTime.ElapsedGameTime;

                if (elapsedTime > TimeSpan.FromSeconds(1))
                {
                    elapsedTime -= TimeSpan.FromSeconds(1);
                    frameRate = frameCounter;
                    frameCounter = 0;
                }
            }


            public override void Draw(GameTime gameTime)
            {
                frameCounter++;

                string fps = string.Format("fps: {0}", frameRate);

                if (spriteBatch != null)
                {
                    spriteBatch.Begin();

                    spriteBatch.DrawString(spriteFont, fps, new Vector2(33, 33), Color.Black);
                    spriteBatch.DrawString(spriteFont, fps, new Vector2(32, 32), Color.White);

                    spriteBatch.End();
                }
            }
        }
}
