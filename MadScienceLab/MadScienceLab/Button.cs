using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    class Button:CellObject
    {
        public List<Door> LinkedDoors { get; set; }
        public Boolean IsPressed { get; set; }
        private Boolean doorsToggled = false;

        public Button(int column, int row):base(column, row)
        {
            LinkedDoors = new List<Door>();
            base.Model = Game1._models["button"];
            base.isCollidable = true;
            Translate(Position.X, Position.Y - Game1.SINGLE_CELL_SIZE / 2 + 1, Position.Z); //Matt: this is for offsetting the model position so it's flat on the floor

            // Provides a hitbox for the block - Steven
            BoundingBox box = UpdateBoundingBox(base.Model, Matrix.CreateTranslation(base.Position));
            Vector3 size = box.Max - box.Min;
            base.Hitbox = new Rectangle((int)Position.X, (int)Position.Y, (int)size.X, (int)size.Y);
        }

        public override void Update(RenderContext renderContext)
        {
            
            if (IsPressed && doorsToggled == false)
            {
                doorsToggled = true;
                foreach (Door door in LinkedDoors)
                {
                    door.Toggle();
                }
            }
            else if (!IsPressed && doorsToggled == true)
            {
                doorsToggled = false;
                foreach (Door door in LinkedDoors)
                {
                    door.Toggle();
                }
            }
            IsPressed = false; //button resets to up unless current pressed
        }

        public override void Draw(RenderContext _renderContext)
        {
            //Jacob: These lines don't seem to be used anymore.
            //var transforms = new Matrix[model.Bones.Count];
            //model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in base.Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.View = _renderContext.Camera.View;
                    effect.Projection = _renderContext.Camera.Projection;
                    effect.World = Matrix.CreateTranslation(Position);
                }
                mesh.Draw();
            }
        }
    }
}
