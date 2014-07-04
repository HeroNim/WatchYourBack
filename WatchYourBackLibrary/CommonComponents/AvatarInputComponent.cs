using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchYourBackLibrary;

namespace WatchYourBackLibrary
{
    /*
     * Holds the data containing the current inputs to the avatar
     */
    public class AvatarInputComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.PLAYER_INPUT; } }
        public override Masks Mask { get { return Masks.PLAYER_INPUT; } }

        
        private int moveX;
        private int moveY;

        public AvatarInputComponent()
        {
            
            moveX = 0;
            moveY = 0;
        }

       
        public int MoveX { get { return moveX; } set { moveX = value; } }
        public int MoveY { get { return moveY; } set { moveY = value; } }

        public void Reset()
        {
            
            moveX = 0;
            moveY = 0;
        }
    }
}
