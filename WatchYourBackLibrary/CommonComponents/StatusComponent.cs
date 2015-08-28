using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBackLibrary
{
    public enum Status
    {
        None,
        Paralyzed,
        Dashing
    }

    /// <summary>
    /// The component which holds all the data on the status effects on an avatar, including negative affects such as paralysis,
    /// and positive effects such as buffs.
    /// </summary>
    public class StatusComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.Status; } }
        public override Masks Mask { get { return Masks.Status; } }

        private Dictionary<Status, float[]> currentStatus;
        private List<Status> keys;

        public StatusComponent()
        {
            currentStatus = new Dictionary<Status, float[]>();
            
            foreach (Status status in Enum.GetValues(typeof(Status)))
                currentStatus.Add(status, new float[2] { 0, 0 });
            keys = new List<Status>(currentStatus.Keys);
        }

        public void ApplyStatus(Status status, float statusDuration, float cooldown)
        {
            if (GetCooldown(status) <= 0)
            {
                currentStatus[status][0] = statusDuration;
                currentStatus[status][1] = cooldown;
            }
        }
      
        public Dictionary<Status, float[]> CurrentStatus
        {
            get { return currentStatus; }
        }

        public void IterateTimers (float time)
        {

            foreach (Status status in keys)
            {
                currentStatus[status][0] -= time;
                currentStatus[status][1] -= time;
            }
        }

        public float GetDuration (Status status)
        {
            return currentStatus[status][0];
        }

        public float GetCooldown(Status status)
        {
            return currentStatus[status][1];
        }       
    }
}
