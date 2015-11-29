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
        public BackgroundBlock(int row, int column)
            : base(row, column, -GameConstants.SINGLE_CELL_SIZE) //calls overloaded cellobject constructor and places in background
        {
            //base.Model = GameplayScreen._models["BackgroundBlock"];
            //Scale(48f, 48f, 48f);
            //this.Texture = texture;
        }

        public List<VertexPositionTexture> GetBillboardVertices()
        {
            int halfcell = GameConstants.SINGLE_CELL_SIZE / 2;
            List<VertexPositionTexture> vList =
               new List<VertexPositionTexture>();
            vList.Add(new VertexPositionTexture(
                new Vector3(Position.X-halfcell, Position.Y - halfcell, Position.Z/2),Vector2.UnitY));
            vList.Add(new VertexPositionTexture(
                new Vector3(halfcell + Position.X, Position.Y - halfcell, Position.Z/2), Vector2.One));
            vList.Add(new VertexPositionTexture(
                new Vector3(0 + Position.X - halfcell, halfcell + Position.Y, Position.Z/2), Vector2.Zero));
            vList.Add(new VertexPositionTexture(
                new Vector3(halfcell + Position.X, Position.Y - halfcell, Position.Z/2), Vector2.One));
            vList.Add(new VertexPositionTexture(
                new Vector3(halfcell + Position.X, halfcell + Position.Y, Position.Z/2), Vector2.UnitX));
            vList.Add(new VertexPositionTexture(
                new Vector3(Position.X - halfcell, halfcell + Position.Y, Position.Z/2), Vector2.Zero));
            return vList;
        }
    }
}
