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

        //public BoundingBox Hitbox { get; set; }
        public Rectangle Hitbox { get; set; }
        //compiled version of position, rotation, and scale
        //Allows for easier transformations
        protected Matrix WorldMatrix;

        float verticalOffset;
        public void SetVerticalOffset(float verticalOffset)
        {
            this.verticalOffset = verticalOffset;
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
                          Matrix.CreateTranslation(new Vector3(Position.X,Position.Y - verticalOffset, Position.Z));



            Children.ForEach(child => child.Update(renderContext));
        }

        //call draw on all children
        public virtual void Draw(RenderContext renderContext)
        {
            Children.ForEach(child => child.Draw(renderContext));
        }
    }
}
