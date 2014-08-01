using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBackLibrary
{
    [Serializable()]
    public class SoundArgs : EventArgs
    {
        private int xPos;
        private int yPos;
        private string fileName;
        private bool loop;
        private float volume;
        private float pitch;
        private float pan;

        public SoundArgs(int xPos, int yPos, string fileName, bool loop = false, float volume = 1f, float pitch = 0f, float pan = 0f)
        {
            this.xPos = xPos;
            this.yPos = yPos;
            this.fileName = fileName;
            this.loop = loop;
            this.volume = volume;
            this.pitch = pitch;
            this.pan = pan;
        }

        public int XPos { get { return xPos; } }
        public int YPos { get { return yPos; } }
        public string FileName { get { return fileName; } }
        public bool Loop { get { return loop; } }
        public float Volume { get { return volume; } }
        public float Pitch { get { return pitch; } }
        public float Pan { get { return pan; } }
       
    }
}
