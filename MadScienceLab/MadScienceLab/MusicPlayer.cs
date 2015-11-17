using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace MadScienceLab
{
    public static class MusicPlayer
    {
        static float volume = 1.0f;
        static SoundEffectInstance currentSong;

        //Plays the provided song in a loop.
        public static void PlaySong(SoundEffect song){
            Stop();
            currentSong = song.CreateInstance();
            currentSong.Volume = volume;
            currentSong.IsLooped = true;
            currentSong.Play();
        }


        //Sets volume of player
        public static void SetVolume(float volume)
        {
            MusicPlayer.volume = volume;

            if (currentSong != null && !currentSong.IsDisposed)
                currentSong.Volume = volume;
        }

        //Stops song currently playing
        public static void Stop()
        {
            if (currentSong != null && !currentSong.IsDisposed)
            {
                currentSong.Stop();
            }
        }
    }
}
