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

    enum LevelDimensions
    {
        WIDTH = 32,
        HEIGHT = 18,
        X_SCALE = 40,
        Y_SCALE = 40
    };

    /*
     * Holds all the levels in the game, and manages which one should be loaded at any time.
     */

    class LevelManager
    {

        private Dictionary<LevelName, Level> levels;
        private ECSManager manager;
        private EFactory factory;

        public LevelManager(ECSManager manager, EFactory factory)
        {
            levels = new Dictionary<LevelName, Level>();
            this.manager = manager;
            this.factory = factory;
        }

        public void addLevel(LevelName levelName, Level level)
        {
            levels.Add(levelName, level);
        }

        public void buildLevel(LevelName levelName)
        {
            Level level = levels[levelName];
            int y, x;
            for (y = 0; y < (int)LevelDimensions.HEIGHT; y++)
                for (x = 0; x < (int)LevelDimensions.WIDTH; x++)
                {
                    if (level.levelData[y, x].Type == TileType.WALL)
                        manager.addEntity(factory.createWall(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE));
                }
        }
    }
}
