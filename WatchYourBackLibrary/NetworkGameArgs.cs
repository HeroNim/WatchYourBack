using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{


    [Serializable()]
    public class NetworkGameArgs : EventArgs
    {
        private int[] scores;
        private int time;

        public NetworkGameArgs(int[] scores, int time)
        {
            this.scores = scores;
            this.time = time;
        }

        public int[] Scores { get { return scores; } }
        public int Time { get { return time; } }

    }
}