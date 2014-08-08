using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Audio;

namespace WatchYourBackLibrary
{
    public enum SoundTriggers
    {
        Initialize,
        Destroy,
        Action
    }

    /// <summary>
    /// The component responsible for audio data
    /// </summary>
    class SoundEffectComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.Audio + (int)Masks.SoundEffect; } }
        public override Masks Mask { get { return Masks.SoundEffect; } }

        private Dictionary<SoundTriggers, SoundInfo> sounds;      
       
        public SoundEffectComponent()
        {
            sounds = new Dictionary<SoundTriggers, SoundInfo>();
        }

        public Dictionary<SoundTriggers, SoundInfo> Sounds
        {
            get { return sounds; }
        }

        public void AddSound(SoundTriggers trigger, string fileName)
        {
            sounds.Add(trigger, new SoundInfo(fileName));
        }
    }
}
