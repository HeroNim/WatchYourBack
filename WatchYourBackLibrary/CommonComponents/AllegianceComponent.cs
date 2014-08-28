using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatchYourBackLibrary;

namespace WatchYourBackLibrary
{
    public enum Allegiance
    {
        Player1 = 1,
        Player2 = 2,
        Neutral = 3

    }

    /// <summary>
    /// The component that tells the game who's 'team' the entity belongs to. Largely used to determine which entities should interact
    /// with each other.
    /// </summary>
    public class AllegianceComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.Allegiance; } }
        public override Masks Mask { get { return Masks.Allegiance; } }

        private Allegiance myAllegiance;

        public AllegianceComponent(Allegiance owner)
        {
            myAllegiance = owner;
        }

        public Allegiance Allegiance
        {
            get { return myAllegiance; }
            set { myAllegiance = value; }
        }      
    }
}
