using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    class MessageEvent : CellObject
    {
        public string Message {get; set;}
        public MessageEvent(int column, int row) : base(column, row)
        {
            base.Model = GameplayScreen._models["ExitBlock"];
            Scale(48f, 48f, 48f);
            Position = new Vector3(Position.X, Position.Y - 2, Position.Z - 27); //not sure about this position
            HitboxHeightOffset = 2;
            isCollidable = true;
        }
    }
}
