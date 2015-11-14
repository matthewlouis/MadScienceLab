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
            UpdateBoundingBox ( GameplayScreen._models["BasicBlock"], Matrix.CreateTranslation ( base.Position ), false, false ); //use bounding box of a BasicBlock
        }
    }
}
