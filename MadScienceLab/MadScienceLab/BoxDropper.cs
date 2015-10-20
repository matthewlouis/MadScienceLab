using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MadScienceLab
{
    class BoxDropper:SwitchableObject
    {
        public int NumberOfBoxes { get; private set; }
        private int row, column;

        public BoxDropper(int column, int row, int numberOfBoxes)
            : base(column, row)
        {
            this.row = row;
            this.column = column;
            base.Model = Game1._models["BlockDropper"];
            NumberOfBoxes = numberOfBoxes;
        }

        //Drops a box
        public override void Toggle(RenderContext renderContext)
        {
            if (NumberOfBoxes > 0)
            {
                //Creates new PickableBox underneath dropper.
                PickableBox newBox = new PickableBox(new Vector2(this.Position.X, this.Position.Y-GameConstants.SINGLE_CELL_SIZE));
                renderContext.Level.AddChild(newBox);
                NumberOfBoxes--;
            }
        }
    }
}
