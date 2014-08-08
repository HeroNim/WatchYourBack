using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    /// <summary>
    /// A system used to update elements of the game's UI, such as the score and timer displays
    /// </summary>
    public class UIUpdateSystem : ESystem
    {
        LevelInfo level;
        UIInfo ui;

        public UIUpdateSystem(UIInfo ui)
            : base(false, true, 9)
        {
            components += (int)Masks.PlayerInfo;
            this.ui = ui;
            
        }
        public override void update(TimeSpan gameTime)
        {
            level = manager.LevelInfo;
            PlayerInfoComponent p1 = (PlayerInfoComponent)level.Avatars[0].Components[Masks.PlayerInfo];
            PlayerInfoComponent p2 = (PlayerInfoComponent)level.Avatars[1].Components[Masks.PlayerInfo];
            ui.updateUI(p1.Score, p2.Score, level.GameTime);                                 
        }      
    }
}
