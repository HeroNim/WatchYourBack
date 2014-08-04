using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBackLibrary
{
    public class SoundInfo
    {       
        private string sound;
        private float volume;
        private float pitch;
        private float pan;
        private bool looped;

        public SoundInfo(string soundEffectName, bool loop = false, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f)
        {
            sound = soundEffectName;
            this.volume = volume;
            this.pitch = pitch;
            this.pan = pan;
            this.looped = loop;
        }

        public string Sound
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
            get { return looped; }
            set { looped = value; }
        }
    }  
}
