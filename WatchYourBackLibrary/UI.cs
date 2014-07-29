using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBackLibrary
{
    /// <summary>
    /// Contains all the data for the UI of the game, such as time and score displays.
    /// </summary>
    public class UIInfo
    {
        private Entity p1Display;
        private Entity p2Display;
        private Entity timeDisplay;
        private List<Entity> uiElements;


        public UIInfo(GraphicsDevice graphicsDevice) 
        {
            p1Display = EFactory.createDisplay(new Rectangle(graphicsDevice.Viewport.Width/10, 10, 200, 50));
            p2Display = EFactory.createDisplay(new Rectangle((int)((float)graphicsDevice.Viewport.Width / (1.5f)), 10, 200, 50));
            timeDisplay = EFactory.createDisplay(new Rectangle(graphicsDevice.Viewport.Width / 2 - 100, 10, 200, 50));
            uiElements = new List<Entity>();
            uiElements.Add(p1Display);
            uiElements.Add(p2Display);
            uiElements.Add(timeDisplay);

        }

        public void updateUI(int score1, int score2, int time)
        {
            GraphicsComponent g1 = (GraphicsComponent)p1Display.Components[Masks.GRAPHICS];
            GraphicsComponent g2 = (GraphicsComponent)p2Display.Components[Masks.GRAPHICS];
            GraphicsComponent g3 = (GraphicsComponent)timeDisplay.Components[Masks.GRAPHICS];

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
