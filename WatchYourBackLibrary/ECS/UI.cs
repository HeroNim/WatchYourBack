using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WatchYourBackLibrary
{
    /// <summary>
    /// Contains all the data for the UI of the game, such as time and score displays.
    /// </summary>
    public class UI
    {
        private Entity p1Display;
        private Entity p2Display;
        private Entity timeDisplay;
        private List<Entity> uiElements;

        public UI(GraphicsDevice graphicsDevice) 
        {
            ContentManager content = GameServices.GetService<ContentManager>();
            p1Display = EFactory.CreateDisplay(new Rectangle(graphicsDevice.Viewport.Width/10, 10, 200, 50), content.Load<SpriteFont>("Fonts/TestFont"));
            p2Display = EFactory.CreateDisplay(new Rectangle((int)((float)graphicsDevice.Viewport.Width / (10f/9f)), 10, 200, 50), content.Load<SpriteFont>("Fonts/TestFont"));
            timeDisplay = EFactory.CreateDisplay(new Rectangle(graphicsDevice.Viewport.Width / 2 - 15, 10, 200, 50), content.Load<SpriteFont>("Fonts/TestFont"));
            uiElements = new List<Entity>();
            uiElements.Add(p1Display);
            uiElements.Add(p2Display);
            uiElements.Add(timeDisplay);
        }

        public void UpdateUI(int score1, int score2, int time)
        {
            GraphicsComponent g1 = (GraphicsComponent)p1Display.Components[Masks.Graphics];
            GraphicsComponent g2 = (GraphicsComponent)p2Display.Components[Masks.Graphics];
            GraphicsComponent g3 = (GraphicsComponent)timeDisplay.Components[Masks.Graphics];

            g1.Text = score1.ToString();
            g2.Text = score2.ToString();
            g3.Text = time.ToString();
        }

        public List<Entity> UIElements
        {
            get { return uiElements; }
        }
    }
}
