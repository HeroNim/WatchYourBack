﻿using System;
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
    /// <summary>
    /// The system responsible for updating and sending updates to the server when playing online.
    /// </summary>
    class ClientUpdateSystem : ESystem
    {
        private Dictionary<int, int> entityIDMappings;
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

        private int bufferCount;
        
        List<List<EventArgs>> buffer; //Holds incoming messages

        private NetworkInputArgs toSend;
        private NetClient client;
        private ClientECSManager activeManager;

        public ClientUpdateSystem(NetClient client) : base(false, true, 10)
        {           
            this.client = client;
            bufferCount = 1;
            layer = 0;
            rotationOrigin = Vector2.Zero;
            rotationOffset = Vector2.Zero;
            sourceRectangle = Rectangle.Empty;

            entityIDMappings = new Dictionary<int, int>();
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

        /// <summary>
        /// The update loop updates the status of all the entities the player is responsible for drawing, as well as updating any UI elements as needed. It also sends player input to the server.
        /// </summary>
        /// <param name="gameTime">The update time of the game</param>
        public override void update(TimeSpan gameTime)
        {
            activeManager = (ClientECSManager)manager;
            NetOutgoingMessage om;

            if (InputManager.checkIfActive(this))
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
                if (Keyboard.GetState().IsKeyDown(mappings[KeyBindings.DASH]))
                    dash = true;
                if (ms.LeftButton == ButtonState.Pressed)
                    leftMouseClicked = true;
                if (ms.RightButton == ButtonState.Pressed)
                    rightMouseClicked = true;

                if (Keyboard.GetState().IsKeyDown(mappings[KeyBindings.PAUSE]))
                    onFire(new InputArgs(Inputs.PAUSE));
            }

            toSend = new NetworkInputArgs(client.UniqueIdentifier, xInput, yInput, mouseLocation, leftMouseClicked, rightMouseClicked, activeManager.DrawTime, dash);
            om = client.CreateMessage();
            om.Write(SerializationHelper.Serialize(toSend));
            client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            Console.WriteLine("Sent");
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

                    foreach (EventArgs receivedArgs in receivedData)
                    {
                        if (receivedArgs is NetworkEntityArgs)
                        {
                            NetworkEntityArgs args = (NetworkEntityArgs)receivedArgs;
                            Rectangle body = new Rectangle((int)args.XPos, (int)args.YPos, args.Width, args.Height);
                            Entity e;
                            switch (args.Command)
                            {
                                case EntityCommands.Add:
                                    if (args.SubIndex != null)
                                    {
                                        e = EFactory.createGraphics(body, args.Rotation, rotationOrigin, rotationOffset, args.ID, args.SubIndex, args.Type, layer);
                                        manager.addEntity(e);
                                    }
                                    else
                                    {
                                        e = EFactory.createGraphics(body, args.Rotation, rotationOrigin, rotationOffset, args.ID, sourceRectangle, args.Type, layer);
                                        manager.addEntity(e);
                                    }
                                    if (entityIDMappings.Keys.Contains(args.ID))
                                    {
                                        manager.Entities.Remove(entityIDMappings[args.ID]);
                                        entityIDMappings.Remove(args.ID);
                                    }
                                    entityIDMappings.Add(args.ID, e.ClientID);

                                    break;
                                case EntityCommands.Remove:
                                    manager.Entities.Remove(entityIDMappings[args.ID]);
                                    entityIDMappings.Remove(args.ID);
                                    break;
                                case EntityCommands.Modify:
                                    try
                                    {
                                        e = manager.Entities[entityIDMappings[args.ID]];
                                        GraphicsComponent graphics = (GraphicsComponent)e.Components[Masks.Graphics];
                                        graphics.Body = body;
                                        graphics.RotationAngle = args.Rotation;
                                    }
                                    catch (KeyNotFoundException)
                                    {

                                        Console.WriteLine("Modify before Add caught");
                                    }
                                    catch (ArgumentException)
                                    {
                                        Console.WriteLine("Weird stuff");
                                    }
                                    break;
                            }
                        }
                        else if (receivedArgs is NetworkGameArgs)
                        {
                            NetworkGameArgs args = (NetworkGameArgs)receivedArgs;
                            activeManager.UI.updateUI(args.Scores[0], args.Scores[1], args.Time);
                        }
                        else if (receivedArgs is SoundArgs)
                        {
                            SoundArgs args = (SoundArgs)receivedArgs;
                            onFire(args);
                        }
                        else if (receivedArgs is NetworkUpdateArgs)
                        {
                            NetworkUpdateArgs args = (NetworkUpdateArgs)receivedArgs;
                            switch (args.Command)
                            {
                                case ServerCommands.Disconnect:
                                    Console.WriteLine("Player Disconnected. Game Ending");
                                    onFire(new InputArgs(Inputs.EXIT));
                                    break;
                                case ServerCommands.Lose:
                                    Console.WriteLine("You lose. :(");
                                    onFire(new InputArgs(Inputs.EXIT));
                                    break;
                                case ServerCommands.Win:
                                    Console.WriteLine("You win!");
                                    onFire(new InputArgs(Inputs.EXIT));
                                    break;
                                case ServerCommands.Tie:
                                    Console.WriteLine("You tied. :/");
                                    onFire(new InputArgs(Inputs.EXIT));
                                    break;
                            }
                        }
                    }
                }
                Console.WriteLine(manager.Entities.Count);
                manager.ChangedEntities.Clear();
        }
      
        /// <summary>
        /// Resets the inputs to default values.
        /// </summary>
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
