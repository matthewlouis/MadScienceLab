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
        public bool LevelOver { get; set; }
    }
}
