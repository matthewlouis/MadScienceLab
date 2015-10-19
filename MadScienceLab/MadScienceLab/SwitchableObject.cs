using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MadScienceLab
{
    abstract class SwitchableObject : CellObject
    {
        public SwitchableObject(int row, int column) : base(row, column) { }
        public abstract void Toggle(RenderContext renderContext);
    }
}
