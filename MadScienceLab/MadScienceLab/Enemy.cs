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

        float damageDelayTime;
        TimeSpan DAMAGE_DELAY = TimeSpan.FromMilliseconds(1000f);
        TimeSpan timeHit = TimeSpan.Zero;
        int TIME_DELAY = 1000;
        int elaspedTime = 0;
        int attackRange = 4;
        float movementAmount = 1f;
        float speed = 1f;

        bool movestate = true;
        private SoundEffectPlayer soundEffects;

        private GameAnimatedModel animmodel;

        Texture2D texture;
        public Rectangle enemyRangeL;
        public Rectangle enemyRangeR;
        public List<GameObject3D> wallsToCheck;

        public Enemy(int column, int row, RenderContext rendeerContext)
            : base(column, row)
        {
            texture = new Texture2D(rendeerContext.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.Gray });

            animmodel = new GameAnimatedModel("Doomba", column, row, this);
            animmodel.PlayAnimation("Move", true, 0f);

            isCollidable = false;

            wallsToCheck = new List<GameObject3D>();
            Scale(40f, 40f, 40f); // 48,48,48
            Position = new Vector3(Position.X, Position.Y - 18, Position.Z);  // position.y - 18          
        }

        public Enemy()
        {
            // TODO: Complete member initialization
        }

        public override void LoadContent(ContentManager contentManager)
        {
            animmodel.LoadContent(contentManager);
            // Provides a hitbox for the block - Steven
            UpdateBoundingBox(animmodel.Model, Matrix.CreateTranslation(base.Position), false, false);
            base.HitboxWidth = 40;
            base.HitboxHeight = 16;
            base.HitboxHeightOffset = 20;
            soundEffects = new SoundEffectPlayer(this);
            SoundEffect sound = contentManager.Load<SoundEffect>("Sounds/DoombaLoop");
            soundEffects.LoadSound("Roomba", contentManager.Load<SoundEffect>("Sounds/DoombaLoop"));
            soundEffects.PlayAndLoopSound("Roomba");
            base.LoadContent(contentManager);

            //attack range
            enemyRangeL = new Rectangle((int)(Position.X - (HitboxWidth * 10)), (int)(Position.Y + (HitboxHeight * 1.5)), (HitboxWidth * 10), (HitboxHeight));
            enemyRangeR = new Rectangle((int)(Position.X + (HitboxWidth)), (int)(Position.Y + (HitboxHeight * 1.5)), (HitboxWidth * 10), (HitboxHeight));

            // generate random direction and initialize
            Random rand = new Random();
            if (rand.Next(0, 1) == 0)
                direction = GameConstants.POINTDIR.pointLeft;
            else
                direction = GameConstants.POINTDIR.pointRight;
        }

        public override void Update(RenderContext renderContext)
        {



            if (direction == GameConstants.POINTDIR.pointLeft)
            {
                MoveLeft(movementAmount * speed);
            }
            else if (direction == GameConstants.POINTDIR.pointRight)
            {
                MoveRight(movementAmount * speed);

            }

            enemyRangeL.X = (int)(Position.X - (HitboxWidth * 10));
            enemyRangeL.Y = (int)(Position.Y + HitboxHeight * 1.5);

            enemyRangeR.X = (int)(Position.X + HitboxWidth);
            enemyRangeR.Y = (int)(Position.Y + HitboxHeight * 1.5);



            CheckEnemyBoxCollision(renderContext);

            if (CheckPlayerNearby(renderContext))
            {
                if (direction == GameConstants.POINTDIR.pointLeft)
                {
                    MoveLeft((int)(GameConstants.MOVEAMOUNT * 1.5));
                }

                else if (direction == GameConstants.POINTDIR.pointRight)
                {
                    MoveRight((int)(GameConstants.MOVEAMOUNT * 1.5));
                }
            }

            soundEffects.Update(renderContext);
            animmodel.Update(renderContext);

            base.Update(renderContext);
        }

        public override void Draw(RenderContext renderContext)
        {

            animmodel.Draw(renderContext);

            //renderContext.SpriteBatch.Begin();
            //renderContext.SpriteBatch.Draw(texture, enemyRangeL, Color.Black);
            //renderContext.SpriteBatch.Draw(texture, enemyRangeR, Color.Black);
            //renderContext.SpriteBatch.End();
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
            elaspedTime += renderContext.GameTime.ElapsedGameTime.Milliseconds;
            if (renderContext.Player.Hitbox.Intersects(Hitbox))
            {
                renderContext.Player.TakeDamage(GameConstants.PLAYER_DAMAGE, renderContext.GameTime);
                movestate = !movestate;

                if (direction == GameConstants.POINTDIR.pointLeft && elaspedTime > TIME_DELAY)
                {

                    direction = GameConstants.POINTDIR.pointRight;
                    elaspedTime = 0;

                    return;
                }
                else if (direction == GameConstants.POINTDIR.pointRight && elaspedTime > TIME_DELAY)
                {

                    direction = GameConstants.POINTDIR.pointLeft;
                    elaspedTime = 0;

                    return;
                }

                return;
            }
            List<GameObject3D> checkBlocks = new List<GameObject3D>();
            checkBlocks.AddRange(renderContext.Level.gameObjects[typeof(PickableBox)]);
            checkBlocks.AddRange(renderContext.Level.gameObjects[typeof(Door)]);
            checkBlocks.AddRange(wallsToCheck);
            //Check collision of enemy against all objects in level except toggle switch and 
            foreach (CellObject levelObject in checkBlocks)
            {

                if ((levelObject.isCollidable && Hitbox.Intersects(levelObject.Hitbox)
                    && levelObject.GetType() != typeof(ToggleSwitch)) ||
                    (Hitbox.Intersects(levelObject.Hitbox) && levelObject.GetType() == typeof(Character)))
                {
                    if (levelObject.GetType() == typeof(Character))
                    {

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
        private bool CheckPlayerNearby(RenderContext renderContext)
        {
            List<GameObject3D> checkBlocks = new List<GameObject3D>();
            checkBlocks.AddRange(renderContext.Level.gameObjects[typeof(PickableBox)]);
            checkBlocks.AddRange(wallsToCheck);
            //check the if the player in the attack range of left side. and enemy face left.
            if (enemyRangeL.Intersects(renderContext.Player.Hitbox) && direction == GameConstants.POINTDIR.pointLeft)
            {
                foreach (CellObject rectangle in checkBlocks)
                {   //check the obejcet in the box , if it between the enemy and player. than enemy will speed up
                    if (rectangle.Hitbox.X > renderContext.Player.Position.X && rectangle.Hitbox.Intersects(enemyRangeL))
                    {
                        return false;
                    }
                }
                return true;
            }
            //same with the left side, now just check right side.
            else if (enemyRangeR.Intersects(renderContext.Player.Hitbox) && direction == GameConstants.POINTDIR.pointRight)
            {
                foreach (CellObject rectangle in checkBlocks)
                {
                    if (rectangle.Hitbox.X < renderContext.Player.Position.X && rectangle.Hitbox.Intersects(enemyRangeR))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

    }
}



