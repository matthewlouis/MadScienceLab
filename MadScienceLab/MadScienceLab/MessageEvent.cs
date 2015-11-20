using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    public class MessageEvent : CellObject
    {
        
        int elapsedTypingTime = 0;
        int totalTimeToType = 0;
        int WPM = 400; //default WPM for text
        public GameConstants.TYPING_STATE typingState;
        public string Message
        {
            get { return message; }
            set
            {
            //Set default total time to type for the message to correspond to WPM. This is typically done by the LevelBuilder.
            //So, note that each time this is set, this changes the total time to type by a default value!
            int charsPerSec = WPM * 5/60;
            int MillisecPerChar = 1000 / charsPerSec;
            totalTimeToType = value.Length * MillisecPerChar;
            message = value;
        } }
        string message;

        /// <summary>
        /// The current form of the message that is typed.
        /// </summary>
        public string typedMessage {get; private set;}
        public MessageEvent(int column, int row) : base(column, row)
        {
            base.isCollidable = true;
            base.IsPassable = true;
            base.Model = GameplayScreen._models["ExitBlock"];
            this.typingState = GameConstants.TYPING_STATE.NotTyped;
            Scale(48f, 48f, 48f);
            //Position = new Vector3(Position.X, Position.Y - 2, Position.Z - 27); //not sure about this position
            //HitboxHeightOffset = 2;
            HitboxHeight = GameConstants.SINGLE_CELL_SIZE;
            HitboxWidth = GameConstants.SINGLE_CELL_SIZE;
<<<<<<< HEAD
=======
            isCollidable = true;
            IsPassable = true;
>>>>>>> refs/remotes/origin/master
        }
        /*
         make length of text shown (as a substring) proportional to the (elapsed time) / (total timelength of text).
         * Can make eg. as default, each char amounting to maybe ~50ms more time, or something => 20 chars per second.
         * Maybe 33 chars per second - for 400 WPM.
         */

        /// <summary>
        /// Start typing the message.
        /// </summary>
        public void StartTyping ()
        {
            typingState = GameConstants.TYPING_STATE.Typing;
            //resets elapsedTypingTime to 0 if necessary.
            elapsedTypingTime = 0;
            //could also reset totalTimeToType to what would correspond to the WPM
        }

        /// <summary>
        /// Used to eg. just finish displaying the text immediately.
        /// For example, if the player presses a button to finish displaying the text (before it were done typing).
        /// If such were the case, note that elapsedTypingTime would still be < totalTypingTime.
        /// </summary>
        public void FinishTyping ()
        {
            typedMessage = Message;
            typingState = GameConstants.TYPING_STATE.DoneTyping;
        }

        /// <summary>
        /// Method to set the typing speed to correspond to the WPM, if necessary.
        /// Alternatively, can set totalTimeToType in order to set the amount of time to type.
        /// </summary>
        /// <param name="WPM"></param>
        public void SetTypingSpeed ( int WPM )
        {
            this.WPM = WPM;
            int charsPerSec = WPM * 5 / 60;
            int MillisecPerChar = 1000 / charsPerSec;
            totalTimeToType = Message.Length * MillisecPerChar;
        }

        /// <summary>
        /// Update code for the MessageEvent. Handles typing internally; it's just a matter of incrementing the state in order to start it.
        /// </summary>
        /// <param name="renderContext"></param>
        public override void Update ( RenderContext renderContext )
        {
            if (typingState == GameConstants.TYPING_STATE.Typing)
            {
                if (elapsedTypingTime < totalTimeToType) //until it's done
                {
                    elapsedTypingTime += renderContext.GameTime.ElapsedGameTime.Milliseconds;
                    typedMessage = Message.Substring ( 0, (int)(Message.Length * elapsedTypingTime / totalTimeToType) ); //Get current message to be typed for the current point
                }
                else //done
                {
                    FinishTyping ();
                }
            }
            //handle typing ...

            base.Update ( renderContext );
        }

    }
}
