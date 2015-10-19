using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace MadScienceLab
{
    /**
     * Used as a a helper to create a 'Scene Graph'. Allows objects in a scene graph to have access
     * to others in the same Scene Graph.
     */
    public class RenderContext
    {
        public SpriteBatch    SpriteBatch { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }
        public GameTime       GameTime { get; set; }
        public BaseCamera     Camera { get; set; }
        public Character      Player { get; set; }
        public Level          Level { get; set; }
    }
}
