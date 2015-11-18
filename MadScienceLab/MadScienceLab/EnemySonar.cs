using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    class EnemySonar : CellObject
    {
        bool active; // to set inactive if projectile collides with another object projectile should then be removed from list
        GameConstants.POINTDIR direction;

        public EnemySonar(Vector2 position, GameConstants.POINTDIR direction) : base(position)
        {
            active = true;
            this.direction = direction;
            isCollidable = false;

            
            base.Model = GameplayScreen._models["projectile"];

            // hitbox for collision
            UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position), false, false);
            SetVerticalOffset(-20);
            SetDirection();
            base.HitboxHeight = 10;
            //base.HitboxWidth = 24;
            //base.HitboxWidthOffset = 48;
            base.HitboxHeightOffset = 48;

        }

        public override void Update(RenderContext renderContext)
        {
            List<CellObject> returnObjs = new List<CellObject>();

            renderContext.Quadtree.clear();
            foreach (CellObject obj in renderContext.Level.collidableObjects)
            {
                if (obj.GetType() != typeof(LaserTurret))
                    renderContext.Quadtree.insert(obj);
            }

            renderContext.Quadtree.retrieve(returnObjs, Hitbox);


            if (active)
            {

                CheckSonarCollision(renderContext, returnObjs);
                Position += TransVelocity;
            }
            else
            {
                renderContext.Level.RemoveChild(this);
            }



            base.Update(renderContext);
        }

        public override void Draw(RenderContext renderContext)
        {

            base.Draw(renderContext);
        }

        public void SetDirection()
        {
            if (direction == GameConstants.POINTDIR.pointLeft)
            {
                //SetVerticalOffset(-30);
                //Position += new Vector3(0, 0, 0);
                base.TransVelocity = new Vector3(-GameConstants.PROJECTILE_X_VELOCITY, 0f, 0f);
            }
            else
                if (direction == GameConstants.POINTDIR.pointRight)
            {
                //SetVerticalOffset(-30);
                //Position += new Vector3(0, 0, 0);
                base.TransVelocity = new Vector3(GameConstants.PROJECTILE_X_VELOCITY, 0f, 0f);
            }
            else
                TransVelocity = new Vector3(0);
        }

        public void CheckSonarCollision(RenderContext renderContext, List<CellObject> returnObjs)
        {
            
            foreach (CellObject worldObject in returnObjs)
            {
                if (!worldObject.IsPassable && Hitbox.Intersects(worldObject.Hitbox) && worldObject.GetType() != typeof(Enemy) )
                {
                    if (worldObject.GetType() == typeof(Character))
                    {
                        active = false;

                        TransVelocity = Vector3.Zero;
                    }
                    else
                    {
                        active = false;
                        TransVelocity = Vector3.Zero;
                    }
                }
            }
        }



    }
}
