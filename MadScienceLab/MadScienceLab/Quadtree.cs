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
    public class Quadtree
    {
        private int MAX_OBJECTS = 10;
        private int MAX_LEVELS = 5;

        private int level;
        private List<CellObject> objects;
        private Rectangle bounds;
        private Quadtree[] nodes;

        public Quadtree(int pLevel, Rectangle pBounds)
        {
            level = pLevel;
            objects = new List<CellObject>();
            bounds = pBounds;
            nodes = new Quadtree[4];
        }

        /*
        * Clears the quadtree
        */
        public void clear() 
        {
            objects.Clear();

            for (int i = 0; i < nodes.Length; i++) 
            {
                if (nodes[i] != null) {
                    nodes[i].clear();
                    nodes[i] = null;
                }
            }
        }

        /*
        * Splits the node into 4 subnodes
        */
        private void split() 
        {
            int subWidth = (int)(bounds.Width / 2);
            int subHeight = (int)(bounds.Height / 2);
            int x = (int)bounds.X;
            int y = (int)bounds.Y;

            nodes[0] = new Quadtree(level+1, new Rectangle(x + subWidth, y, subWidth, subHeight));
            nodes[1] = new Quadtree(level+1, new Rectangle(x, y, subWidth, subHeight));
            nodes[2] = new Quadtree(level+1, new Rectangle(x, y + subHeight, subWidth, subHeight));
            nodes[3] = new Quadtree(level+1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight));
        }

        /*
        * Determine which node the object belongs to. -1 means
        * object cannot completely fit within a child node and is part
        * of the parent node
        */
        private int getIndex(Rectangle pRect) {
            int index = -1;
            double verticalMidpoint = bounds.X + (bounds.Width / 2);
            double horizontalMidpoint = bounds.Y + (bounds.Height / 2);

            // Object can completely fit within the top quadrants
            bool topQuadrant = (pRect.Y < horizontalMidpoint && pRect.Y + pRect.Height < horizontalMidpoint);
            // Object can completely fit within the bottom quadrants
            bool bottomQuadrant = (pRect.Y > horizontalMidpoint);

            // Object can completely fit within the left quadrants
            if (pRect.X < verticalMidpoint && pRect.X + pRect.Width < verticalMidpoint) {
                if (topQuadrant) {
                    index = 1;
                }
                else if (bottomQuadrant) {
                    index = 2;
                }
            }
            // Object can completely fit within the right quadrants
            else if (pRect.X > verticalMidpoint) {
                if (topQuadrant) {
                    index = 0;
                }
                else if (bottomQuadrant) {
                    index = 3;
                }
            }

            return index;
        }
        /*
        * Insert the object into the quadtree. If the node
        * exceeds the capacity, it will split and add all
        * objects to their corresponding nodes.
        */
        public void insert(CellObject obj) {
            if (nodes[0] != null) {
                int index = getIndex(obj.Hitbox);

                if (index != -1) {
                    nodes[index].insert(obj);

                    return;
                }
            }

            objects.Add(obj);

            if (objects.Count > MAX_OBJECTS && level < MAX_LEVELS) {
                if (nodes[0] == null) { 
                    split(); 
                }
                
                int i = 0;
                while (i < objects.Count) {
                    int index = getIndex(objects[i].Hitbox);
                    if (index != -1) {
                        nodes[index].insert(objects[i]);
                        objects.RemoveAt(i);
                    }
                    else {
                        i++;
                    }
                }
            }
        }

        /*
        * Return all objects that could collide with the given object
        */
        public List<CellObject> retrieve(List<CellObject> returnObjects, Rectangle pRect) {
            int index = getIndex(pRect);
            if (index != -1 && nodes[0] != null) {
                nodes[index].retrieve(returnObjects, pRect);
            }

            returnObjects.AddRange(objects);

            return returnObjects;
        }
    }
}
