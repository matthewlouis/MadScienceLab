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
        bool facingBottom = false;
        public Trapdoor(int column, int row, Boolean isOpen):base(column, row, isOpen) {
            int verticalOffset = -GameConstants.SINGLE_CELL_SIZE / 2 + 5;
            Translate(this.Position.X, this.Position.Y - verticalOffset, this.Position.Z);
            Rotate(0, 0, 90f);
            UpdateBoundingBox(GameplayScreen._models["BasicBlock"], /*Matrix.CreateRotationZ(90f) * */Matrix.CreateTranslation(base.Position), false, false);
            HitboxHeightOffset = 15; //Lower the hitbox
            HitboxHeight = 12;
            HitboxWidth = 48;
            //HitboxHeight /= 2;
        }

        
        public void faceBottom ()
        {
            if (!facingBottom)
            {
                facingBottom = true;
                int verticalOffset = -GameConstants.SINGLE_CELL_SIZE / 2 + 5;
                Translate ( this.Position.X, this.Position.Y + 2*verticalOffset, this.Position.Z ); //translate back to its orig. position then another verticalOffset
                Rotate ( 0, 0, 90f );
                UpdateBoundingBox ( GameplayScreen._models["BasicBlock"], /*Matrix.CreateRotationZ(90f) * */Matrix.CreateTranslation ( base.Position ), false, false );
                HitboxHeightOffset = -verticalOffset; //Move hitbox higher? Put the hitbox on the other side of it.
            }
        }
    }
}
