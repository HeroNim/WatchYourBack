using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using WatchYourBackLibrary;

namespace WatchYourBack
{
    /*
     * Checks for collisions between mouseclicks and menu elements, and activates the appropriate response.
     */
    public class MenuInputSystem : ESystem, InputSystem
    {



         public MenuInputSystem() : base(false, true, 1)
        {
            components += (int)Masks.COLLIDER;
            components += (int)Masks.BUTTON;

        }

         public override void update(TimeSpan gameTime)
        {
            MouseState ms = Mouse.GetState();
            
                foreach (Entity entity in activeEntities)
                {
                    ColliderComponent collider = (ColliderComponent)entity.Components[Masks.COLLIDER];
                    ButtonComponent button = (ButtonComponent)entity.Components[Masks.BUTTON];
                    GraphicsComponent graphics = (GraphicsComponent)entity.Components[Masks.GRAPHICS];

                    if (collider.Collider.Contains(ms.X, ms.Y) && ms.LeftButton == ButtonState.Pressed)
                        button.Focused = true;

                    else if (ms.LeftButton == ButtonState.Released && button.Focused && collider.Collider.Contains(ms.X, ms.Y))
                    {
                        onFire(button.Args);
                        button.Focused = false;
                    }
                    else
                        button.Focused = false;

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
