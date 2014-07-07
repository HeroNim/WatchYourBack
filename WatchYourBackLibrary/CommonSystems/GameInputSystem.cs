using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using WatchYourBackLibrary;

namespace WatchYourBack
{

    public enum KeyBindings
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
        PAUSE,
        ATTACK
    }
    /*
     * Takes the state of the game, and modifies what recieves input in that state. For example, in a Playing state, the input should go to the player and the AI should be active,
     * while in a Menu state, the input should go to the menu and the AI should be inactive.
     */
    public class GameInputSystem : ESystem, InputSystem
    {
        private Dictionary<KeyBindings, Keys> mappings;
        public GameInputSystem()
            : base(false, true, 2)
        {
            components += (int)Masks.PLAYER_INPUT;
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
            //If (state == Playing)
            AvatarInputComponent p1;

            foreach(Entity entity in activeEntities)
            {
                p1 = (AvatarInputComponent)entity.Components[Masks.PLAYER_INPUT];
                MouseState ms = Mouse.GetState();
                
                if (Keyboard.GetState().IsKeyDown(mappings[KeyBindings.RIGHT]))
                    p1.MoveX = 1;
                else if (Keyboard.GetState().IsKeyDown(mappings[KeyBindings.LEFT]))
                    p1.MoveX = -1;
                else
                    p1.MoveX = 0;

                if (Keyboard.GetState().IsKeyDown(mappings[KeyBindings.UP]))
                    p1.MoveY = -1;
                else if (Keyboard.GetState().IsKeyDown(mappings[KeyBindings.DOWN]))
                    p1.MoveY = 1;
                else
                    p1.MoveY = 0;

                if (Keyboard.GetState().IsKeyDown(mappings[KeyBindings.PAUSE]))
                    onFire(new InputArgs(Inputs.PAUSE));   
                if(ms.LeftButton == ButtonState.Pressed)
                    p1.Attack = true;
                p1.LookX = ms.X;
                p1.LookY = ms.Y;
            }
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
