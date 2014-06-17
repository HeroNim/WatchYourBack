using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    enum LevelName
    {
        FIRST_LEVEL
    };

    /*
     * Holds the level data, such as the current level, and whether or not the level has actually started.
     */
    class LevelComponent : EComponent
    {
        public readonly static int bitMask = (int)Masks.LEVEL;
        public override int Mask { get { return bitMask; } }

        private LevelName currentLevel;
        private bool playing;

        public LevelComponent()
        {
            currentLevel = LevelName.FIRST_LEVEL;
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
