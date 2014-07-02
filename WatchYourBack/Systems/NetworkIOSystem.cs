using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Lidgren.Network;
using WatchYourBackLibrary;

namespace WatchYourBack
{
    class NetworkIOSystem : ESystem
    {
        private Dictionary<KeyBindings, Keys> mappings;
        private int xInput;
        private int yInput;
        private Vector2 mouseLocation;
        private bool mouseClicked;

        private NetworkArgs toSend;
        private NetClient client;


        public NetworkIOSystem(NetClient client) : base(false, true, 1)
        {
            this.client = client;
            mappings = new Dictionary<KeyBindings, Keys>();
            mappings.Add(KeyBindings.LEFT, Keys.Left);
            mappings.Add(KeyBindings.RIGHT, Keys.Right);
            mappings.Add(KeyBindings.UP, Keys.Up);
            mappings.Add(KeyBindings.DOWN, Keys.Down);
            mappings.Add(KeyBindings.PAUSE, Keys.Escape);
            mappings.Add(KeyBindings.ATTACK, Keys.Space);
        }

        public override void update(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();
            mouseLocation = new Vector2(ms.X, ms.Y);

            if (Keyboard.GetState().IsKeyDown(mappings[KeyBindings.RIGHT]))
                xInput = 1;
            if (Keyboard.GetState().IsKeyDown(mappings[KeyBindings.LEFT]))
                xInput = -1;
            if (Keyboard.GetState().IsKeyDown(mappings[KeyBindings.UP]))
                yInput = -1;
            if (Keyboard.GetState().IsKeyDown(mappings[KeyBindings.DOWN]))
                yInput = 1;
            if (ms.LeftButton == ButtonState.Pressed)
                mouseClicked = true;

            toSend = new NetworkArgs(xInput, yInput, mouseLocation, mouseClicked);
            NetOutgoingMessage om = client.CreateMessage();
            om.Write(Serialize(toSend));
            client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            
            
            reset();
            
        }


        

        private void reset()
        {
            xInput = 0;
            yInput = 0;
            mouseClicked = false;
        }

        private byte[] Serialize(object objectToSerialize)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, objectToSerialize);
            byte[] result = new Byte[stream.Length];
            stream.Position = 0;
            stream.Read(result, 0, (int)stream.Length);
            stream.Close();
            return result;
            
        }

    }
}
