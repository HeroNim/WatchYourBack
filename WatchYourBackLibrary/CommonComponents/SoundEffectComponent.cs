using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Audio;

namespace WatchYourBackLibrary
{
    /// <summary>
    /// The component responsible for audio data
    /// </summary>
    class SoundEffectComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.Audio + (int)Masks.SoundEffect; } }
        public override Masks Mask { get { return Masks.SoundEffect; } }

        private SoundEffectInstance sound;
        private float volume;        
        private float pitch;       
        private float pan;
        private bool played;
       
        public SoundEffectComponent(SoundEffect soundEffect, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f, bool loop = false)
        {
            sound = soundEffect.CreateInstance();
            this.volume = volume;
            this.pitch = pitch;
            this.pan = pan;
            sound.IsLooped = loop;
            played = false;
        }

        public SoundEffectInstance Sound
        {
            get { return sound; }
            set { sound = value; }
        }

        public float Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        public float Pitch
        {
            get { return pitch; }
            set { pitch = value; }
        }

        public float Pan
        {
            get { return pan; }
            set { pan = value; }
        }

        public bool Loop
        {
            get { return sound.IsLooped; }
            set { sound.IsLooped = value; }
        }

        public bool Played
        {
            get { return played; }
            set { played = value; }
        }
    }
}
