using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using Microsoft.Xna.Framework;

using Lidgren.Network;
using WatchYourBackLibrary;


namespace WatchYourBackServer
{
    /// <summary>
    /// The system responsible for updating and sending updates to the client when playing online.
    /// </summary>
    class ServerUpdateSystem : ESystem
    {

        NetServer server;
        List<EventArgs> sendData;
        bool updating;
        int playerIndex;
        const double timeStep = 1.0 / (double)ServerSettings.TimeStep;
        double[] interpolation;
        double[] accumulator;

        LevelInfo level;
        Dictionary<long, int> playerMap;
        int[] scores;

        
        public ServerUpdateSystem(NetServer server)
            : base(false, true, 10)
        {
            components += (int)Masks.PlayerInput;
            this.server = server;
            sendData = new List<EventArgs>();
            accumulator = new double[server.ConnectionsCount];
            interpolation = new double[server.ConnectionsCount];
            updating = true;
            playerIndex = 0;
            

            scores = new int[2];
            playerMap = new Dictionary<long, int>();

            foreach(NetConnection player in server.Connections)
            {
                playerMap.Add(player.RemoteUniqueIdentifier, playerIndex);
                playerIndex++;
            }
        }


 
         

        /// <summary>
        /// The update loop updates the status of all the entities the player is responsible for drawing, and sends the resulting data to the players. It also is responsible
        /// for receiving input from the players, and appropriately dealing with it.
        /// </summary>
        /// <remarks>
        /// The update loop pulls all the draw rates of the players, and adds that value to their accumulators. When EVERY accumulator has more time than the fixed
        /// update rate, the server updates the entities. In the meantime, any player that is waiting on an update will receive interpolated graphics values whenever
        /// they send input, based on the ratio between how much time is in their accumulator and the fixed update rate. This means that every player should receive
        /// at least one packet from the server per cycle.
        /// </remarks>
        /// <param name="gameTime">The update time of the game</param>
        public override void update(TimeSpan gameTime)
        {
            if (level == null)
                level = manager.LevelInfo;

            if (checkGame())
            {
                if (updating)
                {
                    checkGame();
                    packEntities();
                    packScores();

                    NetOutgoingMessage om = server.CreateMessage();
                    om.Write(SerializationHelper.Serialize(sendData));
                    server.SendToAll(om, NetDeliveryMethod.UnreliableSequenced);
                    sendData.Clear();
                    manager.ChangedEntities.Clear();
                    manager.RemoveAll();
                    updating = false;
                    for (int i = 0; i < accumulator.Length; i++)
                    {
                        accumulator[i] -= timeStep;
                    }
                }

                while (!updating && manager.Playing == true)
                {
                    NetIncomingMessage msg;
                    NetworkInputArgs args;
                    AvatarInputComponent playerInputComponent;
                    NetOutgoingMessage om;


                    while ((msg = server.ReadMessage()) != null)
                    {
                        playerIndex = playerMap[msg.SenderConnection.RemoteUniqueIdentifier];
                        om = server.CreateMessage();
                        switch (msg.MessageType)
                        {
                            case NetIncomingMessageType.DiscoveryRequest:
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
                                if (status == NetConnectionStatus.Disconnected)
                                {
                                    Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " disconnected. Ending game.");
                                    sendData.Clear();
                                    sendData.Add(new NetworkUpdateArgs(ServerCommands.Disconnect));
                                    om.Write(SerializationHelper.Serialize(sendData));
                                    server.SendToAll(om, NetDeliveryMethod.ReliableOrdered);
                                    sendData.Clear();
                                    
                                    manager.Playing = false;
                                }
                                break;
                            case NetIncomingMessageType.Data:
                                //
                                // The client sent input to the server
                                //
                                args = SerializationHelper.DeserializeObject<NetworkInputArgs>(msg.ReadBytes(msg.LengthBytes));
                                playerInputComponent = (AvatarInputComponent)level.Avatars[playerIndex].Components[Masks.PlayerInput];

                                if (args.XInput == 1)
                                    playerInputComponent.MoveX = 1;
                                else if (args.XInput == -1)
                                    playerInputComponent.MoveX = -1;
                                else
                                    playerInputComponent.MoveX = 0;

                                if (args.YInput == 1)
                                    playerInputComponent.MoveY = 1;
                                else if (args.YInput == -1)
                                    playerInputComponent.MoveY = -1;
                                else
                                    playerInputComponent.MoveY = 0;

                                if (args.LeftClicked)
                                    playerInputComponent.SwingWeapon = true;
                                if (args.RightClicked)
                                    playerInputComponent.ThrowWeapon = true;
                                if (args.Dash)
                                    playerInputComponent.Dash = true;

                                playerInputComponent.LookX = args.MouseX;
                                playerInputComponent.LookY = args.MouseY;



                                accumulator[playerIndex] += args.DrawTime;
                                if (accumulator[playerIndex] < timeStep)
                                    interpolation[playerIndex] = accumulator[playerIndex] / timeStep;
                                interpolate(interpolation[playerIndex]);
                                Console.WriteLine(playerIndex);

                                om.Write(SerializationHelper.Serialize(sendData));
                                server.SendMessage(om, server.Connections[playerIndex], NetDeliveryMethod.ReliableOrdered);
                                sendData.Clear();

                                break;

                        }
                        server.Recycle(msg);

                        for (int i = 0; i < accumulator.Length; i++)
                        {
                            if (accumulator[i] < timeStep)
                            {
                                updating = false;
                                break;
                            }
                            updating = true;
                        }
                        if (updating)
                            break;
                    }


                }




                //Output

            }


        }

        public bool checkGame()
        {
            if (!level.Playing)
            {
                sendData.Clear();
                int comparison = scores[0].CompareTo(scores[1]);
                int winner = -1;
                if (comparison == -1)
                    winner = 1;
                else if (comparison == 1)
                    winner = 0;


                if(winner != -1)
                    for (int i = 0; i < server.ConnectionsCount; i++)
                    {
                        NetOutgoingMessage om = server.CreateMessage();
                        if (i == winner)
                            sendData.Add(new NetworkUpdateArgs(ServerCommands.Win));
                        else
                            sendData.Add(new NetworkUpdateArgs(ServerCommands.Lose));
                        om.Write(SerializationHelper.Serialize(sendData));
                        server.SendMessage(om, server.Connections[i], NetDeliveryMethod.ReliableOrdered);
                        sendData.Clear();

                    }
                else
                {
                    NetOutgoingMessage om = server.CreateMessage();
                    sendData.Add(new NetworkUpdateArgs(ServerCommands.Tie));
                    om.Write(SerializationHelper.Serialize(sendData));
                    server.SendToAll(om, NetDeliveryMethod.ReliableOrdered);
                    sendData.Clear();

                }
               
                

                manager.Playing = false;
                return false;
            }
            return true;
        }

        public void packEntities()
        {
            foreach (int id in manager.ChangedEntities.Keys)
            {
                Entity e = manager.Entities[id];
                if (e.hasComponent(Masks.Transform) && e.Drawable == true)
                {
                    TransformComponent transform = (TransformComponent)e.Components[Masks.Transform];
                    if (e.hasComponent(Masks.Tile))
                    {
                        TileComponent tile = (TileComponent)e.Components[Masks.Tile];
                        sendData.Add(new NetworkEntityArgs(e.Type, manager.ChangedEntities[id], e.ServerID, transform.X, transform.Y, transform.Width, transform.Height, transform.Rotation, tile.SubIndex));
                    }
                    else
                        sendData.Add(new NetworkEntityArgs(e.Type, manager.ChangedEntities[id], e.ServerID, transform.X, transform.Y, transform.Width, transform.Height, transform.Rotation, 0));
                }
            }
        }

        public void packScores()
        {          
            {
                for (int i = 0; i < 2; i++)
                {
                    PlayerInfoComponent p = (PlayerInfoComponent)level.Avatars[i].Components[Masks.PlayerInfo];
                    scores[i] = p.Score;
                }
                sendData.Add(new NetworkGameArgs(scores, level.GameTime));
            }     
        }
        

        /// <summary>
        /// Interpolates a new position for each entity based on the interpolation factor, without actually changing the location of the entity. This allows
        /// each player to draw entities as they should see them, depending on their update rates.
        /// </summary>
        /// <param name="interpolationFactor">The ratio between the player's elapsed time and the update rate of the server</param>
        private void interpolate(double interpolationFactor)
        {
            foreach(int id in manager.ChangedEntities.Values)
            {
                Entity e = manager.Entities[id];
                if(e.hasComponent(Masks.Transform) && e.hasComponent(Masks.Velocity))
                {
                    TransformComponent transform = (TransformComponent)e.Components[Masks.Transform];
                    VelocityComponent velocity = (VelocityComponent)e.Components[Masks.Velocity];
                    float x = transform.X + (float)(velocity.X * interpolationFactor);
                    float y = transform.Y + (float)(velocity.Y * interpolationFactor);
                    float rotation = transform.Rotation + (float)(velocity.RotationSpeed * interpolationFactor);
                    
                    if(e.hasComponent(Masks.Tile))
                    {
                        TileComponent tile = (TileComponent)e.Components[Masks.Tile];
                        sendData.Add(new NetworkEntityArgs(e.Type, EntityCommands.Modify, e.ServerID, x, y, transform.Width, transform.Height, rotation, tile.AtlasIndex));
                    }
                    else
                        sendData.Add(new NetworkEntityArgs(e.Type, EntityCommands.Modify, e.ServerID, x, y, transform.Width, transform.Height, rotation, 0));
                }
            }
        }

        public double[] Accumulator
        {
            get { return accumulator; }
            set { accumulator = value; }
        }

        public override void EventListener(object sender, EventArgs e)
        {
            if(e is SoundArgs)
            {
                SoundArgs s = (SoundArgs)e;
                sendData.Add(s);

                NetOutgoingMessage om = server.CreateMessage();
                om.Write(SerializationHelper.Serialize(sendData));
                server.SendToAll(om, NetDeliveryMethod.UnreliableSequenced);
                sendData.Clear();
            }
        }
        
    }
}
