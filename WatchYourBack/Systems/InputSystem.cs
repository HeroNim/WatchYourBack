using System;
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
    class InputSystem : ESystem
    {
        public InputSystem()
            : base(false, true)
        {
            components += PlayerInputComponent.bitMask;
        }

        public override void update()
        {
            //If (state == Playing)
            PlayerInputComponent p1;

            foreach(Entity entity in activeEntities)
            {
                p1 = (PlayerInputComponent)entity.Components[typeof(PlayerInputComponent)];
                
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
            }
                    
                
        }
        

    }
}
