using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    /**
     * This is the basic block object that will make up floors, walls etc of our level.
     */
    class BasicBlock:CellObject
    {
        private Model model;

        public BasicBlock(int column, int row):base(column, row)
        {
            model = GameplayScreen._models["BasicBlock"];
            base.isCollidable = true;

            // Provides a hitbox for the block - Steven
            UpdateBoundingBox(model, Matrix.CreateTranslation(base.Position), false, false);
        }

        public override void Draw(RenderContext renderContext)
        {
            Vector3 screenPos = renderContext.GraphicsDevice.Viewport.Project(WorldPosition, renderContext.Camera.Projection, renderContext.Camera.View, WorldMatrix);
            Vector2 screenPos2D = new Vector2(screenPos.X, screenPos.Y);


            if (screenPos2D.X >= -GameConstants.SINGLE_CELL_SIZE * 2 &&
                screenPos2D.X <= renderContext.GraphicsDevice.Viewport.Width + GameConstants.SINGLE_CELL_SIZE * 2 &&
                screenPos2D.Y >= -GameConstants.SINGLE_CELL_SIZE * 2 &&
                screenPos2D.Y <= renderContext.GraphicsDevice.Viewport.Width * 2)
            {
                    foreach (BasicEffect effect in model.Meshes[0].Effects)
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
                    model.Meshes[0].Draw();
                }
        }
    }
}
