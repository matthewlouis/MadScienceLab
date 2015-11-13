using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    public class Level:GameObject3D
    {
        public Point PlayerPoint;
        public Dictionary<Type, List<GameObject3D>> gameObjects;
        public List<GameObject3D> collidableObjects;
        public bool LevelOver { get; set; }
        public bool GameOver { get; set; }

        Type[] types;

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager)
        {
            gameObjects = new Dictionary<Type, List<GameObject3D>>();
            Type[] types = { typeof(PickableBox), typeof(ToggleSwitch), typeof(Door), typeof(Button), typeof(BasicBlock), typeof(LaserTurret), typeof(MovingPlatform), typeof(Character) };
            foreach (Type type in types)
            {
                gameObjects.Add(type, new List<GameObject3D>());
            }
            this.types = types;
            base.LoadContent(contentManager);
        }


        public override void Update(RenderContext renderContext)
        {
            
            base.Update(renderContext);
            PopulateTypeList(renderContext);
        }

        /// <summary>
        /// Populates the gameObjects map of list of game objects by type.
        /// </summary>
        /// <param name="renderContext"></param>
        public void PopulateTypeList(RenderContext renderContext)
        {

            gameObjects = new Dictionary<Type, List<GameObject3D>>();
            
            foreach (Type type in types)
            {
                gameObjects.Add(type, new List<GameObject3D>());
            }
            foreach (GameObject3D Child in this.Children)
            {
                if (!gameObjects.ContainsKey(Child.GetType())) //add any object types as found in the level
                {
                    gameObjects.Add(Child.GetType(), new List<GameObject3D>());
                }
                gameObjects[Child.GetType()].Add(Child);
            }
            gameObjects[typeof(Character)].Add(renderContext.Player);

            this.gameObjects = gameObjects;
        }
    }
}
