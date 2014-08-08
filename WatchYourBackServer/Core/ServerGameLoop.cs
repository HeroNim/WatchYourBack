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
    /// <summary>
    /// The main game loop for the server. If no game is running, it waits for clients to connect. Once two clients connect, it begins the game. If a client disconnects, 
    /// it goes back to waiting for clients to connect.
    /// </summary>
    class ServerGameLoop
    {
        NetServer server;
        bool initializing;
        bool playing;
        TimeSpan gameTime;
        NetPeerConfiguration config;
        
        float nextUpdate;       
        World inGame;
        Dictionary<LevelName, LevelTemplate> levels;

        public ServerGameLoop()
        {
            config = new NetPeerConfiguration("WatchYourBack");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = (int)ServerSettings.Port;
            config.EnableUPnP = true;
            config.ConnectionTimeout = (int)ServerSettings.TimeOut;

            server = new NetServer(config);
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
            server.Start();
            
            nextUpdate = (float)NetTime.Now;
            levels = new Dictionary<LevelName, LevelTemplate>();

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
                                if (server.ConnectionsCount == (int)ServerSettings.MaxConnections)
                                {
                                    initializing = true;
                                    NetOutgoingMessage om = server.CreateMessage();
                                    om.Write((int)ServerCommands.SendLevels);
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
                                levels = SerializationHelper.DeserializeObject<Dictionary<LevelName, LevelTemplate>>(msg.ReadBytes(msg.LengthBytes));
                                Console.WriteLine("Levels recieved");
                                foreach (LevelTemplate level in levels.Values)
                                    Console.WriteLine(level.ToString());
                                Console.WriteLine("Starting game");
                                
                                foreach (NetConnection player in server.Connections)
                                {
                                    NetOutgoingMessage om = server.CreateMessage();
                                    om.Write((int)ServerCommands.Start);
                                    server.SendMessage(om, player, NetDeliveryMethod.ReliableUnordered);
                                }
                                playing = true;
                            }
                            break;
                    }
                }
            }
            Thread.Sleep(100);

            inGame = new World(Worlds.InGame, true, true);
            inGame.addManager(new ServerECSManager(server.ConnectionsCount));
            inGame.Manager.Playing = true;
            ServerUpdateSystem input = new ServerUpdateSystem(server);
            input.Accumulator = new double[server.ConnectionsCount];
            inGame.Manager.addSystem(input);           

            inGame.Manager.addSystem(new AvatarInputSystem());
            inGame.Manager.addSystem(new GameCollisionSystem());
            inGame.Manager.addSystem(new MovementSystem());
            inGame.Manager.addSystem(new LevelSystem(levels));
            inGame.Manager.addSystem(new AttackSystem());

            foreach(ESystem system in inGame.Manager.Systems)
            {
                if (system != input)
                    system.inputFired += new EventHandler(input.EventListener);
            }
        }

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
            Thread.Sleep(100);
            server.Shutdown("Restarting");
            Thread.Sleep(100);
            Console.WriteLine(server.ConnectionsCount);
            playing = false;
                Initialize();
        }        
    }
}
