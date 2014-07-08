using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Lidgren.Network;
using WatchYourBackLibrary;

namespace WatchYourBackServer
{
    public enum SERVER_PROPERTIES
    {
        TIME_STEP = 60,
        MAX_CONNECTIONS = 1,
        PORT = 14242
    }

    

    class ServerGameLoop
    {
        NetServer server;
        bool initializing;
        bool playing;
        TimeSpan gameTime;
        NetPeerConfiguration config;
        

        float nextUpdate;
        
        World inGame;

        List<LevelTemplate> levels;


        public ServerGameLoop()
        {

            config = new NetPeerConfiguration("WatchYourBack");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = (int)SERVER_PROPERTIES.PORT;
            config.EnableUPnP = true;

            server = new NetServer(config);
            server.Start();

            gameTime = new TimeSpan();
            playing = false;
            initializing = false;
            Initialize();
            while (true)
                Update();
        }

        private void Initialize()
        {
            // TODO: Add your initialization logic here

            nextUpdate = (float)NetTime.Now;
            levels = new List<LevelTemplate>();


            NetIncomingMessage msg;
            while (!playing)
            {
                while ((msg = server.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryRequest:
                            server.SendDiscoveryResponse(null, msg.SenderEndpoint);
                            Console.WriteLine("yo");
                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            //
                            // Just print diagnostic messages to console
                            //
                            Console.WriteLine(msg.ReadString());
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                            if (status == NetConnectionStatus.Connected)
                            {
                                Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected");
                                if (server.ConnectionsCount == (int)SERVER_PROPERTIES.MAX_CONNECTIONS)
                                {
                                    initializing = true;
                                    NetOutgoingMessage om = server.CreateMessage();
                                    om.Write((int)SERVER_COMMANDS.SEND_LEVELS);
                                    server.SendMessage(om, server.Connections[0], NetDeliveryMethod.ReliableUnordered);
                                    Console.WriteLine("Initializing");
                                    break;
                                }
                            }
                            if (status == NetConnectionStatus.Disconnected)
                            {
                                Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " disconnected");

                            }
                            break;
                        case NetIncomingMessageType.Data:
                            //
                            // The client sent input to the server
                            //
                            if (initializing)
                            {
                                levels = SerializationHelper.DeserializeObject<List<LevelTemplate>>(msg.ReadBytes(msg.LengthBytes));
                                Console.WriteLine("Levels recieved");
                                foreach (LevelTemplate level in levels)
                                    Console.WriteLine(level.ToString());
                                Console.WriteLine("Starting game");
                                NetOutgoingMessage om = server.CreateMessage();
                                int index = 0;
                                foreach (NetConnection player in server.Connections)
                                {
                                    om.Write(index);
                                    server.SendMessage(om, player, NetDeliveryMethod.ReliableUnordered);
                                    index++;
                                }

                                playing = true;
                                
                            }


                            break;
                    }
                }
            }
            Thread.Sleep(100);

            inGame = new World(Worlds.IN_GAME);
            inGame.addManager(new ServerECSManager(server.ConnectionsCount));
            inGame.Manager.Playing = true;
            ServerUpdateSystem input = new ServerUpdateSystem(server);
            inGame.Manager.addSystem(input);
            inGame.Manager.addInput(input);
            input.Accumulator = inGame.Manager.Accumulator;

            inGame.Manager.addSystem(new AvatarInputSystem());
            inGame.Manager.addSystem(new GameCollisionSystem());
            inGame.Manager.addSystem(new MovementSystem());
            inGame.Manager.addSystem(new LevelSystem(levels));
            inGame.Manager.addSystem(new AttackSystem());

            
            //FIXFIXFIXFIXFIXFIXFIX
            //levels.Add(LevelName.TEST_LEVEL, new LevelTemplate(testLevelLayout));
            //levels.Add(LevelName.FIRST_LEVEL, new LevelTemplate(levelOne));
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
       



        //Pseudo-XNA style gametime. Would probably cause problems on a larger scale game (still might).
        private void Update()
        {
            while (true)
            {
                while (inGame.Manager.Playing == true)
                    inGame.Manager.update(gameTime);
                reset();
            }
            
        }

        private void reset()
        {
            Console.WriteLine(server.ConnectionsCount);
            playing = false;
                Initialize();
        }

        

        
        
    }
}
