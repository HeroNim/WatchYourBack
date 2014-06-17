﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;

namespace WatchYourBack
{
    /*
     * Takes the state of the game, and modifies what recieves input in that state. For example, in a Playing state, the input should go to the player and the AI should be active,
     * while in a Menu state, the input should go to the menu and the AI should be inactive.
     */
    class GameInputSystem : ESystem, InputSystem
    {
        public GameInputSystem()
            : base(false, true)
        {
            components += AvatarInputComponent.bitMask;
        }

        public override void update()
        {
            //If (state == Playing)
            AvatarInputComponent p1;

            foreach(Entity entity in activeEntities)
            {
                p1 = (AvatarInputComponent)entity.Components[typeof(AvatarInputComponent)];
                
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    p1.MoveRight = true;
                else
                    p1.MoveRight = false;

                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    p1.MoveLeft = true;
                else
                    p1.MoveLeft = false;

                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    p1.MoveUp = true;
                else
                    p1.MoveUp = false;

                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    p1.MoveDown = true;
                else
                    p1.MoveDown = false;
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
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
