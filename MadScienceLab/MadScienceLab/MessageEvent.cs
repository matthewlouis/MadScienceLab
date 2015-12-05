using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;
using System.Threading;

namespace MadScienceLab
{
    public class MessageEvent : CellObject
    {
       
        int elapsedTypingTime = 0;
        int totalTimeToType = 0;
        int WPM = 600; //default WPM for text
        Vector2 messageBoxPosition = new Vector2(298, 500);
        Rectangle buttonPosition = new Rectangle(959, 662, 48, 48);
        Vector2 textPosition = new Vector2(318, 516);
        public GameConstants.TYPING_STATE typingState;
        
        /// <summary>
        /// The full message displayed by the MessageEvent.
        /// </summary>
        public string Message
        {
            get { return message; }
            set
            {
                //Set default total time to type for the message to correspond to WPM. This is typically done by the LevelBuilder.
                //So, note that each time this is set, this changes the total time to type by a default value!
                int charsPerSec = WPM * 5 / 60;
                int MillisecPerChar = 1000 / charsPerSec;
                totalTimeToType = value.Length * MillisecPerChar;
                message = value;
            }
        }
        string message;

        /// <summary>
        /// The current form of the message that is typed.
        /// </summary>
        /// 
        String typedMessage;
        List<String> typedMessageLines;

        //Store state
        KeyboardState lastKey;
        GamePadState lastButton;


        public MessageEvent(int column, int row, RenderContext renderContext) : base(column, row)
        {
            base.isCollidable = true;
            base.IsPassable = true;
            this.typingState = GameConstants.TYPING_STATE.NotTyped;
            Scale(48f, 48f, 48f);
            //Position = new Vector3(Position.X, Position.Y - 2, Position.Z - 27); //not sure about this position
            //HitboxHeightOffset = 2;
            HitboxHeight = GameConstants.SINGLE_CELL_SIZE;
            HitboxWidth = GameConstants.SINGLE_CELL_SIZE;
            //Translate(new Vector3(0, 0, 0));

            typedMessageLines = new List<String>(4);
            typedMessage = "";
        }
        /*
         * make length of text shown (as a substring) proportional to the (elapsed time) / (total timelength of text).
         * Can make eg. as default, each char amounting to maybe ~50ms more time, or something => 20 chars per second.
         * Maybe 33 chars per second - for 400 WPM.
         */

        /// <summary>
        /// Start typing the message.
        /// </summary>
        public void StartTyping()
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
        public void FinishTyping()
        {
            typingState = GameConstants.TYPING_STATE.DoneTyping;
        }

        /// <summary>
        /// Method to set the typing speed to correspond to the WPM, if necessary.
        /// Alternatively, can set totalTimeToType in order to set the amount of time to type.
        /// </summary>
        /// <param name="WPM"></param>
        public void SetTypingSpeed(int WPM)
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
        public override void Update(RenderContext renderContext)
        {
            if (this.typingState == GameConstants.TYPING_STATE.Disabled || this.typingState == GameConstants.TYPING_STATE.NotTyped) //Completely cease update upon being disabled.
            {
                return;
            }

            //Action button leads to closing the screen, if typing is all displayed.
            if (this.typingState == GameConstants.TYPING_STATE.DoneTyping
                && ((Keyboard.GetState().IsKeyDown(Keys.F) && lastKey.IsKeyUp(Keys.F)) || 
                    (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.B) && lastButton.IsButtonUp(Buttons.B))))
            {
                this.typingState = GameConstants.TYPING_STATE.Disabled;
                GameplayScreen.messageActive = false;
            }

            //Action button leads to finishing typing, if currently typing.
            else if(this.typingState == GameConstants.TYPING_STATE.Typing
                 && ((Keyboard.GetState().IsKeyDown(Keys.F) && lastKey.IsKeyUp(Keys.F)) || 
                    (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.B) && lastButton.IsButtonUp(Buttons.B))))
            {
                FinishTyping();
            }

            //When typing, have the typed text update, as if it were a typewriter.
            if (typingState == GameConstants.TYPING_STATE.Typing)
            {
                //Update the typed text until it's done.
                if (elapsedTypingTime < totalTimeToType) 
                {
                    elapsedTypingTime += renderContext.GameTime.ElapsedGameTime.Milliseconds;
                    typedMessage = Message.Substring(0, (int)(Message.Length * elapsedTypingTime / totalTimeToType)); //Get current message to be typed for the current point
                    
                }
                else //done
                {
                    FinishTyping();
                }
            }
            else if (typingState == GameConstants.TYPING_STATE.DoneTyping) //Use the complete message.
            {
                typedMessage = Message;
            }
            typedMessageLines = GetLines(typedMessage, renderContext, renderContext.Textures["MessageBackground"].Width - 30); //Wrap text according to the width of the message box.

            //Update the previous button and key state.
            lastKey = Keyboard.GetState();
            lastButton = GamePad.GetState(PlayerIndex.One);

            base.Update(renderContext);
        }

        public override void Draw(RenderContext renderContext)
        {
            //Do nothing - don't call base
        }

        /// <summary>
        /// Jacob: This breaks the text into separate lines, depending on the specified max width for the text string
        /// And that is returned.
        /// </summary>
        /// <returns></returns>
        public virtual List<String> GetLines(string text, RenderContext renderContext, int width)
        {
            float TextWidth = 0;
            SpriteFont font = renderContext.MessageFont;
            string trimmableText = text;
            List<String> lines = new List<String>();

            while (trimmableText.Length >= 1)
            {
                int c = 1;
                while (c < trimmableText.Length &&
                    font.MeasureString(trimmableText.Substring(0, ((trimmableText.IndexOf(" ", c + 1) != -1) ? trimmableText.IndexOf(" ", c + 1) : trimmableText.Length))).X < width) //so ends at trimmableText.Length or when the width when adding another char would exceed the width
                {
                    //Set to the next space or the end of the string if that doesn't exist.
                    c = ((trimmableText.IndexOf(" ", c + 1) != -1) ? trimmableText.IndexOf(" ", c + 1) : trimmableText.Length);
                }
                String linestring = trimmableText.Substring(0, c).TrimStart(' ');
                lines.Add(linestring);
                trimmableText = trimmableText.Substring(c);

                //Code to get max TextWidth (used for centering the text)
                if (LineWidth(renderContext, linestring) > TextWidth)
                    TextWidth = font.MeasureString(linestring).X;
            }

            return lines;
        }

        /// <summary>
        /// Gets the width of the string, given the font and its respective size (in screen).
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        protected virtual float LineWidth(RenderContext renderContext, String line)
        {
            SpriteFont font = renderContext.MessageFont;
            return font.MeasureString(line).X;
        }

        public void DisplayMessage(RenderContext renderContext)
        {
            if (typingState == GameConstants.TYPING_STATE.Typing)
            {
                DrawMessage(renderContext);
            }
            else if (typingState == GameConstants.TYPING_STATE.DoneTyping)
            {
                DrawMessage(renderContext);
            }

            renderContext.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            renderContext.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }

        private void DrawMessage(RenderContext renderContext)
        {
            
            if (this.typingState != GameConstants.TYPING_STATE.Disabled)
            {
                // trying to get the game to pause while message is displaying and then unpause and remove message
                if (!GameplayScreen.messageActive)
                {
                    GameplayScreen.messageActive = true;
                    GameplayScreen.messageObj = this;

                }

                // draw the message to screen
                renderContext.SpriteBatch.Begin();
                renderContext.SpriteBatch.Draw(renderContext.Textures["MessageBackground"], messageBoxPosition, Color.White);
                renderContext.SpriteBatch.Draw(renderContext.Textures["B_Button"], buttonPosition, Color.White);
                for (int line = 0; line < typedMessageLines.Count && line < 4; line++)
                {
                    renderContext.SpriteBatch.DrawString(renderContext.MessageFont, typedMessageLines[line], textPosition + new Vector2(0, 40*line), Color.White);
                }
                renderContext.SpriteBatch.End();
            }
        }

    }

   
}
