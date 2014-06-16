using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;

namespace WatchYourBack
{
    /*
     * Checks for collisions between mouseclicks and menu elements, and activates the appropriate response.
     */
    class MenuInputSystem : ESystem
    {

        private bool clickable;
        private bool collided;
        private World menu;

         public MenuInputSystem(World menu) : base(false, true)
        {
            components += ColliderComponent.bitMask;
            components += ButtonComponent.bitMask;
            this.menu = menu;
            clickable = false;
            collided = false;
        }

        public override void update()
        {
            MouseState ms = Mouse.GetState();
            collided = false;
                foreach (Entity entity in activeEntities)
                {
                    ColliderComponent collider = (ColliderComponent)entity.Components[typeof(ColliderComponent)];
                    ButtonComponent button = (ButtonComponent)entity.Components[typeof(ButtonComponent)];
                    GraphicsComponent graphics = (GraphicsComponent)entity.Components[typeof(GraphicsComponent)];

                    if (collider.Collider.Contains(ms.X, ms.Y))
                    {
                        collided = true;
                        if (ms.LeftButton == ButtonState.Pressed && clickable == true)
                        {
                            onClick(button.Args);
                            clickable = false;
                        }
                        else if (ms.LeftButton != ButtonState.Pressed)
                            clickable = true;
                        
                    }           
                }
                if (collided == false)
                    clickable = false;
        }

        public event EventHandler buttonClicked;

        private void onClick(EventArgs e)
        {
            if (buttonClicked != null)
                buttonClicked(menu, e);
        }
    }
}
