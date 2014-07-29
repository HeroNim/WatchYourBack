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
    

    public enum LevelDimensions
    {
        WIDTH = 64,
        HEIGHT = 36,
        X_SCALE = 20,
        Y_SCALE = 20
    };


    /// <summary>
    /// The system which manages the levels of the game. Contains a level component which holds the information, and methods to build, update, and reset levels.
    /// </summary>
    public class LevelSystem : ESystem
    {

        private List<LevelTemplate> levels;
        private LevelName currentLevel;
        private LevelComponent level;
        private bool built;

        public LevelSystem(List<LevelTemplate> levels) : base(false, true, 7)
        {
            components += (int)Masks.LEVEL;
            this.levels = levels;
            built = false;
        }


        public override void update(TimeSpan gameTime)
        {
            if (level == null)
                initialize();


            if (currentLevel == level.CurrentLevel)
            {
                if (!built)
                {
                    buildLevel(currentLevel);
                    level.Start();
                }
                else if (level.Reset == true)
                    resetLevel();
            }
            else
            {
                clearLevel();
                currentLevel = level.CurrentLevel;
                update(gameTime);
            }
                             
        }


        public void addLevel(LevelTemplate level)
        {
            levels.Add(level);
        }

        /// <summary>
        /// Builds a level, including all the entities which are intrinsic to the level, such as walls and spawn points, as well as initializing it by adding
        /// the initial avatars and starting the game timer.
        /// </summary>
        /// <param name="levelName">The name of the level to be built</param>
        private void buildLevel(LevelName levelName)
        {
            int player = 1;

            LevelTemplate levelTemplate = levels.Find(o => o.Name == levelName);
            int y, x;
            for (y = 0; y < (int)LevelDimensions.HEIGHT; y++)
                for (x = 0; x < (int)LevelDimensions.WIDTH; x++)
                {
                    if (levelTemplate.LevelData[y, x] == (int)TileType.WALL)
                    {
                        Entity wall = EFactory.createWall(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, (int)LevelDimensions.X_SCALE, (int)LevelDimensions.Y_SCALE,
                            levelTemplate.SubIndex(y, x), manager.hasGraphics());
                        manager.addEntity(wall);
                        level.Walls.Add(wall);

                    }
                    if (levelTemplate.LevelData[y, x] == (int)TileType.SPAWN)
                    {
                        Entity spawn = EFactory.createSpawn(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, (int)LevelDimensions.X_SCALE, (int)LevelDimensions.Y_SCALE);
                        Entity avatar = EFactory.createAvatar(new PlayerInfoComponent((Allegiance)player), new Rectangle(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE,
                           40, 40), (Allegiance)player, Weapons.SWORD, manager.hasGraphics());

                        manager.addEntity(spawn);
                        manager.addEntity(avatar);

                        level.Spawns.Add(spawn);
                        level.Avatars.Add(avatar);
                        player++;
                    }

                }
            level.GameTime = 300;
            built = true;
        }

        /// <summary>
        /// Initializes the system when the game starts, loading the first level and updating the manager accordingly.
        /// </summary>
        private void initialize()
        {         
            Entity levelEntity = new Entity();
            levelEntity.addComponent(new LevelComponent());
            manager.addEntity(levelEntity);
            level = (LevelComponent)levelEntity.Components[Masks.LEVEL];
            manager.LevelInfo = level;
            currentLevel = level.CurrentLevel;
           
        }

        
        /// <summary>
        /// Removes all entities from the level, apart from information and ui entities
        /// </summary>
        private void clearLevel()
        {
            foreach (Entity entity in manager.Entities.Values)
                if(level.Contains(entity) || entity.IsDestructable)
                    manager.removeEntity(entity);
            level.ResetLevel();
            built = false;
        }
       
        /// <summary>
        /// Resets a level, moving avatars back to their spawns and removing destructable entities such as swords or thrown weapons
        /// </summary>
        private void resetLevel()
        {
            foreach (Entity entity in manager.Entities.Values)
                if (entity.IsDestructable)
                    manager.removeEntity(entity);
            for(int i = 0; i < level.Avatars.Count; i++)
            {
                manager.removeEntity(level.Avatars[i]);
                TransformComponent transform = (TransformComponent)level.Spawns[i].Components[Masks.TRANSFORM];
                PlayerInfoComponent info = (PlayerInfoComponent)level.Avatars[i].Components[Masks.PLAYER_INFO];
                Entity avatar = EFactory.createAvatar(info, new Rectangle((int)transform.X, (int)transform.Y, 40, 40),
                             (Allegiance)i, Weapons.SWORD, manager.hasGraphics());
                manager.addEntity(avatar);
                level.Avatars[i] = avatar;
                
            }
            level.Reset = false;
        }
    }
}
