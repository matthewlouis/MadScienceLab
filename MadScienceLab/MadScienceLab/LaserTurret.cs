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
        GameConstants.DIRECTION direction;
        int elapsedFireTime = 0;
        int firingDelay = 1500;

        //List<LaserProjectile> projectiles = new List<LaserProjectile>();
        public void SetTurret(bool turretOn)
        {
            this.turretOn = turretOn;
        }

        public LaserTurret(int column, int row, bool turretOn, GameConstants.DIRECTION direction):base(column, row)
        {
            // Load and position,rotate model based on level builder direction
            base.Model = Game1._models["Turret"];
            if(direction == GameConstants.DIRECTION.pointLeft)
            {
                RotateLeft();
            }
            else
                if(direction == GameConstants.DIRECTION.pointRight)
                {
                    RotateRight();
                }
            SetVerticalOffset(20);
            
            // set turret to on state
            this.turretOn = turretOn;

            // set turret facing direction
            this.direction = direction;

            // collision handling
            base.isCollidable = true;
            UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position), true, true);
        }

        public override void Update(RenderContext renderContext)
        { 
            if (turretOn)
                FireProjectile(renderContext);
            
            base.Update(renderContext);
        }

        private void FireProjectile(RenderContext renderContext)
        {
            // fire projectile using a delay
            elapsedFireTime += renderContext.GameTime.ElapsedGameTime.Milliseconds;
            if(elapsedFireTime > firingDelay)
            {
                elapsedFireTime = 0;
                //projectiles.Add(new LaserProjectile(CellNumber.X, CellNumber.Y, direction));
                renderContext.Level.AddChild(new LaserProjectile(CellNumber.X, CellNumber.Y, direction));
            }
        }

        public void RotateLeft()
        {
            base.Rotate(0f,90f,0f);
        }

        public void RotateRight()
        {
            base.Rotate(0f, 270f, 0f);
        }



    }
}
