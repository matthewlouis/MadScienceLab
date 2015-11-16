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

namespace MadScienceLab
{
    class Score
    {

        public struct ScoreData
        {
            public int levelCompleted;
            public int levels;
            public Dictionary<int, TimeSpan> levelTime;

            public ScoreData(int levels)
            {
                levelCompleted = 0;
                this.levels = levels;
                
                levelTime = new Dictionary<int, TimeSpan>();

                for (int i = 0; i < levels; i++)
                {
                    levelTime.Add(i, TimeSpan.Zero);    
                }
            }

            public void SetLevelCompleted(int levelCompleted)
            {
                this.levelCompleted = levelCompleted;
            }

            public void AddLevelTime(int level, TimeSpan time)
            {
                if (levelTime.ContainsKey(level))
                {
                    TimeSpan previousTime;
                    if (levelTime.TryGetValue(level, out previousTime))
                        if (time < previousTime)
                            levelTime[level] = previousTime;
                        ;
                }
                    
                else
                    levelTime.Add(level, time);
            }

        }

        public Score()
        {
            

        }
    }
}
