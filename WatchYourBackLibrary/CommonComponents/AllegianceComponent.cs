using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatchYourBackLibrary;

namespace WatchYourBackLibrary
{
    public enum Allegiance
    {
        PLAYER_1 = 1,
        PLAYER_2 = 2
    }

    //A component that tells the game who's 'team' the entity belongs to.
    public class AllegianceComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.ALLEGIANCE; } }
        public override Masks Mask { get { return Masks.ALLEGIANCE; } }


        private Allegiance ownedBy;

        public AllegianceComponent(Allegiance owner)
        {
            ownedBy = owner;
        }

        public Allegiance Owner
        {
            get { return ownedBy; }
            set { ownedBy = value; }
        }

       
    }
}
