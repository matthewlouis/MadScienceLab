﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace MadScienceLab
{
    class LaserTurret:CellObject
    {
        bool turretOn;
        GameConstants.DIRECTION direction;
        int elapsedFireTime = 0;
        int firingDelay = 3000;

        private SoundEffectPlayer soundEffects;

        //List<LaserProjectile> projectiles = new List<LaserProjectile>();
        public void SetTurret(bool turretOn)
        {
            this.turretOn = turretOn;
        }

        public LaserTurret(int column, int row, bool turretOn, GameConstants.DIRECTION direction):base(column, row)
        {
            // Load and position,rotate model based on level builder direction
            base.Model = GameplayScreen._models["Turret"];
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

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager)
        {
            //Sets up 3D sound
            soundEffects = new SoundEffectPlayer(this);
            soundEffects.LoadSound("LaserShoot", contentManager.Load<SoundEffect>("Sounds/LaserShoot"));

            base.LoadContent(contentManager);
        }

        public override void Update(RenderContext renderContext)
        {
            soundEffects.Update(renderContext); //need to update positions for 3d audio

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
                soundEffects.PlaySound("LaserShoot");
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
