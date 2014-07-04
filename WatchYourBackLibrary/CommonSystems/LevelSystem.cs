using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WatchYourBackLibrary;

namespace WatchYourBackLibrary
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

    public class LevelSystem : ESystem
    {

        private List<LevelTemplate> levels;
        private LevelName currentLevel;
        private LevelComponent level;
        private bool built;
        private Texture2D wallTexture;
        private Texture2D spawnTexture;

        public LevelSystem(List<LevelTemplate> levels) : base(false, true, 1)
        {
            components += (int)Masks.LEVEL;
            this.levels = levels;
            built = false;
        }

        public void addLevel(LevelTemplate level)
        {
            levels.Add(level);
        }

        private void buildLevel(LevelName levelName)
        {
            if (manager.hasGraphics())
            {
                wallTexture = manager.getTexture("WallTexture");
                spawnTexture = manager.getTexture("SpawnTexture");
            }
            int player = 1;

            LevelTemplate levelTemplate = levels.Find(o => o.Name == levelName);
            int y, x;
            for (y = 0; y < (int)LevelDimensions.HEIGHT; y++)
                for (x = 0; x < (int)LevelDimensions.WIDTH; x++)
                {
                    if (levelTemplate.LevelData[y, x] == (int)TileType.WALL)
                        manager.addEntity(EFactory.createWall(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, 40, 40, wallTexture, manager.hasGraphics()));
                    if (levelTemplate.LevelData[y, x] == (int)TileType.SPAWN)
                    {
                        manager.addEntity(EFactory.createSpawn(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, 40, 40));
                        manager.addEntity(EFactory.createAvatar(new Rectangle(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, 40, 40),
                             (Allegiance)player, Weapons.THROWN, spawnTexture, manager.hasGraphics()));
                        player++;
                    }
                    
                }
            built = true;
        }

        public override void update(TimeSpan gameTime)
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
                    update(gameTime);
                }
            }

        }

        private void initialize()
        {         
            Entity levelEntity = new Entity();
            levelEntity.addComponent(new LevelComponent());
            manager.addEntity(levelEntity);
            level = (LevelComponent)levelEntity.Components[Masks.LEVEL];
            currentLevel = level.CurrentLevel;
            buildLevel(currentLevel);
        }

        private void clearLevel()
        {
            foreach(Entity entity in manager.ActiveEntities.Values)
            {
                if (entity.hasComponent(Masks.TILE))
                    manager.removeEntity(entity);
            }
            built = false;

        }
    }
}
