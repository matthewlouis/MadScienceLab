using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    class LaserProjectile:CellObject
    {
        bool active; // to set inactive if projectile collides with another object projectile should then be removed from list
        GameConstants.DIRECTION direction;
        
        public LaserProjectile(int column, int row, GameConstants.DIRECTION direction):base(column, row)
        {
            base.Model = Game1._models["projectile"];
            //Rotate(0f, 0f, 90f);
            SetVerticalOffset(-2);
            //SetHorizontalOffset(30);
            active = true;
            this.direction = direction;
            isCollidable = true;

            SetDirection();
            

            // hitbox for collision
            UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position), false, false);
        }

        public override void Update(RenderContext renderContext)
        {
            if (active)
            {
                CheckProjectileCollision(renderContext);
                Position += TransVelocity;
            }
            else
                renderContext.Level.RemoveChild(this);
            base.Update(renderContext);
        }

        public void SetDirection()
        {
            if (direction == GameConstants.DIRECTION.pointLeft)
            {
                SetHorizontalOffset(30);
                base.TransVelocity = new Vector3(-GameConstants.PROJECTILE_X_VELOCITY,0f,0f);
            }
            else
                if (direction == GameConstants.DIRECTION.pointRight)
                {
                    SetHorizontalOffset(-30);
                    base.TransVelocity = new Vector3(GameConstants.PROJECTILE_X_VELOCITY,0f,0f);
                }
                else
                TransVelocity = new Vector3(0);
        }

        public void CheckProjectileCollision(RenderContext renderContext)
        {
            // if collision with player, handle
            if(this.Hitbox.Intersects(renderContext.Player.Hitbox))
                {
                    active = false;
                    TransVelocity = Vector3.Zero;
                    renderContext.Player.SetHealth(GameConstants.PLAYER_DAMAGE);
                }
                Position += TransVelocity;
            
            /*
            foreach (CellObject worldOject in renderContext.Level.Children)
            {
                if (Hitbox.Intersects(worldOject.Hitbox) && worldOject.GetType() != typeof(LaserTurret))
                    
                {
                    active = false;
                    TransVelocity = Vector3.Zero;
                    renderContext.Player.SetHealth(GameConstants.PLAYER_DAMAGE);
                }
                Position += TransVelocity;
            }*/
        }


        
    }
}
