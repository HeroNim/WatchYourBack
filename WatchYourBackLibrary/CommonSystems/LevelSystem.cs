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
        private Dictionary<LevelName, LevelTemplate> levels;
        private LevelName currentLevel;
        private LevelInfo level;
        private bool built;

        public LevelSystem(Dictionary<LevelName, LevelTemplate> levels)
            : base(false, true, 1)
        {
            this.levels = levels;
            built = false;
        }

        public override void Update(TimeSpan gameTime)
        {
            if(currentLevel != level.CurrentLevel)
            {
                ClearLevel();
                currentLevel = level.CurrentLevel;
                Update(gameTime);
            }
            if (!built)
            {
                BuildLevel(currentLevel);
                level.Start();
            }
            if (level.Reset)
                ResetLevel();

            if (!level.Playing)
            {
                //Console.WriteLine("Game over");                
            }                            
        }

        public void AddLevel(LevelTemplate level)
        {
            levels.Add(level.Name, level);
        }

        /// <summary>
        /// Builds a level, including all the entities which are intrinsic to the level, such as walls and spawn points, as well as initializing it by adding
        /// the initial avatars and starting the game timer.
        /// </summary>
        /// <param name="levelName">The name of the level to be built</param>
        private void BuildLevel(LevelName levelName)
        {
            int player = 1;

            LevelTemplate levelTemplate = levels[levelName];
            int y, x;
            for (y = 0; y < (int)LevelDimensions.HEIGHT; y++)
                for (x = 0; x < (int)LevelDimensions.WIDTH; x++)
                {
                    if (levelTemplate.LevelData[y, x] == (int)TileType.WALL)
                    {
                        Entity wall = EFactory.CreateWall(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, (int)LevelDimensions.X_SCALE, (int)LevelDimensions.Y_SCALE,
                            levelTemplate.TileIndex(y, x), manager.HasGraphics());
                        manager.AddEntity(wall);
                        level.Walls.Add(wall);

                        //GraphicsComponent g = (GraphicsComponent)wall.Components[Masks.Graphics];
                        //g.DebugPoints.Clear();
                        Vector2 vertex;

                        if (levelTemplate.CornerVertices[y, x][0] == true)
                        {
                            vertex = new Vector2(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE);
                            level.AddVertex(vertex);
                            //g.DebugPoints.Add(new Vector2(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE));
                        }
                        if (levelTemplate.CornerVertices[y, x][1] == true){
                            vertex = new Vector2((x + 1) * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE);
                            level.AddVertex(vertex);
                            //g.DebugPoints.Add(new Vector2((x + 1) * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE));
                        }
                        if (levelTemplate.CornerVertices[y, x][2] == true){
                            vertex = new Vector2(x * (int)LevelDimensions.X_SCALE, (y + 1) * (int)LevelDimensions.Y_SCALE);
                            level.AddVertex(vertex);
                           // g.DebugPoints.Add(new Vector2(x * (int)LevelDimensions.X_SCALE, (y + 1) * (int)LevelDimensions.Y_SCALE));
                        }
                        if (levelTemplate.CornerVertices[y, x][3] == true){
                            vertex = new Vector2((x + 1) * (int)LevelDimensions.X_SCALE, (y + 1) * (int)LevelDimensions.Y_SCALE);
                            level.AddVertex(vertex);
                            //g.DebugPoints.Add(new Vector2((x + 1) * (int)LevelDimensions.X_SCALE, (y + 1) * (int)LevelDimensions.Y_SCALE));
                        }

                        

   
                    }
                    if (levelTemplate.LevelData[y, x] == (int)TileType.SPAWN)
                    {
                        Entity spawn = EFactory.CreateSpawn(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE, (int)LevelDimensions.X_SCALE, (int)LevelDimensions.Y_SCALE);
                        Entity avatar = EFactory.CreateAvatar(new PlayerInfoComponent((Allegiance)player), new Rectangle(x * (int)LevelDimensions.X_SCALE, y * (int)LevelDimensions.Y_SCALE,
                           40, 40), (Allegiance)player, Weapons.SWORD, manager.HasGraphics());

                        manager.AddEntity(spawn);
                        manager.AddEntity(avatar);

                        level.Spawns.Add(spawn);
                        level.Avatars.Add(avatar);
                        player++;
                    }
                }
            built = true;
        }

        /// <summary>
        /// Initializes the system when the game starts, loading the first level and updating the manager accordingly.
        /// </summary>
        public override void Initialize(IECSManager manager)
        {
            base.Initialize(manager);
            level = new LevelInfo();
            level.Levels = this.levels;

            manager.LevelInfo = level;
            currentLevel = level.CurrentLevel;         
        }
        
        /// <summary>
        /// Removes all entities from the level, apart from information and ui entities
        /// </summary>
        private void ClearLevel()
        {
            foreach (Entity entity in manager.Entities.Values)
                if(level.Contains(entity))
                    manager.RemoveEntity(entity);
            level.ResetLevel();
            built = false;
        }
       
        /// <summary>
        /// Resets a level, moving avatars back to their spawns and removing destructable entities such as swords or thrown weapons
        /// </summary>
        private void ResetLevel()
        {
            foreach (Entity entity in manager.Entities.Values)
                if (entity.IsDestructable)
                    manager.RemoveEntity(entity);
            for(int i = 0; i < level.Avatars.Count; i++)
            {
                manager.RemoveEntity(level.Avatars[i]);
                TransformComponent transform = (TransformComponent)level.Spawns[i].Components[Masks.Transform];
                PlayerInfoComponent info = (PlayerInfoComponent)level.Avatars[i].Components[Masks.PlayerInfo];
                Entity avatar = EFactory.CreateAvatar(info, new Rectangle((int)transform.X, (int)transform.Y, 40, 40),
                             (Allegiance)i, Weapons.SWORD, manager.HasGraphics());
                manager.AddEntity(avatar);
                level.Avatars[i] = avatar;                
            }
            level.Reset = false;
        }
    }
}
