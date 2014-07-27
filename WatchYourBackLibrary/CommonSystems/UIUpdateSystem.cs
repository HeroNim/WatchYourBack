using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    public class UIUpdateSystem : ESystem
    {
        LevelComponent level;
        UIInfo ui;

        public UIUpdateSystem(UIInfo ui)
            : base(false, true, 9)
        {
            components += (int)Masks.PLAYER_INFO;
            this.ui = ui;
            
        }
        public override void update(TimeSpan gameTime)
        {
            level = manager.LevelInfo;
            PlayerInfoComponent p1 = (PlayerInfoComponent)level.Avatars[0].Components[Masks.PLAYER_INFO];
            PlayerInfoComponent p2 = (PlayerInfoComponent)level.Avatars[1].Components[Masks.PLAYER_INFO];
            ui.updateUI(p1.Score, p2.Score, level.GameTime);                                 
        }

       

    }
}
