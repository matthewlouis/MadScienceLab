﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    public class Level:GameObject3D
    {
        public Point PlayerPoint;
        public bool LevelOver { get; set; }
    }
}
