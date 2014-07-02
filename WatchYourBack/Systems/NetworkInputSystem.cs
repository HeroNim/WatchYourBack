using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WatchYourBack
{
    class NetworkIOSystem : ESystem, InputSystem
    {
        private Dictionary<KeyBindings, Keys> mappings;
        private int xInput;
        private int yInput;
        private Vector2 mouseLocation;
        private bool mouseClicked;

        private NetworkArgs toSend;


        public NetworkIOSystem() : base(false, true, 1)
        {
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
            onFire(toSend);
            reset();
            
        }

        public event EventHandler inputFired;


        private void onFire(EventArgs e)
        {
            if (inputFired != null)
                inputFired(this, e);
        }

        private void reset()
        {
            xInput = 0;
            yInput = 0;
            mouseClicked = false;
        }

    }
}
