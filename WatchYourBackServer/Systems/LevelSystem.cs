using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

<<<<<<< HEAD
using WatchYourBackLibrary;

=======
>>>>>>> origin/Networking
namespace WatchYourBackServer
{
    

    enum LevelDimensions
    {
        WIDTH = 32,
        HEIGHT = 18,
        X_SCALE = 40,
        Y_SCALE = 40
    };

<<<<<<< HEAD
    
=======
    enum LevelName
    {
        FIRST_LEVEL,
        TEST_LEVEL
    };
>>>>>>> origin/Networking

    /*
     * Holds all the levels in the game, and manages which one should be loaded at any time.
     */

    class LevelSystem : ESystem
    {

<<<<<<< HEAD
        private List<LevelTemplate> levels;
=======
        private Dictionary<LevelName, LevelTemplate> levels;
>>>>>>> origin/Networking
        private LevelName currentLevel;
        private LevelComponent level;
        private bool built;
        private bool pressed;

<<<<<<< HEAD
        public LevelSystem(List<LevelTemplate> levels) : base(false, true, 1)
=======
        public LevelSystem(Dictionary<LevelName, LevelTemplate> levels) : base(false, true, 1)
>>>>>>> origin/Networking
        {
            components += LevelComponent.bitMask;
            this.levels = levels;
            built = false;
        }

<<<<<<< HEAD
        public void addLevel(LevelTemplate level)
        {
            levels.Add(level);
=======
        public void addLevel(LevelName levelName, LevelTemplate level)
        {
            levels.Add(levelName, level);
>>>>>>> origin/Networking
        }

        private void buildLevel(LevelName levelName)
        {
<<<<<<< HEAD
            int player = 0;
            LevelTemplate levelTemplate = levels.Find(o => o.Name == levelName);
=======
            
            int player = 1;

            LevelTemplate levelTemplate = levels[levelName];
>>>>>>> origin/Networking
            int y, x;
            for (y = 0; y < (int)LevelDimensions.HEIGHT; y++)
                for (x = 0; x < (int)LevelDimensions.WIDTH; x++)
                {
<<<<<<< HEAD
                    if (levelTemplate.LevelData[y, x] == (int)TileType.WALL)
                        manager.addEntity(ServerEFactory.createWall(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, 40, 40));
                    if (levelTemplate.LevelData[y, x] == (int)TileType.SPAWN)
                    {
                        manager.addEntity(ServerEFactory.createSpawn(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, 40, 40));
                        manager.addEntity(ServerEFactory.createAvatar(new Rectangle(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, 40, 40), (Allegiance)player, Weapons.THROWN));
                        player++;
                    }

=======
                    if (levelTemplate.LevelData[y, x] == TileType.WALL)
                        manager.addEntity(EFactory.createWall(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, 40, 40));
                    if (levelTemplate.LevelData[y, x] == TileType.SPAWN)
                    {
                        manager.addEntity(EFactory.createSpawn(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, 40, 40));
                        manager.addEntity(EFactory.createAvatar(new Rectangle(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, 40, 40), (Allegiance)player, Weapons.THROWN));
                        player++;
                    }
                    
>>>>>>> origin/Networking
                }
            built = true;
        }

<<<<<<< HEAD
        public override void update(GameTime gameTime)
        {
            if (level == null)
                initialize();
                
=======
        public override void update(double lastUpdate)
        {
            if (level == null)
                initialize();
>>>>>>> origin/Networking
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
<<<<<<< HEAD
                    update(gameTime);
=======
                    update(lastUpdate);
>>>>>>> origin/Networking
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
<<<<<<< HEAD
            level = (LevelComponent)levelEntity.Components[Masks.LEVEL];
            currentLevel = level.CurrentLevel;
            buildLevel(currentLevel);
=======
            level = (LevelComponent)levelEntity.Components[typeof(LevelComponent)];
            currentLevel = level.CurrentLevel;
>>>>>>> origin/Networking
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
