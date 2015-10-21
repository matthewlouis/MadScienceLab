using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{

    public class LevelBuilder
    {
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
             * @ - Exit
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

            string leveltxt = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXX\n"
                            + "X                           X\n"
                            + "X                           X\n"
                            + "X                2          X\n"
                            + "X                        X  X\n"
                            + "X       X                X  X\n"
                            + "X               S  S   T X  X\n"
                            + "X            XXXXXXXXXXXXX  X\n"
                            + "X            D  D  D     D @X\n"
                            + "X4        XXXXXXXXXXXX   XXXX\n"
                            + "X         XXX           XXXXX\n"
                            + "XP   T    XXXT         XXXXXX\n"
                            + "XXXXXXXXXXXXXXXXXXXXXXXXXXXXX\n";

            string backtxt =  "XXXXXXXXXXXXXXXXXXXXXXXXXXXXX\n"
                            + "X                           X\n"
                            + "X                           X\n"
                            + "X                B  B       X\n"
                            + "X                        X  X\n"
                            + "X       X                X  X\n"
                            + "X               S  S   T X  X\n"
                            + "X            XXXXXXXXXXXXX  X\n"
                            + "X BBB        D TD  D     D @X\n"
                            + "XB        XXXXXXXXXXXX   XXXX\n"
                            + "X         XXX           XXXXX\n"
                            + "XP        XXXT         XXXXXX\n"
                            + "XXXXXXXXXXXXXXXXXXXXXXXXXXXXX\n";

            //get object pairs (for links between switches and doors/boxes)
            // The format used to link buttons to doors "ButtonCoord linked to 1 or more DoorCoord" - Steven
            string linktxt = "7:20|5:17\n"
                           + "2:14|5:26\n"
                           + "7:17|5:14&5:20\n"
                           + "2:6|4:2\n"
                           + "7:24|10:18";
            Dictionary<string, int> _buttons = new Dictionary<string, int>();
            Dictionary<string, int> _doors = new Dictionary<string, int>();

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
                    case 'E':
                        level.AddChild(new Enemy(col++, row)); 
                        break;
                    case 'B':
                        level.AddChild ( new PickableBox ( col++, row ) ); //replace BasicBlock with the actual object once implemented
                        break;
                    case 'T': //Toggleable lever switch
                        level.AddChild ( new ToggleSwitch ( col++, row, true ) );
                        _buttons.Add("" + row + ":" + (col - startWall), level.Children.Count - 1); // Adds the coordinates and actual index of the button
                        break;
                    case 't': //One-time lever switch
                        level.AddChild ( new ToggleSwitch ( col++, row, false ) );
                        _buttons.Add("" + row + ":" + (col - startWall), level.Children.Count - 1); // Adds the coordinates and actual index of the button
                        break;
                    case 'S':
                        level.AddChild ( new Button ( col++, row ) );
                        _buttons.Add("" + row + ":" + (col - startWall), level.Children.Count - 1); // Adds the coordinates and actual index of the button
                        break;
                    case 'X':
                        level.AddChild ( new BasicBlock ( col++, row ) );
                        break;
                    case 'L':
                       level.AddChild(new LaserTurret(col++, row, true));
                        break;
                    case 'd':
                        level.AddChild ( new Door ( col++, row, true ) ); //Starting open door
                        _doors.Add("" + row + ":" + (col - startWall), level.Children.Count - 1);
                        break;
                    case 'D':
                        //closed = new Door(col++, row, true);
                        level.AddChild ( new Door ( col++, row, false ) ); //Starting closed door
                        _doors.Add("" + row + ":" + (col - startWall), level.Children.Count - 1);
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
                    case '\n': //new line/row
                        row--;
                        if (col > levelwidth + startWall) //to get max # columns found - check if greater than largest levelwidth found;
                            levelwidth = col - startWall;
                        col = startWall;
                        break;
                }
                if (c >= '1' && c <= '9')
                { //BoxDropper case
                    level.AddChild(new BoxDropper(col++, row, c - '0')/*new BoxDropper(c - '0')*/ );
                    _doors.Add("" + row + ":" + (col - startWall), level.Children.Count - 1);
                }
            }

            // Interates through the link text to find buttons and their links to the doors - Steven
            if (linktxt.Length != 0)
            {
                string[] buttonLinks = linktxt.Split('\n');

                foreach (string links in buttonLinks)
                {
                    string[] buttonAndDoors = links.Split('|');
                    string[] doors = buttonAndDoors[1].Split('&');
                    int index = _buttons[buttonAndDoors[0]];

                    if (level.Children[index].GetType() == typeof(Button))
                    {
                        Button button = (Button)level.Children[index];
                        foreach (string door in doors)
                        {
                            index = _doors[door];
                            SwitchableObject doorToAdd = (SwitchableObject)level.Children[index];
                            button.LinkedDoors.Add(doorToAdd);
                        }
                    }else if(level.Children[index].GetType() == typeof(ToggleSwitch))
                    {
                        ToggleSwitch toggleSwitch = (ToggleSwitch)level.Children[index];
                        foreach (string door in doors)
                        {
                            index = _doors[door];
                            SwitchableObject doorToAdd = (SwitchableObject)level.Children[index];
                            toggleSwitch.LinkedDoors.Add(doorToAdd);
                        }
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
                        level.AddChild(new BackgroundBlock(col++, row, Game1._textures["BareMetal_Gray"]));
                        break;
                    case '2': //Rounded Brushed Gray Metal
                        level.AddChild(new BackgroundBlock(col++, row, Game1._textures["BrushedRoundMetal_Gray"]));
                        break;
                    case '3': //Textured Metal Floor Gray
                        level.AddChild(new BackgroundBlock(col++, row, Game1._textures["MetalFloor_Gray"]));
                        break;
                    case '4': //Dirty Rusted Metal
                        level.AddChild(new BackgroundBlock(col++, row, Game1._textures["DirtyMetal"]));
                        break;
                    case '5': //White Fiberglass
                        level.AddChild(new BackgroundBlock(col++, row, Game1._textures["Fiberglass_White"]));
                        break;
                    case '6': //Tile Blue
                        level.AddChild(new BackgroundBlock(col++, row, Game1._textures["Tile_Blue"]));
                        break;
                    case '7': //Tile Beige
                        level.AddChild(new BackgroundBlock(col++, row, Game1._textures["Tile_Beige"]));
                        break;
                    case '8': //Tile Multicolored
                        level.AddChild(new BackgroundBlock(col++, row, Game1._textures["Tile_Fun"]));
                        break;
                    case '9': //Tile Gray
                        level.AddChild(new BackgroundBlock(col++, row, Game1._textures["Tile_DarkGray"]));
                        break;
                    case '0': //Windowed Glass Blocks
                        level.AddChild(new BackgroundBlock(col++, row, Game1._textures["WindowBlocks"]));
                        break;
                    case '@':
                        col++;
                        break;
                    case '\n': //new line/row
                        row--;
                        col = startWall;
                        break;
                    default:
                        level.AddChild(new BackgroundBlock(col++, row, Game1._textures["Tile_Gray"]));
                        break;
                }
            }

            return level;
        }
    }
}
