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
        public GameConstants.POINTDIR direction {
            get {
                return _direction;
            }
            set {
                _direction = value;
                if (value == GameConstants.POINTDIR.pointLeft) {
                    RotateLeft ();
                }
                else if (value == GameConstants.POINTDIR.pointRight) {
                    RotateRight ();
                }
            }
        }
        private GameConstants.POINTDIR _direction;

        int elapsedFireTime = 0;
        int firingDelay = 3000;

        //intended to be only assigned once - or, used as a method to increment the fire time.
        public int elapsedFireTimeOffset { set { elapsedFireTime += value; } }
        
        private SoundEffectPlayer soundEffects;

        //List<LaserProjectile> projectiles = new List<LaserProjectile>();
        public void SetTurret(bool turretOn)
        {
            this.turretOn = turretOn;
        }

        public LaserTurret(int column, int row, bool turretOn, GameConstants.POINTDIR direction):base(column, row)
        {
            // Load and position,rotate model based on level builder direction
            base.Model = GameplayScreen._models["Turret"];
            this.direction = direction;
            SetVerticalOffset(20);
            
            // set turret to on state
            this.turretOn = turretOn;

            // collision handling
            base.isCollidable = true;
            UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position), true, true);
            base.HitboxHeightOffset = -5;
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
                LaserProjectile projectile = new LaserProjectile(CellNumber.X, CellNumber.Y, _direction);

                //Position the projectile according to the position of the turret
                float projectileX;
                if (direction == GameConstants.POINTDIR.pointLeft)
                    projectileX = this.Hitbox.Left;
                else
                    projectileX = this.Hitbox.Right - projectile.Hitbox.Width / 2;
                projectile.Translate ( projectileX, this.Hitbox.Top - base.HitboxHeightOffset, this.zPosition ); //position the projectile to be a position relative to the turret
                
                renderContext.Level.AddChild(projectile);
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
