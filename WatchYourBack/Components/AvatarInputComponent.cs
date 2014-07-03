using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    /*
     * Holds the data containing the current inputs to the avatar
     */
    class AvatarInputComponent : EComponent
    {
        public readonly static int bitMask = (int)Masks.PLAYER_INPUT;
        public override Masks Mask { get { return Masks.PLAYER_INPUT; } }

        private bool moveUp;
        private bool moveDown;
        private bool moveLeft;
        private bool moveRight;

        public AvatarInputComponent()
        {
            moveUp = false;
            moveDown = false;
            moveLeft = false;
            moveRight = false;
        }

        public bool MoveUp { get { return moveUp; } set { moveUp = value; } }
        public bool MoveDown { get { return moveDown; } set { moveDown = value; } }
        public bool MoveLeft { get { return moveLeft; } set { moveLeft = value; } }
        public bool MoveRight { get { return moveRight; } set { moveRight = value; } }

        public void Reset()
        {
            moveUp = false;
            moveDown = false;
            moveRight = false;
            moveLeft = false;
        }
    }
}
