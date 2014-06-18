using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;

namespace WatchYourBack
{

    public enum KeyBindings
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
        ESCAPE
    }
    /*
     * Takes the state of the game, and modifies what recieves input in that state. For example, in a Playing state, the input should go to the player and the AI should be active,
     * while in a Menu state, the input should go to the menu and the AI should be inactive.
     */
    class GameInputSystem : ESystem, InputSystem
    {
        private Dictionary<KeyBindings, Keys> mappings;
        public GameInputSystem()
            : base(false, true, 2)
        {
            components += AvatarInputComponent.bitMask;
            mappings = new Dictionary<KeyBindings, Keys>();
            mappings.Add(KeyBindings.LEFT, Keys.Left);
            mappings.Add(KeyBindings.RIGHT, Keys.Right);
            mappings.Add(KeyBindings.UP, Keys.Up);
            mappings.Add(KeyBindings.DOWN, Keys.Down);
            mappings.Add(KeyBindings.ESCAPE, Keys.Escape);
        }

        public override void update()
        {
            //If (state == Playing)
            AvatarInputComponent p1;

            foreach(Entity entity in activeEntities)
            {
                p1 = (AvatarInputComponent)entity.Components[typeof(AvatarInputComponent)];
                
                if (Keyboard.GetState().IsKeyDown(mappings[KeyBindings.RIGHT]))
                    p1.MoveRight = true;
                else
                    p1.MoveRight = false;

                if (Keyboard.GetState().IsKeyDown(mappings[KeyBindings.LEFT]))
                    p1.MoveLeft = true;
                else
                    p1.MoveLeft = false;

                if (Keyboard.GetState().IsKeyDown(mappings[KeyBindings.UP]))
                    p1.MoveUp = true;
                else
                    p1.MoveUp = false;

                if (Keyboard.GetState().IsKeyDown(mappings[KeyBindings.DOWN]))
                    p1.MoveDown = true;
                else
                    p1.MoveDown = false;
                if (Keyboard.GetState().IsKeyDown(mappings[KeyBindings.ESCAPE]))
                    onFire(new InputArgs(Inputs.PAUSE));
            }
        }

        public event EventHandler inputFired;

        private void onFire(EventArgs e)
        {
            if (inputFired != null)
                inputFired(this, e);
        }
        

    }
}
