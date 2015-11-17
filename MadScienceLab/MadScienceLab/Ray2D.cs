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
    public struct Ray2D
    {
        private Vector2 startPos;
        private Vector2 endPos;
        private readonly List<Point> result;

        public Ray2D(Vector2 startPos, Vector2 endPos)
        {
            this.startPos = startPos;
            this.endPos = endPos;
            result = new List<Point>();
        }

        /// <summary>  
        /// Determine if the ray intersects the rectangle  
        /// </summary>  
        /// <param name="rectangle">Rectangle to check</param>  
        /// <returns></returns>  
        public Vector2 Intersects(Rectangle rectangle)
        {
            Point p0 = new Point((int)startPos.X, (int)startPos.Y);
            Point p1 = new Point((int)endPos.X, (int)endPos.Y);

            foreach (Point testPoint in BresenhamLineSorted(p0, p1))
            {
                if (rectangle.Contains(testPoint))
                    return new Vector2((float)testPoint.X, (float)testPoint.Y);
            }

            return Vector2.Zero;
        }

        // Swap the values of A and B  

        private void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }


        private List<Point> BresenhamLineSorted(Point startPoint, Point endPoint)
        {
            List<Point> points = BresenhamLine(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);

            bool steep = IsSteep(startPoint, endPoint);

            if (steep)
            {
                points.Sort(new PointSort(PointSort.Mode.Y));

                if (startPoint.Y > endPoint.Y)
                {
                    points.Reverse();
                }
            }
            else
            {
                points.Sort(new PointSort(PointSort.Mode.Y));

                if (startPoint.X > endPoint.X)
                {
                    points.Reverse();
                }
            }

            return points;
        }


        private bool IsSteep(Point startPoint, Point endPoint)
        {
            return IsSteep(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
        }


        private bool IsSteep(int x0, int y0, int x1, int y1)
        {
            return Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
        }


        // Returns the list of points from (x0, y0) to (x1, y1)  
        private List<Point> BresenhamLine(int x0, int y0, int x1, int y1)
        {
            // Optimization: it would be preferable to calculate in  
            // advance the size of "result" and to use a fixed-size array  
            // instead of a list.  

            result.Clear();

            bool steep = IsSteep(x0, y0, x1, y1);
            if (steep)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }

            int deltax = x1 - x0;
            int deltay = Math.Abs(y1 - y0);
            int error = 0;
            int ystep;
            int y = y0;
            if (y0 < y1) ystep = 1; else ystep = -1;
            for (int x = x0; x <= x1; x++)
            {
                if (steep) result.Add(new Point(y, x));
                else result.Add(new Point(x, y));
                error += deltay;
                if (2 * error >= deltax)
                {
                    y += ystep;
                    error -= deltax;
                }
            }

            return result;
        }
    }
    
}
