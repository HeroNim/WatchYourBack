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
                    buildLevel(currentLevel);
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

        private void buildLevel(LevelName levelName)
        {
            if (manager.hasGraphics())
            {
                wallTexture = manager.getTexture("TileTextures/WallTextureAtlas");
                spawnTexture = manager.getTexture("PlayerTexture");
            }
            int player = 1;

            LevelTemplate levelTemplate = levels.Find(o => o.Name == levelName);
            int y, x;
            for (y = 0; y < (int)LevelDimensions.HEIGHT; y++)
                for (x = 0; x < (int)LevelDimensions.WIDTH; x++)
                {
                    if (levelTemplate.LevelData[y, x] == (int)TileType.WALL)
                    {
                        Entity wall = EFactory.createWall(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, (int)LevelDimensions.X_SCALE, (int)LevelDimensions.Y_SCALE, 
                            wallTexture, levelTemplate.SubIndex(y, x), manager.hasGraphics());
                        manager.addEntity(wall);
                        
                    }
                    if (levelTemplate.LevelData[y, x] == (int)TileType.SPAWN)
                    {
                        Entity spawn = EFactory.createSpawn(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, (int)LevelDimensions.X_SCALE, (int)LevelDimensions.Y_SCALE);
                        Entity avatar = EFactory.createAvatar(new PlayerInfoComponent((Allegiance)player), new Rectangle(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, 
                           40, 40), (Allegiance)player, Weapons.SWORD, spawnTexture, manager.hasGraphics());

                        manager.addEntity(spawn);                        
                        manager.addEntity(avatar);

                        level.Spawns.Add(spawn);
                        level.Avatars.Add(avatar);
                        player++;
                    }
                    
                }
            built = true;
        }

        private void initialize()
        {         
            Entity levelEntity = new Entity();
            levelEntity.addComponent(new LevelComponent());
            manager.addEntity(levelEntity);
            level = (LevelComponent)levelEntity.Components[Masks.LEVEL];
            manager.LevelInfo = level;
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

        private void resetLevel()
        {
            foreach (Entity e in manager.ActiveEntities.Values)
                if (e.Type != ENTITIES.WALL)
                    manager.removeEntity(e);
            for(int i = 0; i < level.Spawns.Count; i++)
            {
                TransformComponent transform = (TransformComponent)level.Spawns[i].Components[Masks.TRANSFORM];
                PlayerInfoComponent info = (PlayerInfoComponent)level.Avatars[i].Components[Masks.PLAYER_INFO];
                Entity avatar = EFactory.createAvatar(info, new Rectangle((int)transform.X, (int)transform.Y, 40, 40),
                             (Allegiance)i, Weapons.SWORD, spawnTexture, manager.hasGraphics());
                manager.addEntity(avatar);
                level.Avatars[i] = avatar;
            }
            level.Reset = false;
        }
    }
}
