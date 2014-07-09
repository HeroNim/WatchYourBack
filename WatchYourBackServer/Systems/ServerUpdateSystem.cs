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
    class ServerUpdateSystem : ESystem, InputSystem
    {
        NetServer server;
        List<NetworkEntityArgs> sendData;
        bool updating;
        int playerIndex;
        const double timeStep = 1.0 / (double)SERVER_PROPERTIES.TIME_STEP;
        double[] interpolation;
        double[] accumulator;
        Dictionary<long, int> playerMap;


        public ServerUpdateSystem(NetServer server)
            : base(false, true, 10)
        {
            components += (int)Masks.PLAYER_INPUT;
            this.server = server;
            sendData = new List<NetworkEntityArgs>();
            interpolation = new double[server.ConnectionsCount];
            updating = true;
            playerIndex = 0;
            playerMap = new Dictionary<long, int>();

            foreach(NetConnection player in server.Connections)
            {
                playerMap.Add(player.RemoteUniqueIdentifier, playerIndex);
                playerIndex++;
            }


        }


        /*
         * The update loop pulls all the draw rates of the players, and adds that value to their accumulators. When EVERY accumulator has more time than the fixed
         * update rate, the server updates the entities. In the meantime, any player that is waiting on an update will receive interpolated graphics values whenever
         * they send input, based on the ratio between how much time is in their accumulator and the fixed update rate. This means that every player should receive
         * at least one packet from the server per cycle.
         * 
         * For example, if one player is drawing four times faster than another, the faster player will receive four interpolated values before receiving an actual update.
         * To the client, the interpolated values are no different than an actual update; however, the server side entities are not actually updated when interpolation occurs.
         */
        public override void update(TimeSpan gameTime)
        {
            if (updating)
            {
                foreach (int id in manager.ChangedEntities.Keys)
                {
                    Entity e = manager.ActiveEntities[id];
                    if (e.hasComponent(Masks.TRANSFORM) && e.Drawable == true)
                    {
                        TransformComponent transform = (TransformComponent)e.Components[Masks.TRANSFORM];
                        sendData.Add(new NetworkEntityArgs(e.Type, manager.ChangedEntities[id], e.ID, transform.X, transform.Y, transform.Width, transform.Height, transform.Rotation));
                    }
                }

                NetOutgoingMessage om = server.CreateMessage();
                om.Write(SerializationHelper.Serialize(sendData));
                server.SendToAll(om, NetDeliveryMethod.UnreliableSequenced);
                sendData.Clear();
                manager.ChangedEntities.Clear();
                manager.RemoveAll();
                updating = false;
                for (int i = 0; i < accumulator.Length; i++ )
                {
                    accumulator[i] -= timeStep;
                }
            }

            while (!updating && manager.Playing == true)
            { 
                NetIncomingMessage msg;
                NetworkInputArgs args;
                AvatarInputComponent playerInputComponent;
                


                while ((msg = server.ReadMessage()) != null)
                {
                        playerIndex = playerMap[msg.SenderConnection.RemoteUniqueIdentifier];
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
                                    manager.Playing = false;
                                }
                                break;
                            case NetIncomingMessageType.Data:
                                //
                                // The client sent input to the server
                                //
                                args = SerializationHelper.DeserializeObject<NetworkInputArgs>(msg.ReadBytes(msg.LengthBytes));
                                playerInputComponent = (AvatarInputComponent)activeEntities[playerIndex].Components[Masks.PLAYER_INPUT];

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

                                playerInputComponent.LookX = args.MouseX;
                                playerInputComponent.LookY = args.MouseY;



                                accumulator[playerIndex] += args.DrawTime;
                                interpolate(playerIndex, interpolation[playerIndex]);

                                break;
                        
                    }
                    server.Recycle(msg);
                    
                   
                }
               

                for(int i = 0; i < manager.Accumulator.Length; i++)
                {
                    if (accumulator[i] < timeStep)
                    {
                        updating = false;
                        interpolation[i] = (accumulator[i] % timeStep) / timeStep;
                        break;
                    }
                    updating = true;
                }
            }




            //Output

            


        }
        

        public event EventHandler inputFired;

        private void onFire(EventArgs e)
        {
            if (inputFired != null)
                inputFired(this, e);
        }

        private void onFire(Entity sender, EventArgs e)
        {
            if (inputFired != null)
                inputFired(sender, e);
        }

        private void interpolate(int playerIndex, double interpolationFactor)
        {
            foreach(int id in manager.ChangedEntities.Values)
            {
                Entity e = manager.ActiveEntities[id];
                if(e.hasComponent(Masks.TRANSFORM) && e.hasComponent(Masks.VELOCITY))
                {
                    TransformComponent transform = (TransformComponent)e.Components[Masks.TRANSFORM];
                    VelocityComponent velocity = (VelocityComponent)e.Components[Masks.VELOCITY];
                    float x = transform.X + (float)(velocity.X * interpolationFactor);
                    float y = transform.Y + (float)(velocity.Y * interpolationFactor);
                    float rotation = transform.Rotation + (float)(velocity.RotationSpeed * interpolationFactor);
                    sendData.Add(new NetworkEntityArgs(e.Type, COMMANDS.MODIFY, e.ID, x, y, transform.Width, transform.Height, rotation));
                }
            }

            NetOutgoingMessage om = server.CreateMessage();
            om.Write(SerializationHelper.Serialize(sendData));
            server.SendMessage(om, server.Connections[playerIndex], NetDeliveryMethod.ReliableOrdered);
            sendData.Clear();
        }

        public double[] Accumulator
        {
            get { return accumulator; }
            set { accumulator = value; }
        }
        
    }
}
