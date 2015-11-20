using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;


namespace MadScienceLab
{
    /**
     * Used as a a helper to create a 'Scene Graph'. Allows objects in a scene graph to have access
     * to others in the same Scene Graph.
     */
    public class RenderContext
    {
        public SpriteBatch    SpriteBatch { get; set; }
        public SpriteFont     SpriteFont { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }
        public GameTime       GameTime { get; set; }
        public GameTimer      GameTimer { get; set; }
        public BaseCamera     Camera { get; set; }
        public Character      Player { get; set; }
        public Enemy          Doomba { get; set; }
        public Level          Level { get; set; }
        public MessageEvent   CurrMsgEvent { get; set; }
        public BasicEffect BasicEffect { get; set; }
        public Quadtree       Quadtree { get; set; }
        public Rectangle      Boxhit { get; set; }
        public List<Rectangle> BoxesHit { get; set; }
        public List<CellObject> QuadtreeDebug { get; set; }
        public Game Game { get; set; }
        public Dictionary<String, Texture2D> Textures { get; set; }
        public Dictionary<String, SoundEffect> Songs { get; set; }
    }
}
