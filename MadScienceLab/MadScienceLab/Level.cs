﻿using System;
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
        public List<GameObject3D> enemyList;
        public bool LevelOver { get; set; }
        public bool GameOver { get; set; }

        public List<BackgroundBlock>[] Background { get; set; }
        private VertexBuffer[] backgroundBuffer;
        private const int NUMBER_OF_BACKGROUND_TEXTURES = 18;

        //For drawing foreground using instance render
        public List<BasicBlock> ForegroundBlocks { get { return foregroundBlockInstances; } set { foregroundBlockInstances = value; } }
        List<BasicBlock> foregroundBlockInstances;
        
        public Dictionary<string,MessageEvent> Messages { get; set; } 

        Matrix[] instanceTransforms;
        Model instancedModel;
        Matrix[] instancedModelBones;
        DynamicVertexBuffer instanceVertexBuffer;
        // Set renderstates for drawing 3D models.
        RasterizerState drawState = new RasterizerState();


        // To store instance transform matrices in a vertex buffer, we use this custom
        // vertex type which encodes 4x4 matrices as a set of four Vector4 values.
        static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );

        Type[] types;

        public Level()
        {
            //init background texture collection
            Background = new List<BackgroundBlock>[NUMBER_OF_BACKGROUND_TEXTURES];
            for (int i = 0; i < Background.Length; i++)
            {
                Background[i] = new List<BackgroundBlock>();
            }

            //init background buffer
            backgroundBuffer = new VertexBuffer[NUMBER_OF_BACKGROUND_TEXTURES];

            ForegroundBlocks = new List<BasicBlock>();
            Messages = new Dictionary<string, MessageEvent>();

            enemyList = new List<GameObject3D>();

            drawState.CullMode = CullMode.None;
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

            //For foreground instance
            instancedModel = contentManager.Load<Model>("BasicBlock");
            instancedModelBones = new Matrix[instancedModel.Bones.Count];
            instancedModel.CopyAbsoluteBoneTransformsTo(instancedModelBones);

            base.LoadContent(contentManager);
        }

        //Overridden to include buffered draw for background
        public override void Draw(RenderContext renderContext)
        {
            drawBackground(renderContext);
            drawForeground(renderContext);

            //Draw the children
            base.Draw(renderContext);

            DrawMessage(renderContext);


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
            for (int texturePatternIndex = 0; texturePatternIndex < Background.Length; texturePatternIndex++)
            {
                List<BackgroundBlock> patternedBlocks = Background[texturePatternIndex];
                List<VertexPositionTexture> vertexList = new List<VertexPositionTexture>();

                foreach (BackgroundBlock backgroundBlock in patternedBlocks)
                {
                    foreach (VertexPositionTexture vertex in
                        backgroundBlock.GetBillboardVertices())
                    {
                        vertexList.Add(vertex);
                    }
                }
                //if there are background blocks of this pattern
                if (vertexList.Count != 0)
                {
                    backgroundBuffer[texturePatternIndex] = new VertexBuffer(renderContext.GraphicsDevice, VertexPositionTexture.VertexDeclaration, vertexList.Count, BufferUsage.WriteOnly);
                    backgroundBuffer[texturePatternIndex].SetData<VertexPositionTexture>(vertexList.ToArray());
                }
            }
        }

        private void DrawMessage(RenderContext renderContext)
        {
            foreach(MessageEvent msg in Messages.Values)
            {
                if(msg.typingState == GameConstants.TYPING_STATE.DoneTyping || msg.typingState == GameConstants.TYPING_STATE.Typing)
                {
                    msg.DisplayMessage(renderContext);
                }
            }

        }

        //Draws Background seperate: was able to get huge performance increase doing this way
        private void drawBackground(RenderContext renderContext)
        {
            //TODO: allow multiple backgrounds
            //Draw background 
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
            for (int patternIndex = 0; patternIndex < backgroundBuffer.Length; patternIndex++)
            {
                    //if there are no patterned blocks of this type
                    if (backgroundBuffer[patternIndex] == null)
                        continue;

                    switch (patternIndex)
                    {
                        case 0:
                            renderContext.BasicEffect.Texture = renderContext.Textures["Tile_Gray"];
                            break;
                        case 1:
                            renderContext.BasicEffect.Texture = renderContext.Textures["Tile_White"];
                            break;
                        case 2:
                            renderContext.BasicEffect.Texture = renderContext.Textures["Vent"];
                            break;
                        case 3:
                            renderContext.BasicEffect.Texture = renderContext.Textures["Pipes1"];
                            break;
                        case 4:
                            renderContext.BasicEffect.Texture = renderContext.Textures["Pipes2"];
                            break;
                        case 5:
                            renderContext.BasicEffect.Texture = renderContext.Textures["Tile_Black"];
                            break;
                        case 6:
                            renderContext.BasicEffect.Texture = renderContext.Textures["Tile_Gray_Stain"];
                            break;
                        case 7:
                            renderContext.BasicEffect.Texture = renderContext.Textures["Tile_Gray_Stain2"];
                            break;
                        case 8:
                            renderContext.BasicEffect.Texture = renderContext.Textures["Tile_Gray_Stain3"];
                            break;
                        case 9:
                            renderContext.BasicEffect.Texture = renderContext.Textures["Tile_Gray_Stain1"];
                            break;
                        case 10:
                            renderContext.BasicEffect.Texture = renderContext.Textures["Emcsquare"];
                            break;
                        case 11:
                            renderContext.BasicEffect.Texture = renderContext.Textures["Einstien"];
                            break;
                        case 12:
                            renderContext.BasicEffect.Texture = renderContext.Textures["atom1"];
                            break;
                        case 13:
                            renderContext.BasicEffect.Texture = renderContext.Textures["atom2"];
                            break;
                        case 14:
                            renderContext.BasicEffect.Texture = renderContext.Textures["chalkboard1"];
                            break;
                        case 15:
                            renderContext.BasicEffect.Texture = renderContext.Textures["chalkboard2"];
                            break;
                        case 16:
                            renderContext.BasicEffect.Texture = renderContext.Textures["chalkboard3"];
                            break;
                        case 17:
                            renderContext.BasicEffect.Texture = renderContext.Textures["chalkboard4"];
                            break;
                }
                if (backgroundBuffer[patternIndex] != null)
                {
                    foreach (EffectPass pass in renderContext.BasicEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        renderContext.GraphicsDevice.SetVertexBuffer(backgroundBuffer[patternIndex]);
                        renderContext.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, backgroundBuffer[patternIndex].VertexCount / 3);
                    }
                }
            }

            //Set back to normal
            renderContext.GraphicsDevice.RasterizerState = oldState;
        }

        private void drawForeground(RenderContext renderContext)
        {
            RasterizerState oldState = renderContext.GraphicsDevice.RasterizerState;
            renderContext.GraphicsDevice.RasterizerState = drawState;
            //renderContext.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            // Gather instance transform matrices into a single array.
            Array.Resize(ref instanceTransforms, foregroundBlockInstances.Count);

            for (int i = 0; i < foregroundBlockInstances.Count; i++)
            {
                instanceTransforms[i] = foregroundBlockInstances[i].GetWorldMatrix();
            }

            //The actual drawing technique:
            if (instanceTransforms.Length == 0)
                return;

            // If we have more instances than room in our vertex buffer, grow it to the neccessary size.
            if ((instanceVertexBuffer == null) ||
                (instanceTransforms.Length > instanceVertexBuffer.VertexCount))
            {
                if (instanceVertexBuffer != null)
                    instanceVertexBuffer.Dispose();

                instanceVertexBuffer = new DynamicVertexBuffer(renderContext.GraphicsDevice, instanceVertexDeclaration,
                                                               instanceTransforms.Length, BufferUsage.WriteOnly);
            }

            // Transfer the latest instance transform matrices into the instanceVertexBuffer.
            instanceVertexBuffer.SetData(instanceTransforms, 0, instanceTransforms.Length, SetDataOptions.Discard);

            foreach (ModelMesh mesh in instancedModel.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                    renderContext.GraphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                        new VertexBufferBinding(instanceVertexBuffer, 0, 1)
                    );

                    renderContext.GraphicsDevice.Indices = meshPart.IndexBuffer;

                    // Set up the instance rendering effect.
                    Effect effect = meshPart.Effect;

                    effect.CurrentTechnique = effect.Techniques["HardwareInstancing"];

                    effect.Parameters["World"].SetValue(instancedModelBones[mesh.ParentBone.Index]);
                    effect.Parameters["View"].SetValue(renderContext.Camera.View);
                    effect.Parameters["Projection"].SetValue(renderContext.Camera.Projection);

                    // Draw all the instance copies in a single call.
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        renderContext.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               meshPart.NumVertices, meshPart.StartIndex,
                                                               meshPart.PrimitiveCount, instanceTransforms.Length);
                    }
                }
                renderContext.GraphicsDevice.RasterizerState = oldState; //set back to original
            }
        }
    }
}
