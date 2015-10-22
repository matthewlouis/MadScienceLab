using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace MadScienceLab
{
    /**
     * A 3D Game Object from which all 3d objects in our game will inherit.
     */
    public abstract class GameObject3D
    {
        //position relative to self
        public Vector3 Position { get; set; }
        //poisition relative to world
        public Vector3 WorldPosition { get; private set; }

        //Object translational velocity, acceleration per second (or 30 frames)
        public Vector3 TransVelocity { get; set; }
        public Vector3 TransAccel { get; set; }

        //Object rotation -- Quaternion (4d vector) required
        public Quaternion LocalRotation { get; set; }
        public Quaternion WorldRotation { get; private set; }

        //Scale-size
        public Vector3 LocalScale { get; set; }
        public Vector3 WorldScale { get; private set; }

        //Properties for setting up parent/child relationships
        public GameObject3D Parent { get; private set; }
        public List<GameObject3D> Children { get; private set; }

        // Hit box information - Steven
        public int HitboxWidth;
        public int HitboxHeight;
        public int HitboxWidthOffset;
        public int HitboxHeightOffset;

        //public BoundingBox Hitbox { get; set; }
        public virtual Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X + HitboxWidthOffset, (int)Position.Y + HitboxHeightOffset, HitboxWidth, HitboxHeight);
            }
        }
        //compiled version of position, rotation, and scale
        //Allows for easier transformations
        protected Matrix WorldMatrix;

        float verticalOffset;
        public void SetVerticalOffset(float verticalOffset)
        {
            this.verticalOffset = verticalOffset;
        }

        float horizontalOffset;
        public void SetHorizontalOffset(float horizontalOffset)
        {
            this.horizontalOffset = horizontalOffset;

        }

        public Matrix GetWorldMatrix()
        {
            return WorldMatrix;
        }

        /**
         * Constructor
         */
        protected GameObject3D()
        {
            Children = new List<GameObject3D>();
            LocalScale = WorldScale = Vector3.One; //default is one

            verticalOffset = 0;
        }

        /**
         * Add 3D Child Object
         */
        public void AddChild(GameObject3D child)
        {
            if (!Children.Contains(child))
            {
                child.Parent = this;
                Children.Add(child);
            }
        }

        /**
         * Remove 3D Child Object
         */
        public void RemoveChild(GameObject3D child)
        {
            if (Children.Remove(child)) //if successful
            {
                child.Parent = null; //remove this parent from the child obkect
            }
        }

        /**
         * Changes position of the GameObject3D
         */
        public void Translate(Vector3 translation)
        {
            Position = translation;
        }

        /**
         *  Changes position of the GameObject3D
         */
        public void Translate(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
        }

        /**
         * Changes the scale of the GameObject3D
         */
        public void Scale(Vector3 scale)
        {
            LocalScale = scale;
        }

        /**
         * Changes the scale of the GameObject3D
         */
        public void Scale(float x, float y, float z)
        {
            LocalScale = new Vector3(x, y, z);
        }

        /**
         * Rotates the GameObject3D
         */
        public void Rotate(Quaternion rotation)
        {
            LocalRotation = rotation;
        }

        /**
         * Rotates the GameObject3D
         */
        public void Rotate(float pitch, float yaw, float roll)
        {
            LocalRotation = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(yaw), MathHelper.ToRadians(pitch), MathHelper.ToRadians(roll));
        }

        public virtual void Initialize()
        {
            Children.ForEach(child => child.Initialize());
        }

        public virtual void LoadContent(ContentManager contentManager)
        {
            Children.ForEach(child => child.LoadContent(contentManager));
        }

        public virtual void Update(RenderContext renderContext)
        {
            //Uses helper methods to create compiled WorldMatrix
            WorldMatrix = Matrix.CreateFromQuaternion(LocalRotation) *
                          Matrix.CreateScale(LocalScale) *
                          Matrix.CreateTranslation(new Vector3(Position.X - horizontalOffset,Position.Y - verticalOffset, Position.Z));



            Children.ForEach(child => child.Update(renderContext));
        }

        //call draw on all children
        public virtual void Draw(RenderContext renderContext)
        {
            Children.ForEach(child => child.Draw(renderContext));
        }

        public virtual void CheckCollision(Level level)
        {

            foreach (CellObject levelObject in level.Children)
            {
                if (levelObject.isCollidable && Hitbox.Intersects(levelObject.Hitbox) && levelObject != this)
                {
                    /**Determining what side was hit**/
                    float wy = (levelObject.Hitbox.Width + Hitbox.Width)
                             * (((levelObject.Hitbox.Y + levelObject.Hitbox.Height) / 2) - (Hitbox.Y + Hitbox.Height) / 2);
                    float hx = (Hitbox.Height + levelObject.Hitbox.Height)
                             * (((levelObject.Hitbox.X + levelObject.Hitbox.Width) / 2) - (Hitbox.X + Hitbox.Width) / 2);

                    Button tmpButton = levelObject as Button;
                    if (tmpButton != null) //if it is a button
                    {
                        Button button = (Button)levelObject as Button;
                        button.IsPressed = true;
                    }
                    if (!levelObject.IsPassable)
                    { //same logic as Character- if levelObject is passable, no need to handle physics
                        if (wy > hx)
                        {
                            if (wy > -hx) //hitting something above
                            {
                                //Game1.boxHitState = "Box Top";//top
                                Position = new Vector3(Position.X, levelObject.Hitbox.Bottom - this.Hitbox.Height, 0); //clip to the top of the colliding object

                                //end all downward acceleration (eg. gravity) and velocity
                                //TransAccel = Vector3.Zero;
                                TransVelocity = new Vector3(TransVelocity.X, Math.Min(TransVelocity.Y, GameConstants.SINGLE_CELL_SIZE * 2), TransVelocity.Z); //cease upward velocity
                            }
                            else
                            {
                                //boxHitState = "Box Left";// left
                                Position = new Vector3(levelObject.Hitbox.Right, Position.Y, 0);
                                TransVelocity = new Vector3(Math.Max(TransVelocity.X, -GameConstants.SINGLE_CELL_SIZE * 2), TransVelocity.Y, TransVelocity.Z); //cease velocity
                                //adjacentObj = levelObject;
                            }
                        }
                        else
                        {
                            if (wy > -hx)
                            {
                                //boxHitState = "Box Right";// right
                                Position = new Vector3(levelObject.Hitbox.Left - this.Hitbox.Width, Position.Y, 0);
                                TransVelocity = new Vector3(Math.Min(TransVelocity.X, GameConstants.SINGLE_CELL_SIZE * 2), TransVelocity.Y, TransVelocity.Z); //cease velocity
                                //adjacentObj = levelObject;
                            }
                            else
                            { //hitting something below
                                //boxHitState = "Box Bottem";//bottem
                                //Game1.boxPosition = "{"+Position.X+", "+Position.Y+", "+Position.Z+"}";
                                Position = new Vector3(Position.X, levelObject.Hitbox.Y + this.Hitbox.Height, 0); //clip to the top of the colliding object
                                TransVelocity = new Vector3(TransVelocity.X, Math.Max(TransVelocity.Y, -GameConstants.SINGLE_CELL_SIZE * 2), TransVelocity.Z); //cease velocity
                                /*Note that not all velocity is reset, to avoid an issue with 'box shaking,' due to:
                                 *  when velocity would be reset, there would be at least one frame where
                                 *  box would change position (eg. from gravity), that position would be large enough to make a difference in how it's drawn
                                 *  while not reaching a value that would lead to collision （ie. an intersection of hitboxes (which would be truncated
                                 *  integers representing position)
                                 *  and another frame where box would be in its original position due to the collision
                                */
                                //jumping = false;
                            }
                        }
                    }
                }
                //boxHitState = "No box";
            }
        }
    }
}
