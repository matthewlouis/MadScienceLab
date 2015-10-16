using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    class LaserTurret:CellObject
    {
        bool turretOn;
        float fireRate;
        float direction;


        public void SetTurret(bool turretOn)
        {
            this.turretOn = turretOn;
        }

        public LaserTurret(int column, int row, bool turretOn):base(column, row)
        {
            base.Model = Game1._models["Turret"];
            this.turretOn = turretOn;
            Rotate(0f, 90f, 0f);
            SetVerticalOffset(20);
            Scale(24,24,24);

            // hitbox for collision
            UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position), true, true);
        }

        public override void Update(RenderContext renderContext)
        {
            if (turretOn)
                FireProjectile();
            
            base.Update(renderContext);
        }

        private void FireProjectile()
        {
            
        }
    }
}
