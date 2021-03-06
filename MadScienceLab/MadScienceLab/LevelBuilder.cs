﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;
using System.IO;

namespace MadScienceLab
{

    public class LevelBuilder
    {
        //MATT- DEBUG STUFF
        static Door open, closed;
        static Button testButton;
        static ToggleSwitch testSwitch;

        public static int levelwidth = 0;
        public const int startWall = 10;
        public const int startFloor = 1;
        //need height in order to determine object Y position, which would be startFloor + ((levelheight-1) - row), levelheight-1 being the highest row
        public static int levelheight; //count \n; (# of "\n")+(1 for floor) is the height. placement; thus, get height first.

        static string levelSelect;

        /// <summary>
        /// Builds a new level.
        /// </summary>
        /// <param name="levelNum"></param>
        /// <param name="renderContext"></param>
        /// <returns></returns>
        public static Level MakeBasicLevel(int levelNum, RenderContext renderContext)
        {
            Level level = new Level();
            levelSelect = "level" + levelNum.ToString();

            //read level from text file - width and height of level will depend on the text file's characters
            /*
             Legend:
             * @ - Exit
             * B - box; 2 - box dropper with 2 boxes (and any other non-zero digit would be a box dropper with said # boxes); 
             * T - toggle switch (can repeatedly use); t - toggle switch (only can use once);
             * S - button switch
             * X - Level wall/block
             * d - door (open by default); D - door (closed by default) 
             * r - trapdoor (open by default); R - trapdoor (closed by default)
             * P - player; G - goal
             * L - laser turret
             *   - empty space
             * m - message event
             * 
             * In order to match trapdoors/doors with switches, can have at the end of the txt file the (row,col) coordinates of each of the paired objects to be linked, in the following format:
             * [row,col]:[row,col];
             * (It's either this, or not have simple characters for the properties ... I think I may prefer this, unless there is a tool that allows easy parsing of XML data or such.)
             * Format for moving platforms:
             * [row,col]:[platform distance]
             */

            //Note: The levels' txt files currently have a new line at the very end of it. Don't delete it.

            string leveltxtfile = FromFile("Levels/" + levelSelect + ".txt");
            string backtxt = FromFile("Levels/" + levelSelect + "Back.txt");

            //get object pairs (for links between switches and doors/boxes)
            // The format used to link buttons to doors "ButtonCoord linked to 1 or more DoorCoord" - Steven

            //differentiate level and link strings
            string leveltxt = null; //everything above the line of "~" in Level.txt
            string linktxt = null; //everything below the line of "~" in Level.txt
                                   //split leveltxtfile into leveltxt and linktxt, with the text enclosed by any number of "~" characters as the delimiter

            int pos = leveltxtfile.IndexOf("~"); //go to the "~" line
            leveltxt = leveltxtfile.Substring(0, pos);
            //put pos on the next line after the last "~" line
            pos = leveltxtfile.LastIndexOf("~"); //go to last ~
            pos = leveltxtfile.IndexOf("\n", pos) + 1; //go to next line, after the last ~
            linktxt = leveltxtfile.Substring(pos);

            Dictionary<string, int> _firstobject = new Dictionary<string, int>();
            Dictionary<string, int> _linkedobjects = new Dictionary<string, int>();

            String newline = "\r\n"; //in case a newline were to be used
            levelheight = leveltxt.Length - leveltxt.Replace("\n", "").Length;
            levelwidth = 0; //reset this each time a new level is made

            CellObject lastXBlock = new CellObject(0, 0);
            CellObject DoorAfterXBlock = new CellObject(0, 0);
            Enemy e = new Enemy();
            bool lookForNextXBlock = false;

            //there will be walls and floor enclosing the aforementioned level
            //iterate through level string, find the level height and width from iterating through the txt file
            int row = startFloor + levelheight - 1, col = startWall;
            foreach (char c in leveltxt)
            {

                switch (c) //convert char to level object at the coordinate iterated through
                {
                    case ' ':
                        col++;
                        break;
                    case 'E':
                        e = new Enemy(col++, row, renderContext);
                        level.AddChild(e);
                        e.wallsToCheck.Add(lastXBlock);
                        lookForNextXBlock = true;
                        level.enemyList.Add(e);
                        break;
                    case 'M':
                        level.AddChild(new MovingPlatform(col++, row));
                        _firstobject.Add("" + row + ":" + (col - startWall), level.Children.Count - 1);
                        break;
                    case 'm':
                        MessageEvent message = new MessageEvent(col++, row, renderContext);
                        level.AddChild(message);
                        level.Messages.Add("" + row + ":" + (col - startWall), message);
                        _firstobject.Add("" + row + ":" + (col - startWall), level.Children.Count - 1);
                        break;
                    case 'B':
                        PickableBox p = new PickableBox(col++, row);
                        level.AddChild(p); //replace BasicBlock with the actual object once implemented
                        break;
                    case 'T': //Toggleable lever switch
                        level.AddChild(new ToggleSwitch(col++, row, true));
                        _firstobject.Add("" + row + ":" + (col - startWall), level.Children.Count - 1); // Adds the coordinates and actual index of the button
                        break;
                    case 't': //One-time lever switch
                        level.AddChild(new ToggleSwitch(col++, row, false));
                        _firstobject.Add("" + row + ":" + (col - startWall), level.Children.Count - 1); // Adds the coordinates and actual index of the button
                        break;
                    case 'S':
                        level.AddChild(new Button(col++, row));
                        _firstobject.Add("" + row + ":" + (col - startWall), level.Children.Count - 1); // Adds the coordinates and actual index of the button
                        break;
                    case 'X':
                        BasicBlock blockToAdd = new BasicBlock(col++, row);
                        level.AddChild(blockToAdd);
                        level.ForegroundBlocks.Add(blockToAdd);
                        lastXBlock = (CellObject)blockToAdd;
                        if (lookForNextXBlock)
                        {
                            lookForNextXBlock = false;
                            e.wallsToCheck.Add(lastXBlock);
                        }
                        break;
                    case 'L':
                        level.AddChild(new LaserTurret(col++, row, true, GameConstants.POINTDIR.pointLeft));
                        _firstobject.Add("" + row + ":" + (col - startWall), level.Children.Count - 1);
                        break;
                    case 'd':
                        level.AddChild(new Door(col++, row, true)); //Starting open door
                        _linkedobjects.Add("" + row + ":" + (col - startWall), level.Children.Count - 1);
                        break;
                    case 'D':
                        //closed = new Door(col++, row, true);
                        level.AddChild(new Door(col++, row, false)); //Starting closed door
                        _linkedobjects.Add("" + row + ":" + (col - startWall), level.Children.Count - 1);
                        break;
                    case 'r':
                        level.AddChild(new Trapdoor(col++, row, true));
                        _firstobject.Add("" + row + ":" + (col - startWall), level.Children.Count - 1);
                        _linkedobjects.Add("" + row + ":" + (col - startWall), level.Children.Count - 1);
                        break;
                    case 'R':
                        level.AddChild(new Trapdoor(col++, row, false));
                        _firstobject.Add("" + row + ":" + (col - startWall), level.Children.Count - 1);
                        _linkedobjects.Add("" + row + ":" + (col - startWall), level.Children.Count - 1);
                        break;
                    case 'P':
                        level.PlayerPoint = new Point(col++, row); //set player position
                        break;
                    case '@':
                        level.AddChild(new ExitBlock(col++, row));
                        break;
                    case 'G':
                        level.AddChild(new BasicBlock(col++, row));
                        break;
                    case '\n': //the '\n' of the new line/row
                        row--;
                        if (col > levelwidth + startWall) //to get max # columns found - check if greater than largest levelwidth found;
                            levelwidth = col - startWall;
                        col = startWall;
                        break;
                }
                if (c >= '1' && c <= '9')
                { //BoxDropper case
                    level.AddChild(new BoxDropper(col++, row, c - '0')/*new BoxDropper(c - '0')*/ );
                    _linkedobjects.Add("" + row + ":" + (col - startWall), level.Children.Count - 1);
                }
            }


            level.collidableObjects = new List<GameObject3D>();
            level.collidableObjects.AddRange(level.Children); // All objects that will be colliding with the player in the same Z axis - Steven
            level.Position = new Vector3(-185, -350, 0);
            level.HitboxWidth = levelwidth * 48;
            level.HitboxHeight = -levelheight * 48 * 2;

            // Interates through the link text to find objects, and: 
            // -their links to other objects (eg. doors, boxdroppers)
            // and/or
            // -other properties set to those objects
            // Steven, Jacob
            if (linktxt.Length != 0)
            {
                string[] buttonLinks = linktxt.Split('\n'); //Each line

                foreach (string links in buttonLinks)
                {
                    string[] ObjectAndSettings = links.Split('|');
                    string[] LinkedObjects = ObjectAndSettings[1].Split('&');
                    int index = _firstobject[ObjectAndSettings[0]];

                    //Parse the links for button and switch
                    if (level.Children[index].GetType() == typeof(Button))
                    {
                        Button button = (Button)level.Children[index];
                        foreach (string door in LinkedObjects)
                        {
                            index = _linkedobjects[door];
                            SwitchableObject doorToAdd = (SwitchableObject)level.Children[index];
                            button.LinkedDoors.Add(doorToAdd);
                        }
                    }
                    else if (level.Children[index].GetType() == typeof(ToggleSwitch))
                    {
                        ToggleSwitch toggleSwitch = (ToggleSwitch)level.Children[index];
                        if (ObjectAndSettings[1].Contains(':')) //check if the settings are regarding coordinates, or the number of possible toggle times
                        {
                            foreach (string door in LinkedObjects) //add each linked object to the doors list
                            {
                                index = _linkedobjects[door];
                                SwitchableObject doorToAdd = (SwitchableObject)level.Children[index];
                                toggleSwitch.LinkedDoors.Add(doorToAdd);

                                //if linked item is BoxDropper, set the toggle switch's number of possible toggles to the number of boxes that the box dropper has, by default
                                if (doorToAdd.GetType() == typeof(BoxDropper))
                                {
                                    toggleSwitch.RemainingToggles = ((BoxDropper)doorToAdd).NumberOfBoxes;
                                }
                            }
                        }
                        else
                        {   //if the settings are regarding toggle times, parse that as toggle times
                            int ToggleTimes = 0;
                            bool isInt = Int32.TryParse(ObjectAndSettings[1], out ToggleTimes);
                            if (isInt)
                            {
                                toggleSwitch.RemainingToggles = ToggleTimes;
                            }
                        }
                    }

                    //Jacob - For moving platform - set moving platform initial direction and distance
                    if (level.Children[index].GetType() == typeof(MovingPlatform))
                    {
                        string[] Settings = ObjectAndSettings[1].Split(',');
                        MovingPlatform movingPlatform = (MovingPlatform)level.Children[index];
                        //set initial direction of moving platform
                        if (Settings[0] == "L")
                        {
                            movingPlatform.movingDirection = GameConstants.DIRECTION.Left;
                        }
                        else if (Settings[0] == "R")
                        {
                            movingPlatform.movingDirection = GameConstants.DIRECTION.Right;
                        }
                        else if (Settings[0] == "U")
                        {
                            movingPlatform.movingDirection = GameConstants.DIRECTION.Up;
                        }
                        else if (Settings[0] == "D")
                        {
                            movingPlatform.movingDirection = GameConstants.DIRECTION.Down;
                        }
                        movingPlatform.maxDistance = Int32.Parse(Settings[1]) * GameConstants.SINGLE_CELL_SIZE;
                    }
                    //Jacob - Set Laser turret direction
                    if (level.Children[index].GetType() == typeof(LaserTurret))
                    {
                        LaserTurret laserTurret = (LaserTurret)level.Children[index];
                        string[] Settings = ObjectAndSettings[1].Split(',');
                        //set initial direction of moving platform
                        if (Settings[0] == "L")
                        {
                            laserTurret.direction = GameConstants.POINTDIR.pointLeft;
                        }
                        else
                            if (Settings[0] == "R")
                        {
                            laserTurret.direction = GameConstants.POINTDIR.pointRight;
                        }
                        //optional 2nd setting - set starting offset of laser turret, in milliseconds.
                        if (Settings.Length >= 2)
                        {
                            laserTurret.elapsedFireTimeOffset = Int32.Parse(Settings[1]);
                        }
                    }

                    //Message event
                    if (level.Children[index].GetType() == typeof(MessageEvent))
                    {
                        level.Messages[ObjectAndSettings[0]].Message = ObjectAndSettings[1];
                    }

                    //Trapdoor
                    if (level.Children[index].GetType() == typeof(Trapdoor))
                    {
                        Trapdoor trapDoor = (Trapdoor)level.Children[index];

                        //Update trapdoors to face the bottom instead
                        if (ObjectAndSettings[1] == "B")
                            trapDoor.faceBottom();
                    }
                }
            }

            //DRAWS BACKGROUND
            row = startFloor + levelheight - 1;
            col = startWall;
            foreach (char c2 in backtxt)
            {
                switch (c2) //convert char to level object at the coordinate iterated through
                {
                    case '1': //White tile
                        level.Background[1].Add(new BackgroundBlock(col++, row));
                        break;
                    case '2': //Metal Vent
                        level.Background[2].Add(new BackgroundBlock(col++, row));
                        break;
                    case '3': //pipe arrangement 1 on bluish gray tile
                        level.Background[3].Add(new BackgroundBlock(col++, row));
                        break;
                    case '4': //pipe arrangement 2 on bluish gray tile
                        level.Background[4].Add(new BackgroundBlock(col++, row));
                        break;
                    case '5': //Black tile
                        level.Background[5].Add(new BackgroundBlock(col++, row));
                        break;
                    case '6': //Black tile
                        level.Background[6].Add(new BackgroundBlock(col++, row));
                        break;
                    case '7': //Black tile
                        level.Background[7].Add(new BackgroundBlock(col++, row));
                        break;
                    case '8': //Black tile
                        level.Background[8].Add(new BackgroundBlock(col++, row));
                        break;
                    case '9': //Black tile
                        level.Background[9].Add(new BackgroundBlock(col++, row));
                        break;
                    case 's': //emcsqaure
                        level.Background[10].Add(new BackgroundBlock(col++, row));
                        break;
                    case 'e': //einstien
                        level.Background[11].Add(new BackgroundBlock(col++, row));
                        break;
                    case 'a': //atom1
                        level.Background[12].Add(new BackgroundBlock(col++, row));
                        break;
                    case 'q': //atom2
                        level.Background[13].Add(new BackgroundBlock(col++, row));
                        break;
                    case 'z': //chalkboard1
                        level.Background[14].Add(new BackgroundBlock(col++, row));
                        break;
                    case 'x': //chalkboard2
                        level.Background[15].Add(new BackgroundBlock(col++, row));
                        break;
                    case 'c': //chalkboard3
                        level.Background[16].Add(new BackgroundBlock(col++, row));
                        break;
                    case 'v': //chalkboard4
                        level.Background[17].Add(new BackgroundBlock(col++, row));
                        break;

                    case '\n': //new line/row
                        row--;
                        col = startWall;
                        break;
                    default: //Regular bluish gray tile
                        level.Background[0].Add(new BackgroundBlock(col++, row));
                        break;
                }
            }

            return level;
        }

        /// <summary>
        /// Retrieves the string from file _and then replaces "\r\n" character pairs with "\n"_.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static String FromFile(String file)
        {
            //credit goes to MikeBMcLBob Taco Industries - https://social.msdn.microsoft.com/forums/windowsapps/en-US/7fcea210-8405-4a38-9459-eb0a361681cc/using-txt-file-in-xna-game?forum=wpdevelop for this.
            String txt = null;
            using (var stream = TitleContainer.OpenStream(file))
            {
                using (var reader = new StreamReader(stream))
                {
                    // Call StreamReader methods like ReadLine, ReadBlock, or ReadToEnd to read in your data, e.g.:  
                    txt = reader.ReadToEnd();
                }
            }
            txt = txt.Replace("\r\n", "\n"); //remove all of the \r newlines - as this wasn't accounted for when we were writing the txt file. Note that the strings _did not_ consider these!
            return txt;
        }
    }
}
