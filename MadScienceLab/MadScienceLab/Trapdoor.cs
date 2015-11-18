using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    class Trapdoor:Door
    {
        public Trapdoor(int column, int row, Boolean isOpen):base(column, row, isOpen) {
            Rotate(0, 0, 90f);
            //UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position), false, false);
        }
    }
}
