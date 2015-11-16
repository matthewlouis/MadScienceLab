using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MadScienceLab
{
    public class Level:GameObject3D
    {
        public Point PlayerPoint;
        public Dictionary<Type, List<GameObject3D>> gameObjects;
        public List<GameObject3D> collidableObjects;
        public bool LevelOver { get; set; }
        public bool GameOver { get; set; }

        public List<BackgroundBlock> Background { get; set; }
        private VertexBuffer backgroundBuffer;

        Type[] types;

        public Level()
        {
            Background = new List<BackgroundBlock>();
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager)
        {
            gameObjects = new Dictionary<Type, List<GameObject3D>>();
            Type[] types = { typeof(PickableBox), typeof(ToggleSwitch), typeof(Door), typeof(Button), typeof(BasicBlock), typeof(LaserTurret), typeof(MovingPlatform), typeof(Character) };
            foreach (Type type in types)
            {
                gameObjects.Add(type, new List<GameObject3D>());
            }
            this.types = types;
            base.LoadContent(contentManager);
        }

        //Overridden to include buffered draw for background
        public override void Draw(RenderContext renderContext)
        {

            drawBackground(renderContext);

            //Draw the children
            base.Draw(renderContext);
        }        

        public override void Update(RenderContext renderContext)
        {
            
            base.Update(renderContext);
            PopulateTypeList(renderContext);
        }

        /// <summary>
        /// Populates the gameObjects map of list of game objects by type.
        /// </summary>
        /// <param name="renderContext"></param>
        public void PopulateTypeList(RenderContext renderContext)
        {

            gameObjects = new Dictionary<Type, List<GameObject3D>>();
            
            foreach (Type type in types)
            {
                gameObjects.Add(type, new List<GameObject3D>());
            }
            foreach (GameObject3D Child in this.Children)
            {
                if (!gameObjects.ContainsKey(Child.GetType())) //add any object types as found in the level
                {
                    gameObjects.Add(Child.GetType(), new List<GameObject3D>());
                }
                gameObjects[Child.GetType()].Add(Child);
            }
            gameObjects[typeof(Character)].Add(renderContext.Player);

            this.gameObjects = gameObjects;
        }

        //Creates buffer for background
        public void setBackgroundBuffer(RenderContext renderContext)
        {
            List<VertexPositionTexture> vertexList = new List<VertexPositionTexture>();

            foreach (BackgroundBlock backgroundBlock in Background)
            {
                foreach (VertexPositionTexture vertex in
                    backgroundBlock.GetBillboardVertices())
                {
                    vertexList.Add(vertex);
                }
            }
            backgroundBuffer = new VertexBuffer(renderContext.GraphicsDevice, VertexPositionTexture.VertexDeclaration, vertexList.Count, BufferUsage.WriteOnly);
            backgroundBuffer.SetData<VertexPositionTexture>(vertexList.ToArray()); 
        }

        //Draws Background seperate: was able to get huge performance increase doing this way
        private void drawBackground(RenderContext renderContext)
        {
            //TODO: allow multiple backgrounds
            //Draw background 
            renderContext.BasicEffect.Texture = renderContext.Textures["Tile_Gray"];
            renderContext.BasicEffect.TextureEnabled = true;

            renderContext.BasicEffect.World = Matrix.Identity;
            renderContext.BasicEffect.View = renderContext.Camera.View;
            renderContext.BasicEffect.Projection = renderContext.Camera.Projection;

            //Store state so we can go back to it
            RasterizerState oldState = renderContext.GraphicsDevice.RasterizerState;

            //Matt: couldn't get it drawing without doing this...could use some help creating vertices and maybe get rid of this
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullClockwiseFace;
            rs.MultiSampleAntiAlias = false;
            renderContext.GraphicsDevice.RasterizerState = rs;
            
            foreach (EffectPass pass in renderContext.BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                renderContext.GraphicsDevice.SetVertexBuffer(backgroundBuffer);
                renderContext.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, backgroundBuffer.VertexCount / 3);
            }

            //Set back to normal
            renderContext.GraphicsDevice.RasterizerState = oldState;
        }
    }
}
