using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using Microsoft.Xna.Framework;

using Lidgren.Network;
using WatchYourBackLibrary;


namespace WatchYourBackServer
{
    class NetworkIOSystem : ESystem, InputSystem
    {
        NetServer server;


        public NetworkIOSystem(NetServer server)
           : base(false, true, 2) 
        {
            components += AvatarInputComponent.bitMask;
            this.server = server;
        }

        public override void update(double lastUpdate)
        {
            NetIncomingMessage msg;
            while ((msg = server.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryRequest:
                        server.SendDiscoveryResponse(null, msg.SenderEndpoint);
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
                        NetworkArgs args = DeserializeObject<NetworkArgs>(msg.ReadBytes(msg.LengthBytes));
                        Console.WriteLine(args.ToString());
                        break;
                }
            }


            //Output

        }

        private T DeserializeObject<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(data);
            object result = formatter.Deserialize(stream);
            stream.Close();
            return (T)result;
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
