using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    public class BackgroundBlock:CellObject
    {
        public BackgroundBlock(int row, int column, Texture2D texture)
            : base(row, column, -GameConstants.SINGLE_CELL_SIZE) //calls overloaded cellobject constructor and places in background
        {
            //base.Model = GameplayScreen._models["BackgroundBlock"];
            //Scale(48f, 48f, 48f);
            this.Texture = texture;
        }

        public List<VertexPositionTexture> GetBillboardVertices()
        {
            List<VertexPositionTexture> vList =
               new List<VertexPositionTexture>();
            vList.Add(new VertexPositionTexture(
                new Vector3(0 + Position.X, 0 + Position.Y, Position.Z/2),Vector2.UnitY));
            vList.Add(new VertexPositionTexture(
                new Vector3(GameConstants.SINGLE_CELL_SIZE + Position.X, 0 + Position.Y, Position.Z/2), Vector2.One));
            vList.Add(new VertexPositionTexture(
                new Vector3(0 + Position.X, GameConstants.SINGLE_CELL_SIZE + Position.Y, Position.Z/2), Vector2.Zero));
            vList.Add(new VertexPositionTexture(
                new Vector3(GameConstants.SINGLE_CELL_SIZE + Position.X, 0 + Position.Y, Position.Z/2), Vector2.One));
            vList.Add(new VertexPositionTexture(
                new Vector3(GameConstants.SINGLE_CELL_SIZE + Position.X, GameConstants.SINGLE_CELL_SIZE + Position.Y, Position.Z/2), Vector2.UnitX));
            vList.Add(new VertexPositionTexture(
                new Vector3(0 + Position.X, GameConstants.SINGLE_CELL_SIZE + Position.Y, Position.Z/2), Vector2.Zero));
            return vList;
        }
    }
}
