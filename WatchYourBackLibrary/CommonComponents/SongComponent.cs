using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Media;

namespace WatchYourBackLibrary
{
    class SongComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.Audio + (int)Masks.Song; } }
        public override Masks Mask { get { return Masks.Song; } }

        private Song song;
        private float volume;        
        private float pitch;       
        private float pan;
        private bool played;
       
        public SongComponent(Song song)
        {
            this.song = song;                       
        }

        public Song Song
        {
            get { return song; }
            set { song = value; }
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

        

        public bool Played
        {
            get { return played; }
            set { played = value; }
        }
    }
}
