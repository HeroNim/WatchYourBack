using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBackServer
{
    

    enum LevelDimensions
    {
        WIDTH = 32,
        HEIGHT = 18,
        X_SCALE = 40,
        Y_SCALE = 40
    };

    enum LevelName
    {
        FIRST_LEVEL,
        TEST_LEVEL
    };

    /*
     * Holds all the levels in the game, and manages which one should be loaded at any time.
     */

    class LevelSystem : ESystem
    {

        private Dictionary<LevelName, LevelTemplate> levels;
        private LevelName currentLevel;
        private LevelComponent level;
        private bool built;
        private bool pressed;

        public LevelSystem(Dictionary<LevelName, LevelTemplate> levels) : base(false, true, 1)
        {
            components += LevelComponent.bitMask;
            this.levels = levels;
            built = false;
        }

        public void addLevel(LevelName levelName, LevelTemplate level)
        {
            levels.Add(levelName, level);
        }

        private void buildLevel(LevelName levelName)
        {
            
            int player = 1;

            LevelTemplate levelTemplate = levels[levelName];
            int y, x;
            for (y = 0; y < (int)LevelDimensions.HEIGHT; y++)
                for (x = 0; x < (int)LevelDimensions.WIDTH; x++)
                {
                    if (levelTemplate.LevelData[y, x] == TileType.WALL)
                        manager.addEntity(EFactory.createWall(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, 40, 40));
                    if (levelTemplate.LevelData[y, x] == TileType.SPAWN)
                    {
                        manager.addEntity(EFactory.createSpawn(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, 40, 40));
                        manager.addEntity(EFactory.createAvatar(new Rectangle(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, 40, 40), (Allegiance)player, Weapons.THROWN));
                        player++;
                    }
                    
                }
            built = true;
        }

        public override void update(double lastUpdate)
        {
            if (level == null)
                initialize();
            else
            {
                if (currentLevel == level.CurrentLevel)
                {
                    if (!built)
                        buildLevel(currentLevel);
                }
                else
                {
                    clearLevel();
                    currentLevel = level.CurrentLevel;
                    update(lastUpdate);
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.N) && pressed == false)
            {
                level.CurrentLevel++;
                pressed = true;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.N) && pressed == true) 
                pressed = false;

        }

        private void initialize()
        {         
            Entity levelEntity = new Entity();
            levelEntity.addComponent(new LevelComponent());
            manager.addEntity(levelEntity);
            level = (LevelComponent)levelEntity.Components[typeof(LevelComponent)];
            currentLevel = level.CurrentLevel;
        }

        private void clearLevel()
        {
            foreach(Entity entity in manager.ActiveEntities)
            {
                if (entity.hasComponent(Masks.TILE))
                    manager.removeEntity(entity);
            }
            built = false;

        }
    }
}
