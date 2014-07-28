using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBackLibrary
{

    /// <summary>
    /// The component which holds all the information about the player, such as their avatar, allegiance, and score.
    /// </summary>
    public class PlayerInfoComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.PLAYER_INFO; } }
        public override Masks Mask { get { return Masks.PLAYER_INFO; } }

        private Allegiance playerNum;
        private Entity avatar;
        private int score;

        public PlayerInfoComponent(Allegiance player)
        {
            playerNum = player;
        }

        public Allegiance PlayerNumber
        {
            get { return playerNum; }
        }

        public Entity Avatar
        {
            get { return avatar; }
            set { avatar = value; }
        }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }


    }
}
