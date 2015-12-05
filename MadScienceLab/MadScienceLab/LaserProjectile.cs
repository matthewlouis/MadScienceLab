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
        GameConstants.POINTDIR direction;
        SoundEffectPlayer soundEffects;
        
        public LaserProjectile(int column, int row, GameConstants.POINTDIR direction):base(column, row)
        {
            base.Model = GameplayScreen._models["projectile"];
            //Rotate(0f, 0f, 90f);
            //SetVerticalOffset();
            //SetHorizontalOffset(30);
            active = true;
            this.direction = direction;
            isCollidable = false ;
            
            SetDirection();
            

            // hitbox for collision
            UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position), false, false);

            soundEffects = new SoundEffectPlayer(this); //set up sound
            soundEffects.LoadSound("LaserWhirLoop", GameplayScreen._sounds["LaserWhirLoop"]);
            soundEffects.PlayAndLoopSound("LaserWhirLoop");
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
                soundEffects.Update(renderContext);
                CheckProjectileCollision(renderContext, returnObjs);
                Position += TransVelocity;
            }
            else
            {
                renderContext.Level.RemoveChild(this);

                //Stop sound and remove resource
                soundEffects.SoundInstances["LaserWhirLoop"].Dispose();
            }



            base.Update(renderContext);
        }

        public void SetDirection()
        {
            if (direction == GameConstants.POINTDIR.pointLeft)
            {
                
                //SetHorizontalOffset(30);
                //Position += new Vector3(-30,0,0);
                base.TransVelocity = new Vector3(-GameConstants.PROJECTILE_X_VELOCITY,0f,0f);
            }
            else
                if (direction == GameConstants.POINTDIR.pointRight)
                {
                    //SetHorizontalOffset(-30);
                    //Position += new Vector3(30, 0, 0);
                    base.TransVelocity = new Vector3(GameConstants.PROJECTILE_X_VELOCITY,0f,0f);
                }
                else
                TransVelocity = new Vector3(0);
        }

        public void CheckProjectileCollision(RenderContext renderContext, List<CellObject> returnObjs)
        {
            // if collision with player, handle
           /* if(this.Hitbox.Intersects(renderContext.Player.Hitbox))
            {
                active = false;
                TransVelocity = Vector3.Zero;
                renderContext.Player.SetHealth(GameConstants.PLAYER_DAMAGE);
            }*/

            // Quad tree collison checks - Steven
            foreach (CellObject worldObject in returnObjs)
            {
                if (!worldObject.IsPassable && Hitbox.Intersects(worldObject.Hitbox))
                {
                    if (worldObject.GetType() == typeof(Character))
                    {
                        active = false;
                        renderContext.Player.TakeDamage(GameConstants.PLAYER_DAMAGE, renderContext.GameTime);
                        TransVelocity = Vector3.Zero;
                    }
                    else
                    {
                        active = false;
                        TransVelocity = Vector3.Zero;
                    }
                }
            }

            //foreach (GameObject3D worldObject in renderContext.Level.gameObjects[typeof(Character)])
            //{
            //    if (Hitbox.Intersects(worldObject.Hitbox) && worldObject.GetType() != typeof(LaserTurret))
            //    {
            //        active = false;
            //        TransVelocity = Vector3.Zero;
            //        renderContext.Player.SetHealth(GameConstants.PLAYER_DAMAGE);
            //    }
            //}

            //foreach (GameObject3D worldObject in renderContext.Level.gameObjects[typeof(BasicBlock)])
            //{
            //    if (Hitbox.Intersects(worldObject.Hitbox) && worldObject.GetType() != typeof(LaserTurret))
            //    {
            //        active = false;
            //        TransVelocity = Vector3.Zero;
            //    }
            //}

            //foreach (GameObject3D worldObject in renderContext.Level.gameObjects[typeof(PickableBox)])
            //{
            //    if (Hitbox.Intersects(worldObject.Hitbox) && worldObject.GetType() != typeof(LaserTurret))
            //    {
            //        active = false;
            //        TransVelocity = Vector3.Zero;
            //    }
            //}
            //Position += TransVelocity;

        }


        
    }
}
