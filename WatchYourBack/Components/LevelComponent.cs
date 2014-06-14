using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    enum LevelName
    {
        firstLevel
    };

    class LevelComponent : EComponent
    {
        public readonly static int bitMask = (int)Masks.Level;
        public override int Mask { get { return bitMask; } }

        private LevelName currentLevel;
        private bool playing;

        public LevelComponent()
        {
            currentLevel = LevelName.firstLevel;
            playing = true;
        }

        public LevelName CurrentLevel
        {
            get { return currentLevel; }
            set { currentLevel = value; }
        }

        public bool Playing
        {
            get { return playing; }
            set { playing = value; }
        }
    }
}
