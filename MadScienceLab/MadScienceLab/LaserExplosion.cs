//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework;

//namespace MadScienceLab
//{
//    class LaserExplosion:CellObject
//    {
//        bool active; // to set inactive if projectile collides with another object projectile should then be removed from list
//        SoundEffectPlayer soundEffects;
//        Texture2D explosionTexture;

//        public struct ParticleData
//        {
//            public float BirthTime;
//            public float MaxAge;
//            public Vector2 Position;
//            public float Scaling;
//            public Color ModColor;
//        }

//        List<ParticleData> particleList = new List<ParticleData>();

//        private void AddExplosion(Vector2 explosionPos, int numberOfParticles, float size, float maxAge, GameTime gameTime)
//        {
//            for (int i = 0; i < numberOfParticles; i++)
//                AddExplosionParticle(explosionPos, size, maxAge, gameTime);
//        }

//        private void AddExplosionParticle(Vector2 explosionPos, float explosionSize, float maxAge, GameTime gameTime)
//        {
//            ParticleData particle = new ParticleData();
            
//            particle.BirthTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
//            particle.MaxAge = maxAge;
//            particle.Scaling = 0.25f;
//            particle.ModColor = Color.White;
//        }

//        private void DrawExplosion()
//        {
//            for (int i = 0; i < particleList.Count; i++)
//            {
//                ParticleData particle = particleList[i];
//                spriteBatch.Draw(explosionTexture, particle.Position, null, particle.ModColor, i, new Vector2(256, 256), particle.Scaling, SpriteEffects.None, 1);
//            }
//        }

//        public LaserExplosion(int column, int row, GameConstants.POINTDIR direction):base(column, row)
//        {
            
//            active = true;
//            isCollidable = false;
            

//            // hitbox for collision
//            UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position), false, false);

//            soundEffects = new SoundEffectPlayer(this); //set up sound
//            soundEffects.LoadSound("LaserWhirLoop", GameplayScreen._sounds["LaserWhirLoop"]);
//            soundEffects.PlayAndLoopSound("LaserWhirLoop");
//        }

//        public override void Update(RenderContext renderContext)
//        {




//            base.Update(renderContext);
//        }

        
        
//    }
//}
