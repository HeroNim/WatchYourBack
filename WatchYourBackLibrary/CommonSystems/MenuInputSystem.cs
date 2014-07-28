using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using WatchYourBackLibrary;

namespace WatchYourBack
{
    
    /// <summary>
    ///  Checks for collisions between mouseclicks and menu elements, and activates the appropriate response based on which element was clicked.
    /// </summary>
    public class MenuInputSystem : ESystem, InputSystem
    {
        public MenuInputSystem()
            : base(false, true, 1)
        {
            components += (int)Masks.RECTANGLE_COLLIDER;
            components += (int)Masks.BUTTON;
        }

        public override void update(TimeSpan gameTime)
        {
            MouseState ms = Mouse.GetState();

            foreach (Entity entity in activeEntities)
            {
                RectangleColliderComponent collider = (RectangleColliderComponent)entity.Components[Masks.RECTANGLE_COLLIDER];
                ButtonComponent button = (ButtonComponent)entity.Components[Masks.BUTTON];
                GraphicsComponent g = (GraphicsComponent)entity.Components[Masks.GRAPHICS];

                if (collider.Collider.Contains(ms.X, ms.Y) && ms.LeftButton == ButtonState.Pressed)
                {
                    button.Focused = true;
                }

                else if (ms.LeftButton == ButtonState.Released && button.Focused && collider.Collider.Contains(ms.X, ms.Y))
                {
                    onFire(button.Args);
                    button.Focused = false;
                }
                else
                    button.Focused = false;

                
                if (button.Focused)
                    g.Sprites["Frame"].Visible = false;
                else
                    g.Sprites["Frame"].Visible = true;
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
