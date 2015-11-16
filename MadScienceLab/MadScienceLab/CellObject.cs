using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace MadScienceLab
{
    /**
     * CellObject class.
     * 
     */
    public class CellObject : GameObject3D
    {
        // Hit box information - Steven
        public bool isCollidable { get; protected set; }
        public bool IsPassable { get; protected set; }
        
        public Model Model { get; protected set; }
        public Texture2D Texture { get; protected set; }

        public Point CellNumber { get; private set; } //on our grid, the cell number that this object occupies.
        protected int zPosition = 0; //0 is where actual elements in play get put...background objects will be further back.

        private float x, y;

  

        /**
         * Constructor. Takes the column and row where the block will placed on a grid.
         */ 
        public CellObject(int column, int row)
        {
            //Calculate offset where object will be placed.
            x = (GameConstants.SINGLE_CELL_SIZE * column) - (GameConstants.X_RESOLUTION / 2.0f); // divde by 2 because 4 quadrants
            y = (GameConstants.SINGLE_CELL_SIZE * row) - (GameConstants.Y_RESOLUTION / 2.0f);
            CellNumber = new Point(column, row);

            //Place object on grid.
            Translate(x, y, zPosition);
            TransVelocity = new Vector3 ( 0, 0, 0 );
            TransAccel = new Vector3 ( 0, 0, 0 );

            //Can place code to translate the hit box here.
        }

        //Overloaded Constructor for placing background objects.
        public CellObject(int column, int row, int zPosition):this(column, row)
        {
            Translate(x, y, zPosition);
        }

        //Overloaded Constructor for placing at exact coordinates
        public CellObject(Vector2 coordinates)
        {
            //Place object on grid.
            Translate(coordinates.X,coordinates.Y, zPosition);
            TransVelocity = new Vector3(0, 0, 0);
            TransAccel = new Vector3(0, 0, 0);
        }

        //Will need to override update to move hitbox as well.
        public override void Update(RenderContext renderContext)
        {
            base.Update(renderContext);
        }
        public virtual void Reset() { }

        public override void Draw(RenderContext renderContext)
        {
            Vector3 screenPos = renderContext.GraphicsDevice.Viewport.Project(WorldPosition, renderContext.Camera.Projection, renderContext.Camera.View, WorldMatrix);
            Vector2 screenPos2D = new Vector2(screenPos.X, screenPos.Y);


            if (screenPos2D.X >= -GameConstants.SINGLE_CELL_SIZE*2 && 
                screenPos2D.X <= renderContext.GraphicsDevice.Viewport.Width + GameConstants.SINGLE_CELL_SIZE*2 &&
                screenPos2D.Y >= -GameConstants.SINGLE_CELL_SIZE * 2 &&
                screenPos2D.Y <= renderContext.GraphicsDevice.Viewport.Width * 2)
            {
                foreach (ModelMesh mesh in Model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();

                        if (Texture != null)
                        {
                            renderContext.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
                            effect.Texture = Texture;
                            effect.TextureEnabled = true;

                        }

                        effect.View = renderContext.Camera.View;
                        effect.Projection = renderContext.Camera.Projection;
                        effect.World = WorldMatrix;
                    }
                    mesh.Draw();
                }
            }
        }

        public void UpdateBoundingBox(Model model, Matrix worldTransform, bool isRotated, bool isOffset)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransform);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            // Create and return bounding box
            Vector3 size = max - min;
            if (isRotated)
            {
                HitboxWidth = (int)size.Z;
                HitboxWidthOffset = GameConstants.SINGLE_CELL_SIZE / 2 - (int)size.Z / 2;
            }
            else
            {
                HitboxWidth = (int)size.X;
                HitboxWidthOffset = 0;
            }

            if (isOffset)
            {
                HitboxWidthOffset = GameConstants.SINGLE_CELL_SIZE / 2 - (int)size.Z / 2;
            }
            else
            {
                HitboxWidthOffset = 0;
            }
            HitboxHeight = (int)size.Y;
        }
    }
}
