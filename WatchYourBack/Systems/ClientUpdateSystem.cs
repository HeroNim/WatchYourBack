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
        private bool leftMouseClicked;
        private bool rightMouseClicked;
        List<NetworkEntityArgs> receivedData;
        Texture2D texture;
        float layer;
        Vector2 rotationOrigin;
        Vector2 rotationOffset;
        
        int bufferCount;
        
        List<List<NetworkEntityArgs>> buffer; //Holds incoming messages



        private NetworkInputArgs toSend;
        private NetClient client;


        public ClientUpdateSystem(NetClient client) : base(false, true, 10)
        {
            
            this.client = client;
            bufferCount = 1;
            texture = null;
            layer = 0;
            rotationOrigin = Vector2.Zero;
            rotationOffset = Vector2.Zero;

            receivedData = new List<NetworkEntityArgs>();
            buffer = new List<List<NetworkEntityArgs>>();
            mappings = new Dictionary<KeyBindings, Keys>();
            mappings.Add(KeyBindings.LEFT, Keys.Left);
            mappings.Add(KeyBindings.RIGHT, Keys.Right);
            mappings.Add(KeyBindings.UP, Keys.Up);
            mappings.Add(KeyBindings.DOWN, Keys.Down);
            mappings.Add(KeyBindings.PAUSE, Keys.Escape);
            mappings.Add(KeyBindings.ATTACK, Keys.Space);

        }

        public override void update(TimeSpan gameTime)
        {
            NetOutgoingMessage om;

           
    

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
                leftMouseClicked = true;
            if (ms.RightButton == ButtonState.Pressed)
                rightMouseClicked = true;

            toSend = new NetworkInputArgs(client.UniqueIdentifier, xInput, yInput, mouseLocation, leftMouseClicked, rightMouseClicked, manager.DrawTime);
            om = client.CreateMessage();
            om.Write(SerializationHelper.Serialize(toSend));
            client.SendMessage(om, NetDeliveryMethod.ReliableOrdered); 
            reset();
            

            NetIncomingMessage msg;


            while ((msg = client.ReadMessage()) != null)
            {
               
            
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.Data:                     
                        buffer.Add(SerializationHelper.DeserializeObject<List<NetworkEntityArgs>>(msg.ReadBytes(msg.LengthBytes)));
                        client.Recycle(msg);
                        break;
                }
            }

            while (buffer.Count != 0 && bufferCount >= 0)
            {
                List<NetworkEntityArgs> receivedData = buffer[0];
                buffer.RemoveAt(0);
                


                foreach (NetworkEntityArgs args in receivedData)
                {
                    Rectangle body = new Rectangle((int)args.XPos, (int)args.YPos, args.Width, args.Height);
                    switch (args.Command)
                    {
                        case COMMANDS.ADD:
                            getGraphics(args);
                            
                            manager.addEntity(EFactory.createGraphics(body, args.Rotation, rotationOrigin, rotationOffset, args.ID, texture, args.Type, layer));
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
                            catch (KeyNotFoundException)
                            {
            
                                Console.WriteLine("Modify before Add caught");
                            }
                            break;
                    }
                }
            }
            manager.ChangedEntities.Clear();
                
            
        }

        private void getGraphics(NetworkEntityArgs args)
        {
            switch (args.Type)
            {
                case ENTITIES.AVATAR:
                    texture = manager.getTexture("PlayerTexture");
                    layer = 1;
                    rotationOrigin = new Vector2(texture.Width/2, texture.Height/2);
                    rotationOffset = new Vector2(args.Width / 2, args.Height / 2); 
                    break;
                case ENTITIES.SWORD:
                    texture = manager.getTexture("SwordTexture");
                    layer = 0;
                    rotationOrigin = new Vector2(texture.Width/2, texture.Height);
                    rotationOffset = Vector2.Zero; 
                    break;
                case ENTITIES.THROWN:
                    texture = manager.getTexture("ThrownTexture");
                    layer = 0;
                    rotationOrigin = Vector2.Zero;
                    rotationOffset = Vector2.Zero; 
                    break;
                case ENTITIES.WALL:
                    texture = manager.getTexture("WallTexture");
                    layer = 1;
                    rotationOrigin = Vector2.Zero;
                    rotationOffset = Vector2.Zero; 
                    break;
                default:
                    texture = null;
                    layer = 1;
                    rotationOrigin = Vector2.Zero;
                    rotationOffset = Vector2.Zero; 
                    break;

            }
            
        }
        

        private void reset()
        {
            xInput = 0;
            yInput = 0;
            leftMouseClicked = false;
            rightMouseClicked = false;

        }

        

    }
}
