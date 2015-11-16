using System;
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
        public static int startWall = 10;
        public static int startFloor = 1;
         //need height in order to determine object Y position, which would be startFloor + ((levelheight-1) - row), levelheight-1 being the highest row
        public static int levelheight; //count \n; (# of "\n")+(1 for floor) is the height. placement; thus, get height first.

        static string levelSelect;

        //Builds Basic level
        public static Level MakeBasicLevel(int levelNum)
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
            
            string leveltxtfile = FromFile ("Levels/" + levelSelect + ".txt" );
            string backtxt = FromFile( "Levels/" + levelSelect + "Back.txt" );

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
            pos = leveltxtfile.IndexOf ( "\n", pos ) + 1; //go to next line, after the last ~
            linktxt = leveltxtfile.Substring ( pos );

            Dictionary<string, int> _firstobject = new Dictionary<string, int>();
            Dictionary<string, int> _linkedobjects = new Dictionary<string, int>();

            String newline = "\r\n"; //in case a newline were to be used
            levelheight = leveltxt.Length - leveltxt.Replace ("\n", "" ).Length;
            
            
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
                    case 'E':
                        level.AddChild(new Enemy(col++, row)); 
                        break;
                    case 'M':
                        level.AddChild(new MovingPlatform(col++, row));
                        _firstobject.Add("" + row + ":" + (col - startWall), level.Children.Count - 1);
                        break;
                    case 'm':
                        level.AddChild(new MessageEvent(col++, row));
                        _firstobject.Add("" + row + ":" + (col - startWall), level.Children.Count - 1);
                        break;
                    case 'B':
                        level.AddChild ( new PickableBox ( col++, row ) ); //replace BasicBlock with the actual object once implemented
                        break;
                    case 'T': //Toggleable lever switch
                        level.AddChild ( new ToggleSwitch ( col++, row, true ) );
                        _firstobject.Add("" + row + ":" + (col - startWall), level.Children.Count - 1); // Adds the coordinates and actual index of the button
                        break;
                    case 't': //One-time lever switch
                        level.AddChild ( new ToggleSwitch ( col++, row, false ) );
                        _firstobject.Add("" + row + ":" + (col - startWall), level.Children.Count - 1); // Adds the coordinates and actual index of the button
                        break;
                    case 'S':
                        level.AddChild ( new Button ( col++, row ) );
                        _firstobject.Add("" + row + ":" + (col - startWall), level.Children.Count - 1); // Adds the coordinates and actual index of the button
                        break;
                    case 'X':
                        level.AddChild ( new BasicBlock ( col++, row ) );
                        break;
                    case 'L':
                       level.AddChild(new LaserTurret(col++, row, true, GameConstants.DIRECTION.pointLeft)); 
                        _firstobject.Add("" + row + ":" + (col - startWall), level.Children.Count - 1);                      
                        break;
                    case 'd':
                        level.AddChild ( new Door ( col++, row, true ) ); //Starting open door
                        _linkedobjects.Add("" + row + ":" + (col - startWall), level.Children.Count - 1);
                        break;
                    case 'D':
                        //closed = new Door(col++, row, true);
                        level.AddChild ( new Door ( col++, row, false ) ); //Starting closed door
                        _linkedobjects.Add("" + row + ":" + (col - startWall), level.Children.Count - 1);
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
                    case '@':
                        level.AddChild(new ExitBlock(col++, row));
                        break;
                    case 'G':
                        level.AddChild ( new BasicBlock ( col++, row ) );
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
                    }else if(level.Children[index].GetType() == typeof(ToggleSwitch))
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
                        if(Settings[0] == "L") {
                            movingPlatform.movingLeft = true;
                        }
                        else
                            if (Settings[0] == "R")
                        {
                            movingPlatform.movingLeft = false;
                        }
                        movingPlatform.maxDistance = Int32.Parse(Settings[1]) * GameConstants.SINGLE_CELL_SIZE;
                    }
                    //Jacob - Set Laser turret direction
                    if (level.Children[index].GetType () == typeof ( LaserTurret ))
                    {
                        LaserTurret laserTurret = (LaserTurret)level.Children[index];
                        //set initial direction of moving platform
                        if (ObjectAndSettings[1] == "L")
                        {
                            laserTurret.direction = GameConstants.DIRECTION.pointLeft;
                        }
                        else
                            if (ObjectAndSettings[1] == "R")
                            {
                                laserTurret.direction = GameConstants.DIRECTION.pointRight;
                            }
                    }


                    //Message event
                    if (level.Children[index].GetType() == typeof(MessageEvent))
                    {
                        MessageEvent messageEvent = (MessageEvent)level.Children[index];
                        messageEvent.Message = ObjectAndSettings[1];
                    }
                }
            }
            
            /**DRAWS BACKGROUND**/
            row = startFloor + levelheight - 1;
            col = startWall; 
            foreach (char c2 in backtxt)
            {
                switch (c2) //convert char to level object at the coordinate iterated through
                {
                    case '1': //Bare Metal Gray
                        level.AddChild(new BackgroundBlock(col++, row, GameplayScreen._textures["BareMetal_Gray"]));
                        break;
                    case '2': //Rounded Brushed Gray Metal
                        level.AddChild(new BackgroundBlock(col++, row, GameplayScreen._textures["BrushedRoundMetal_Gray"]));
                        break;
                    case '3': //Textured Metal Floor Gray
                        level.AddChild(new BackgroundBlock(col++, row, GameplayScreen._textures["MetalFloor_Gray"]));
                        break;
                    case '4': //Dirty Rusted Metal
                        level.AddChild(new BackgroundBlock(col++, row, GameplayScreen._textures["DirtyMetal"]));
                        break;
                    case '5': //White Fiberglass
                        level.AddChild(new BackgroundBlock(col++, row, GameplayScreen._textures["Fiberglass_White"]));
                        break;
                    case '6': //Tile Blue
                        level.AddChild(new BackgroundBlock(col++, row, GameplayScreen._textures["Tile_Blue"]));
                        break;
                    case '7': //Tile Beige
                        level.AddChild(new BackgroundBlock(col++, row, GameplayScreen._textures["Tile_Beige"]));
                        break;
                    case '8': //Tile Multicolored
                        level.AddChild(new BackgroundBlock(col++, row, GameplayScreen._textures["Tile_Fun"]));
                        break;
                    case '9': //Tile Gray
                        level.AddChild(new BackgroundBlock(col++, row, GameplayScreen._textures["Tile_DarkGray"]));
                        break;
                    case '0': //Windowed Glass Blocks
                        level.AddChild(new BackgroundBlock(col++, row, GameplayScreen._textures["WindowBlocks"]));
                        break;
                    case '@':
                        col++;
                        break;
                    case '\n': //new line/row
                        row--;
                        col = startWall;
                        break;
                    default:
                        level.AddChild(new BackgroundBlock(col++, row, GameplayScreen._textures["Tile_Gray"]));
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
        private static String FromFile ( String file )
        {
            //credit goes to MikeBMcLBob Taco Industries - https://social.msdn.microsoft.com/forums/windowsapps/en-US/7fcea210-8405-4a38-9459-eb0a361681cc/using-txt-file-in-xna-game?forum=wpdevelop for this.
            String txt = null;
            using (var stream = TitleContainer.OpenStream ( file ))
            {
                using (var reader = new StreamReader ( stream ))
                {
                    // Call StreamReader methods like ReadLine, ReadBlock, or ReadToEnd to read in your data, e.g.:  
                    txt = reader.ReadToEnd ();
                }
            }
            txt = txt.Replace ( "\r\n", "\n" ); //remove all of the \r newlines - as this wasn't accounted for when we were writing the txt file. Note that the strings _did not_ consider these!
            return txt;
        }
    }
}
