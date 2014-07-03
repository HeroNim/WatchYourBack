using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace WatchYourBackServer
{
    /*
     * Checks for collisions between mouseclicks and menu elements, and activates the appropriate response.
     */
    class MenuInputSystem : ESystem, InputSystem
    {

        private bool clickable;
        private bool collided;


         public MenuInputSystem() : base(false, true, 1)
        {
            components += ColliderComponent.bitMask;
            components += ButtonComponent.bitMask;
            clickable = false;
            collided = false;
        }

        public override void update(double lastUpdate)
        {
            MouseState ms = Mouse.GetState();
            collided = false;
                foreach (Entity entity in activeEntities)
                {
                    ColliderComponent collider = (ColliderComponent)entity.Components[typeof(ColliderComponent)];
                    ButtonComponent button = (ButtonComponent)entity.Components[typeof(ButtonComponent)];

                    if (collider.Collider.Contains(ms.X, ms.Y))
                    {
                        collided = true;
                        if (ms.LeftButton == ButtonState.Pressed && clickable == true)
                        {
                            onFire(button.Args);
                            clickable = false;
                        }
                        else if (ms.LeftButton != ButtonState.Pressed)
                            clickable = true;
                        
                    }           
                }
                if (collided == false)
                    clickable = false;
        }

        public event EventHandler inputFired;

        private void onFire(EventArgs e)
        {
            if (inputFired != null)
                inputFired(this, e);
        }
    }
}
