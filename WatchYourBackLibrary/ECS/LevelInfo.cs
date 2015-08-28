using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    public enum Displays
    {
        P1 = 0,
        P2 = 1,
        Score = 2
    }
    
    /// <summary>
    /// The component which holds the level data, such as the current level and the time left in the level.
    /// </summary>
    public class LevelInfo
    {
        private Dictionary<LevelName, LevelTemplate> levels;

        private List<List<Entity>> allEntities;
        private List<Entity> spawns;
        private List<Entity> avatars;
        private List<Vector2> vertices;

        private List<Entity> walls;
        private Timer timer;
        private int timeLeft;

        private bool reset;
        private bool playing;

        private LevelName currentLevel;

        public LevelInfo()
        {
            currentLevel = LevelName.FIRST_LEVEL;
            timeLeft = 60;
            timer = new Timer(1000);
            timer.AutoReset = true;
            timer.Elapsed += Tick;
            playing = true;

            levels = new Dictionary<LevelName, LevelTemplate>();
            vertices = new List<Vector2>();

            spawns = new List<Entity>();
            avatars = new List<Entity>();
            walls = new List<Entity>();
            
            allEntities = new List<List<Entity>>();
            allEntities.Add(spawns);
            allEntities.Add(avatars);
            allEntities.Add(walls);
        }

        public Dictionary<LevelName, LevelTemplate> Levels
        {
            get { return levels; }
            set { levels = value; }
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

        public List<Vector2> Vertices
        {
            get { return vertices; }
            set { vertices = value; }
        }

        public void AddVertex(Vector2 toAdd)
        {
            foreach (Vector2 vertex in vertices)
                if (vertex.Equals(toAdd))
                    return;
            vertices.Add(toAdd);
        }

        public bool Reset
        {
            get { return reset; }
            set { reset = value; }
        }

        public bool Playing
        {
            get { return playing; }
        }

        public List<Entity> Walls
        {
            get { return walls; }
            set { walls = value; }
        }      

        public int GameTime
        {
            get { return timeLeft; }
            set { timeLeft = value; }
        }

        public void Start()
        {
            timer.Start();
        }

        public void ResetLevel()
        {
            walls.Clear();
            avatars.Clear();
            spawns.Clear();
            allEntities.Clear();
        }

        public bool Contains(Entity e)
        {
            foreach (List<Entity> list in allEntities)
                if (list.Contains(e))
                    return true;
            return false;
        }

        private void Tick(object sender, EventArgs e)
        {
            if(timeLeft > 0)
            {
                timeLeft -= 1;
            }
            else
            {
                timer.Stop();
                playing = false;
            }
        }       
    }
}
