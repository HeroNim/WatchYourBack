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
        int playerIndex;



        public ServerUpdateSystem(NetServer server)
            : base(false, true, 2)
        {
            components += (int)Masks.PLAYER_INPUT;
            this.server = server;
            sendData = new List<NetworkEntityArgs>();


        }

        public override void update(TimeSpan gameTime)
        {
            NetIncomingMessage msg;
            NetworkInputArgs args;
            AvatarInputComponent playerInputComponent;
            playerIndex = 0;

            foreach (NetConnection player in server.Connections)
            {
                while ((msg = server.ReadMessage()) != null)
                {

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
                                Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " disconnected");
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

                            if (args.Clicked)
                                onFire(playerInputComponent.getEntity(), new InputArgs(Inputs.ATTACK, args.MouseX, args.MouseY));


                            break;
                    }
                    server.Recycle(msg);
                }




                playerIndex++;

            }
            playerIndex = 0;




            //Output

            foreach (int id in manager.ChangedEntities.Keys)
            {
                Entity e = manager.ActiveEntities[id];
                if (e.hasComponent(Masks.TRANSFORM))
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

        

        
    }
}
