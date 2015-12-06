using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace MadScienceLab
{
    class SoundEffectPlayer
    {
        private static float masterSetVolume = 0.5f;
        private static float masterVolume = masterSetVolume;
        private static bool masterMuted = false;

        private const int distanceScale = GameConstants.SINGLE_CELL_SIZE;
        private CellObject emitterTarget; //what makes the sound
        private Dictionary<String, SoundEffect> soundReferences = new Dictionary<string,SoundEffect>(); //stores the actual sound effect reference


        private AudioEmitter emitter = new AudioEmitter();
        private static AudioListener listener = new AudioListener();
        private List<SoundEffectInstance> loopedSounds = new List<SoundEffectInstance>(); //keep track of sounds that need to loop

        //stores all possible sounds
        public Dictionary<String, SoundEffectInstance> SoundInstances { get; set; }

        public SoundEffectPlayer(CellObject emitter)
        {
            SoundEffect.DistanceScale = GameConstants.DISTANCE_SCALE;
            SoundInstances = new Dictionary<string, SoundEffectInstance>();
            this.emitterTarget = emitter;
        }

        //Loads a SoundEffect
        public void LoadSound(String name, SoundEffect soundEffect)
        {
            soundReferences.Add(name, soundEffect);
            SoundInstances.Add(name, soundEffect.CreateInstance());
            emitter.Position = emitterTarget.Position / distanceScale;
        }

        //Plays a SoundEffect
        public void PlaySound(String name)
        {
            if (SoundInstances.ContainsKey(name))
            {
                SoundInstances[name].Dispose();
                SoundInstances[name] = soundReferences[name].CreateInstance();
                SoundInstances[name].Volume = masterVolume;
                SoundInstances[name].Apply3D(listener, emitter);
                SoundInstances[name].Play();
            }
        }

        //Plays and loops a sound
        public void PlayAndLoopSound(String name)
        {
            if (SoundInstances.ContainsKey(name))
            {
                SoundInstances[name].IsLooped = true;
                loopedSounds.Add(SoundInstances[name]);
                SoundInstances[name].Volume = masterVolume;
                SoundInstances[name].Apply3D(listener, emitter);
                SoundInstances[name].Play();
            }
        }

        //Changes the pitch of a sound
        public void AdjustPitch(String name, float pitchAmount)
        {
            SoundInstances[name].Pitch = pitchAmount;
        }

        public void Update(RenderContext renderContext)
        {
            listener.Position = renderContext.Player.Position / distanceScale; //always make player the listener
            emitter.Position = emitterTarget.Position / distanceScale;

            //If there are any looped sounds, we need to update their positions so sound moves.
            if (loopedSounds.Count > 0)
            {
                foreach (SoundEffectInstance soundFXInstance in loopedSounds)
                {
                    soundFXInstance.Apply3D(listener, emitter);
                }
            }
        }

        public static void Mute()
        {
            masterMuted = !masterMuted;
            if (masterMuted)
            {
                masterVolume = 0;
            }
            else
            {
                masterVolume = masterSetVolume;
            }
        }
    }
}
