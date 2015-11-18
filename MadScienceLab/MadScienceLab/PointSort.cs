using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace MadScienceLab
{
    public class PointSort : IComparer<Point>
    {
        public enum Mode
        {
            X,
            Y
        }

        Mode currentMode = Mode.X;

        public PointSort(Mode mode)
        {
            currentMode = mode;
        }


        //Comparing function 
        //Returns one of three values - 0 (equal), 1 (greater than), 2 (less than) 
        public int Compare(Point a, Point b)
        {
            Point point1 = (Point)a;
            Point point2 = (Point)b;

            if (currentMode == Mode.X) //Compare X values 
            {
                if (point1.X > point2.X)
                    return 1;
                else if (point1.X < point2.X)
                    return -1;
                else
                    return 0;
            }
            else
            {
                if (point1.Y > point2.Y) //Compare Y Values 
                    return 1;
                else if (point1.Y < point2.Y)
                    return -1;
                else
                    return 0;
            }
        }
    }
}
