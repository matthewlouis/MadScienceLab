﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{

    public class LevelBuilder
    {
        //MATT- DEBUG STUFF
        static Door open, closed;
        static Button testButton;

        public static int levelwidth = 0;
        public static int startWall = 10;
        public static int startFloor = 1;
         //need height in order to determine object Y position, which would be startFloor + ((levelheight-1) - row), levelheight-1 being the highest row
        public static int levelheight; //count \n; (# of "\n")+(1 for floor) is the height. placement; thus, get height first.

        //Builds Basic level
        public static Level MakeBasicLevel()
        {
            Level level = new Level();
            
            //read level from text file - width and height of level will depend on the text file's characters
            /*
             Legend:
             * B - box; 2 - box dropper with 2 boxes (and any other non-zero digit would be a box dropper with said # boxes); 
             * T - toggle switch (can repeatedly use); t - toggle switch (only can use once);
             * S - button switch
             * X - Level wall/block
             * d - door (open by default); D - door (closed by default) 
             * r - trapdoor (open by default); R - trapdoor (closed by default)
             * P - player; G - goal
             *   - empty space
             * 
             * In order to match trapdoors/doors with switches, can have at the end of the txt file the (row,col) coordinates of each of the paired objects to be linked, in the following format:
             * [row,col]:[row,col];
             * (It's either this, or not have simple characters for the properties ... I think I may prefer this, unless there is a tool that allows easy parsing of XML data or such.)
             */
            string leveltxt = "XXXXXXXXXXXXXXXXXXXXXX\n"
                            + "XT                   X\n"
                            + "XXXX                 X\n"
                            + "X                    X\n"
                            + "X                   XX\n"
                            + "X       X        X  XX\n"
                            + "X    X          X    X\n"
                            + "XB              X    X\n"
                            + "XXX         B   X    X\n"
                            + "X       B   XXXXXX   X\n"
                            + "X     XXXrrX  B      X\n"
                            + "X PBS d   D   B      X\n"
                            + "XXXXXXXXXXXXXXXXXXXXXX\n";

            string backtxt  = "XXXXXXXXXXXXXXXXXXXXXX\n"
                            + "XT                   X\n"
                            + "XXXX                 X\n"
                            + "X                    X\n"
                            + "X                    X\n"
                            + "X       X        X111X\n"
                            + "X    X          X1111X\n"
                            + "XBS             X1111X\n"
                            + "XXX         B   X1111X\n"
                            + "X       B   XXXXXX111X\n"
                            + "X     XXXrrX111111111X\n"
                            + "X PBS 111111111111111X\n"
                            + "XXXXXXXXXXXXXXXXXXXXXX\n";
            //get object pairs (for links between switches and doors/boxes)

            levelheight = leveltxt.Length - leveltxt.Replace("\n", "").Length;
            
            
            //there will be walls and floor enclosing the aforementioned level
            //iterate through level string, find the level height and width from iterating through the txt file
            int row = startFloor+levelheight-1, col = startWall; 
            foreach (char c in leveltxt)
            {
                switch (c) //convert char to level object at the coordinate iterated through
                {
                    case ' ':
                        col++;
                        break;
                    case 'B':
                        level.AddChild ( new PickableBox ( col++, row ) ); //replace BasicBlock with the actual object once implemented
                        break;
                    case 'T': //Toggleable lever switch
                        level.AddChild ( new Switch ( col++, row, true ) );
                        break;
                    case 't': //One-time lever switch
                        level.AddChild ( new Switch ( col++, row, false ) );
                        break;
                    case 'S':
                        testButton = new Button(col++, row);
                        //level.AddChild ( new Button ( col++, row ) );
                        break;
                    case 'X':
                        level.AddChild ( new BasicBlock ( col++, row ) );
                        break;
                    case 'd':
                        open = new Door(col++, row, true);
                        level.AddChild(open);
                        //level.AddChild ( new Door ( col++, row, true ) ); //Starting open door
                        break;
                    case 'D':
                        closed = new Door(col++, row, true);
                        //level.AddChild ( new Door ( col++, row, false ) ); //Starting closed door
                        break;
                    case 'r':
                        level.AddChild ( new BasicBlock ( col++, row ) );
                        break;
                    case 'R':
                        level.AddChild ( new BasicBlock ( col++, row ) );
                        break;
                    case 'P':
                        level.PlayerPoint = new Point (col++, row); //set player position
                        break;
                    case 'G':
                        level.AddChild ( new BasicBlock ( col++, row ) );
                        break;
                    case '\n': //new line/row
                        row--;
                        if (col > levelwidth + startWall) //to get max # columns found - check if greater than largest levelwidth found;
                            levelwidth = col - startWall;
                        col = startWall;
                        break;
                }
                if (c >= '1' && c <= '9') //BoxDropper case
                    level.AddChild ( new BasicBlock ( col++, row )/*new BoxDropper(c - '0')*/ ); //replace BasicBlock with the actual BoxDropper once implemented
            }

            //Matt- Button debug
            testButton.LinkedDoors.Add(open);
            testButton.LinkedDoors.Add(closed);
            level.AddChild(open);
            level.AddChild(closed);
            level.AddChild(testButton);
            //End debug

            /**DRAWS BACKGROUND**/
            row = startFloor + levelheight - 1;
            col = startWall; 
            foreach (char c2 in backtxt)
            {
                switch (c2) //convert char to level object at the coordinate iterated through
                {
                    case '1': //blue clay
                        level.AddChild(new BackgroundBlock(col++, row, Game1._textures["clay_blue"]));
                        break;
                    case '\n': //new line/row
                        row--;
                        col = startWall;
                        break;
                    default:
                        level.AddChild(new BackgroundBlock(col++, row, Game1._textures["clay_white"]));
                        break;
                }
            }

            return level;
        }
        
    }
}
