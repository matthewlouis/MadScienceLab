using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections;


namespace MadScienceLab
{
    public class Enemy : CellObject
    {
        private GameConstants.POINTDIR direction;
        public GameConstants.POINTDIR Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
            }
        }

        int attackRange = 4;
        float movementAmount = 1f;
        float speed = 1f;

        bool movestate = true;
        private SoundEffectPlayer soundEffects;

        private GameAnimatedModel animmodel;

        public Enemy(int column, int row)
            : base(column, row)
        {
            animmodel = new GameAnimatedModel("Doomba", column, row, this);
            animmodel.PlayAnimation("Move", true, 0f);

            isCollidable = false;

            Scale(48f, 48f, 48f);
            Position = new Vector3(Position.X, Position.Y - 18, Position.Z);
        }

        public override void LoadContent(ContentManager contentManager)
        {
            animmodel.LoadContent(contentManager);
            // Provides a hitbox for the block - Steven
            UpdateBoundingBox(animmodel.Model, Matrix.CreateTranslation(base.Position), false, false);
            base.HitboxWidth = 48;
            base.HitboxHeight = 20;
            base.HitboxHeightOffset = 20;
            soundEffects = new SoundEffectPlayer(this);
            SoundEffect sound = contentManager.Load<SoundEffect>("Sounds/DoombaLoop");
            soundEffects.LoadSound("Roomba", contentManager.Load<SoundEffect>("Sounds/DoombaLoop"));
            soundEffects.PlayAndLoopSound("Roomba");
            base.LoadContent(contentManager);

            // generate random direction and initialize
            Random rand = new Random();
            if (rand.Next(0, 1) == 0)
                direction = GameConstants.POINTDIR.pointLeft;
            else
                direction = GameConstants.POINTDIR.pointRight;
           
            
        }

        public override void Update(RenderContext renderContext)
        {

            List<CellObject> returnObjs = new List<CellObject>();
            renderContext.Quadtree.clear();
            foreach (CellObject obj in renderContext.Level.collidableObjects)
            {
                    renderContext.Quadtree.insert(obj);
            }

            renderContext.Quadtree.retrieve(returnObjs, Hitbox);

            if (direction == GameConstants.POINTDIR.pointLeft)
            {
                MoveLeft(movementAmount * speed);
            }
            else if(direction == GameConstants.POINTDIR.pointRight)
            {
                MoveRight(movementAmount * speed);

            }
            

            CheckPlayerNearby(renderContext); 
            CheckEnemyBoxCollision(renderContext);
            soundEffects.Update(renderContext);
            animmodel.Update(renderContext);

            base.Update(renderContext);
        }

        public override void Draw(RenderContext renderContext)
        {
            animmodel.Draw(renderContext);
        }

        public void MoveLeft(float movementAmount)
        {
            //facingDirection = FACING_LEFT;
            Vector3 newPosition = Position + new Vector3(-movementAmount, 0, 0);
            Rotate(0, -90f, 0);
            Translate(newPosition);
        }

        public void MoveRight(float movementAmount)
        {
            //facingDirection = FACING_RIGHT;
            Vector3 newPosition = Position + new Vector3(movementAmount, 0, 0);
            Rotate(0, 90f, 0);
            Translate(newPosition);
        }

        private void CheckEnemyBoxCollision(RenderContext renderContext)
        {
            //Check collision of enemy against all objects in level except toggle switch and 
            foreach (CellObject levelObject in renderContext.Level.collidableObjects)
            {                
                if ((levelObject.isCollidable && Hitbox.Intersects(levelObject.Hitbox)
                    && levelObject.GetType() != typeof(ToggleSwitch)) || 
                    (Hitbox.Intersects(levelObject.Hitbox) && levelObject.GetType() == typeof(Character)))
                {
                    if(levelObject.GetType() == typeof(Character))
                    {
                        renderContext.Player.TakeDamage(GameConstants.PLAYER_DAMAGE, renderContext.GameTime);
                        movestate = !movestate;
                        return;
                    }
                    float wy = (levelObject.Hitbox.Width + Hitbox.Width)
                            * (levelObject.Hitbox.Center.Y - Hitbox.Center.Y);
                    float hx = ((Hitbox.Height) + levelObject.Hitbox.Height)
                             * (levelObject.Hitbox.Center.X - Hitbox.Center.X);

                    if (wy > hx)
                    {
                        //boxHitState = "Box Left";// left
                        movestate = false;
                        direction = GameConstants.POINTDIR.pointRight;

                    }
                    if (wy > -hx)
                    {
                        //boxHitState = "Box Right";// right
                        movestate = true;
                        direction = GameConstants.POINTDIR.pointLeft;
                    }

                }
            }
        }

        /// <summary>
        /// Checkes based on a hitbox whether the player is nearby the enemy and sets enemy volicity while in range
        /// </summary>
        /// <param name="renderContext"></param>
        private void CheckPlayerNearby(RenderContext renderContext)
        {
            
        }


    }


}



