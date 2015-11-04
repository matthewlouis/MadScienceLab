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
            using (MadLabGame game = new MadLabGame())
            {
                game.Run();
            }
        }
    }
#endif
}

