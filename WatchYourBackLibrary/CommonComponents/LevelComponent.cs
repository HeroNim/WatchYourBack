using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchYourBackLibrary;

namespace WatchYourBackLibrary
{
    

    /*
     * Holds the level data, such as the current level, and whether or not the level has actually started.
     */
    public class LevelComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.LEVEL; } }
        public override Masks Mask { get { return Masks.LEVEL; } }

        private List<Entity> spawns;
        private List<Entity> avatars;
        //private List<Entity> walls;

        private bool reset;

        private LevelName currentLevel;

        public LevelComponent()
        {
            currentLevel = LevelName.FIRST_LEVEL;
            spawns = new List<Entity>();
            avatars = new List<Entity>();
        }

        public LevelName CurrentLevel
        {
            get { return currentLevel; }
            set { currentLevel = value; }
        }

        public List<Entity> Spawns
        {
            get { return spawns; }
            set { spawns = value; }
        }

        public List<Entity> Avatars
        {
            get { return avatars; }
            set { avatars = value; }
        }

        public bool Reset
        {
            get { return reset; }
            set { reset = value; }
        }

        //public List<Entity> Walls
        //{
        //    get { return walls; }
        //    set { walls = value; }
        //}

        
    }
}
