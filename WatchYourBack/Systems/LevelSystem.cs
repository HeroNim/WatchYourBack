using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    

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

    class LevelSystem : ESystem
    {

        private Dictionary<LevelName, LevelTemplate> levels;
        private LevelComponent level;
        private bool built;

        public LevelSystem(Dictionary<LevelName, LevelTemplate> levels) : base(false, true)
        {
            components += LevelComponent.bitMask;
            this.levels = levels;
            built = false;
        }

        public void addLevel(LevelName levelName, LevelTemplate level)
        {
            levels.Add(levelName, level);
        }

        public void buildLevel(LevelName levelName)
        {
            LevelTemplate level = levels[levelName];
            int y, x;
            for (y = 0; y < (int)LevelDimensions.HEIGHT; y++)
                for (x = 0; x < (int)LevelDimensions.WIDTH; x++)
                {
                    if (level.LevelData[y, x] == TileType.WALL)
                        manager.addEntity(manager.Factory.createWall(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE));
                }
            built = true;
        }

        public override void update()
        {
            if (level == null){
                foreach (Entity entity in activeEntities)
                    if (entity.hasComponent(LevelComponent.bitMask))
                        level = (LevelComponent)entity.Components[typeof(LevelComponent)];
            }
            else
            {
                if(level.Playing)
                    if (!built)
                        buildLevel(level.CurrentLevel);
            }

        }
    }
}
