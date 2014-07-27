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
        private bool dash;
        List<EventArgs> receivedData;
        float layer;
        Vector2 rotationOrigin;
        Vector2 rotationOffset;
        Rectangle sourceRectangle;
        
        int bufferCount;
        
        List<List<EventArgs>> buffer; //Holds incoming messages



        private NetworkInputArgs toSend;
        private NetClient client;
        private ClientECSManager thisManager;


        public ClientUpdateSystem(NetClient client) : base(false, true, 10)
        {
            
            this.client = client;
            bufferCount = 1;
            layer = 0;
            rotationOrigin = Vector2.Zero;
            rotationOffset = Vector2.Zero;
            sourceRectangle = Rectangle.Empty;


            receivedData = new List<EventArgs>();
            buffer = new List<List<EventArgs>>();
            mappings = new Dictionary<KeyBindings, Keys>();
            mappings.Add(KeyBindings.LEFT, Keys.A);
            mappings.Add(KeyBindings.RIGHT, Keys.D);
            mappings.Add(KeyBindings.UP, Keys.W);
            mappings.Add(KeyBindings.DOWN, Keys.S);
            mappings.Add(KeyBindings.PAUSE, Keys.Escape);
            mappings.Add(KeyBindings.DASH, Keys.Space);

            
        }

        public override void update(TimeSpan gameTime)
        {
            thisManager = (ClientECSManager)manager;
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
            if (Keyboard.GetState().IsKeyDown(mappings[KeyBindings.DASH]))
                dash = true;
            if (ms.LeftButton == ButtonState.Pressed)
                leftMouseClicked = true;
            if (ms.RightButton == ButtonState.Pressed)
                rightMouseClicked = true;

            toSend = new NetworkInputArgs(client.UniqueIdentifier, xInput, yInput, mouseLocation, leftMouseClicked, rightMouseClicked, manager.DrawTime, dash);
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
                        buffer.Add(SerializationHelper.DeserializeObject<List<EventArgs>>(msg.ReadBytes(msg.LengthBytes)));
                        client.Recycle(msg);
                        break;
                }
            }

            while (buffer.Count != 0 && bufferCount >= 0)
            {
                List<EventArgs> receivedData = buffer[0];
                buffer.RemoveAt(0);



                foreach (EventArgs arg in receivedData)
                {
                    if (arg is NetworkEntityArgs)
                    {
                        NetworkEntityArgs args = (NetworkEntityArgs)arg;
                        Rectangle body = new Rectangle((int)args.XPos, (int)args.YPos, args.Width, args.Height);
                        switch (args.Command)
                        {
                            case COMMANDS.ADD:
                                if (args.SubIndex != null)
                                    manager.addEntity(EFactory.createGraphics(body, args.Rotation, rotationOrigin, rotationOffset, args.ID, args.SubIndex, args.Type, layer));
                                else
                                    manager.addEntity(EFactory.createGraphics(body, args.Rotation, rotationOrigin, rotationOffset, args.ID, sourceRectangle, args.Type, layer));
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
                    else if(arg is NetworkGameArgs)
                    {
                        NetworkGameArgs args = (NetworkGameArgs)arg;
                        thisManager.UI.updateUI(args.Scores[0], args.Scores[1], args.Time);
                    }
                }
            }
            manager.ChangedEntities.Clear();
                
            
        }
      

        private void reset()
        {
            xInput = 0;
            yInput = 0;
            leftMouseClicked = false;
            rightMouseClicked = false;
            dash = false;


        }

        

    }
}
