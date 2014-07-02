using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Lidgren.Network;

namespace WatchYourBackServer
{
    class GameLoop
    {
        double nextUpdate;
        double lastUpdate;
        
        World inGame;

        Dictionary<LevelName, LevelTemplate> levels;


        public GameLoop()
        {
           
            Initialize();
            LoadContent();
            while (true)
                Update();
        }

        private void Initialize()
        {
            // TODO: Add your initialization logic here
            NetPeerConfiguration config = new NetPeerConfiguration("WatchYourBack");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = 14242;

            NetServer server = new NetServer(config);
            server.Start();

            nextUpdate = NetTime.Now;
            lastUpdate = 0;
            levels = new Dictionary<LevelName, LevelTemplate>();

            //FIXFIXFIXFIXFIXFIXFIX
            levels.Add(LevelName.TEST_LEVEL, new LevelTemplate(testLevelLayout));
            levels.Add(LevelName.FIRST_LEVEL, new LevelTemplate(levelOne));
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        private void LoadContent()
        {
            createGame();
        }




        private void Update()
        {
            double now = NetTime.Now;
            //Console.WriteLine(now);
            if (now > nextUpdate)
            {
                inGame.Manager.update(lastUpdate);
                //Console.WriteLine(NetTime.Now);
                nextUpdate += (1.0 / 60.0);
                lastUpdate = NetTime.Now;
            }
            
        }
       
        private void createGame()
        {
            inGame = new World(Worlds.IN_GAME);
            NetworkIOSystem input = new NetworkIOSystem();
            inGame.Manager.addSystem(input);
            inGame.Manager.addInput(input);

            inGame.Manager.addSystem(new AvatarInputSystem());
            inGame.Manager.addSystem(new GameCollisionSystem());
            inGame.Manager.addSystem(new MovementSystem());
           // inGame.Manager.addSystem(new LevelSystem(levels));
            inGame.Manager.addSystem(new AttackSystem());


        }

       

        private void reset(World world)
        {
                createGame();
        }

        

        
        
    }
}
