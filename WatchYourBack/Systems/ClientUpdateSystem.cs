using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Lidgren.Network;
using WatchYourBackLibrary;

namespace WatchYourBack
{
    class ClientUpdateSystem : ESystem
    {
        private Dictionary<KeyBindings, Keys> mappings;
        private int xInput;
        private int yInput;
        private Vector2 mouseLocation;
        private bool mouseClicked;
        List<NetworkEntityArgs> receivedData;
        List<NetworkEntityArgs> lastData;

        private List<COMMANDS>[] debug;


        private NetworkInputArgs toSend;
        private NetClient client;


        public ClientUpdateSystem(NetClient client) : base(false, true, 2)
        {
            this.client = client;
            
            receivedData = new List<NetworkEntityArgs>();
            lastData = new List<NetworkEntityArgs>();
            mappings = new Dictionary<KeyBindings, Keys>();
            mappings.Add(KeyBindings.LEFT, Keys.Left);
            mappings.Add(KeyBindings.RIGHT, Keys.Right);
            mappings.Add(KeyBindings.UP, Keys.Up);
            mappings.Add(KeyBindings.DOWN, Keys.Down);
            mappings.Add(KeyBindings.PAUSE, Keys.Escape);
            mappings.Add(KeyBindings.ATTACK, Keys.Space);

            debug = new List<COMMANDS>[5000];
            for (int i = 0; i < debug.Length; i++ )
                debug[i] = new List<COMMANDS>();
        }

        public override void update(TimeSpan gameTime)
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

            toSend = new NetworkInputArgs(client.UniqueIdentifier, xInput, yInput, mouseLocation, mouseClicked);
            NetOutgoingMessage om = client.CreateMessage();
            om.Write(SerializationHelper.Serialize(toSend));
            client.SendMessage(om, NetDeliveryMethod.ReliableOrdered); 
            reset();
            manager.ChangedEntities.Clear();

            NetIncomingMessage msg;
            
                while ((msg = client.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.Data:
                            receivedData = SerializationHelper.DeserializeObject<List<NetworkEntityArgs>>(msg.ReadBytes(msg.LengthBytes));
                            foreach (NetworkEntityArgs args in receivedData)
                            {
                                Rectangle body = new Rectangle((int)args.XPos, (int)args.YPos, args.Width, args.Height);
                                debug[args.ID].Add(args.Command);
                                Texture2D texture = null;
                                switch (args.Command)
                                {
                                    case COMMANDS.ADD:
                                        texture = getTexture(args.Type);
                                        manager.addEntity(EFactory.createGraphics(body, args.Rotation, args.ID, texture));
                                        break;
                                    case COMMANDS.REMOVE:
                                        manager.ActiveEntities.Remove(args.ID);
                                        break;
                                    case COMMANDS.MODIFY:
                                        try
                                        {
                                            Entity e = manager.ActiveEntities[args.ID];
                                            GraphicsComponent graphics = (GraphicsComponent)e.Components[Masks.GRAPHICS];
                                            graphics.Body = body;
                                            graphics.RotationAngle = args.Rotation;
                                        }
                                        catch(KeyNotFoundException)
                                        {
                                            texture = getTexture(args.Type);
                                            manager.addEntity(EFactory.createGraphics(body, args.Rotation, args.ID, texture));
                                            Console.WriteLine("Modify before Add caught and fixed");
                                        }
                                        break;


                                }
                            }
                            receivedData = null;

                            break;
                    }


            }
            
        }

        private Texture2D getTexture(ENTITIES type)
        {
            Texture2D texture;
            switch (type)
            {
                case ENTITIES.AVATAR:
                    texture = manager.getTexture("SpawnTexture");
                    break;
                case ENTITIES.SWORD:
                    texture = manager.getTexture("WeaponTexture");
                    break;
                case ENTITIES.THROWN:
                    texture = manager.getTexture("WeaponTexture");
                    break;
                case ENTITIES.WALL:
                    texture = manager.getTexture("WallTexture");
                    break;
                default:
                    texture = null;
                    break;
                    
            }
            return texture;
        }
        

        private void reset()
        {
            xInput = 0;
            yInput = 0;
            mouseClicked = false;
        }

        

    }
}
