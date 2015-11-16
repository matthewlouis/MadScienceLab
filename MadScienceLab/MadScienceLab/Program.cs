using System;

namespace MadScienceLab
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            MadLabGame game = new MadLabGame();

            using (game)
            {
                game.Run();
            }
        }
    }
#endif
}

