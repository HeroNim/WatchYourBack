using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBackServer
{
    

    /*
     * Holds the level data, such as the current level, and whether or not the level has actually started.
     */
    class LevelComponent : EComponent
    {
        public readonly static int bitMask = (int)Masks.LEVEL;
        public override Masks Mask { get { return Masks.LEVEL; } }

        private LevelName currentLevel;

        public LevelComponent()
        {
            currentLevel = LevelName.FIRST_LEVEL;
        }

        public LevelName CurrentLevel
        {
            get { return currentLevel; }
            set { currentLevel = value; }
        }

        
    }
}
