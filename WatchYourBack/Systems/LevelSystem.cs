using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WatchYourBackLibrary;

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

        private List<LevelTemplate> levels;
        private LevelName currentLevel;
        private LevelComponent level;
        private bool built;
        private bool pressed;

        public LevelSystem(List<LevelTemplate> levels) : base(false, true, 1)
        {
            components += LevelComponent.bitMask;
            this.levels = levels;
            built = false;
        }

        public void addLevel(LevelTemplate level)
        {
            levels.Add(level);
        }

        private void buildLevel(LevelName levelName)
        {
            Texture2D wallTexture = manager.getTexture("WallTexture");
            Texture2D spawnTexture = manager.getTexture("SpawnTexture");
            int player = 1;

            LevelTemplate levelTemplate = levels.Find(o => o.Name == levelName);
            int y, x;
            for (y = 0; y < (int)LevelDimensions.HEIGHT; y++)
                for (x = 0; x < (int)LevelDimensions.WIDTH; x++)
                {
                    if (levelTemplate.LevelData[y, x] == (int)TileType.WALL)
                        manager.addEntity(EFactory.createWall(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, 40, 40, manager.getTexture("WallTexture")));
                    if (levelTemplate.LevelData[y, x] == (int)TileType.SPAWN)
                    {
                        manager.addEntity(EFactory.createSpawn(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, 40, 40, manager.getTexture("SpawnTexture")));
                        manager.addEntity(EFactory.createAvatar(new Rectangle(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, 40, 40), 
                            spawnTexture, (Allegiance)player, Weapons.THROWN));
                        player++;
                    }
                    
                }
            built = true;
        }

        public override void update(GameTime gameTime)
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
            level = (LevelComponent)levelEntity.Components[Masks.LEVEL];
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
