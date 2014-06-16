using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{

    /*
     * Defines the behaviour of the player's movement
     */
    class AvatarInputSystem : ESystem
    {
        public AvatarInputSystem()
            : base(false, true)
        {
            components += AvatarInputComponent.bitMask;
            components += VelocityComponent.bitMask;
        }

        public override void update()
        {
            foreach (Entity entity in activeEntities)
            {
                AvatarInputComponent p1 = (AvatarInputComponent)entity.Components[typeof(AvatarInputComponent)];
                VelocityComponent v1 = (VelocityComponent)entity.Components[typeof(VelocityComponent)];
                if (p1.MoveDown)
                    v1.Y = 5;
                else if (p1.MoveUp)
                    v1.Y = -5;
                else
                    v1.Y = 0;

                if (p1.MoveRight)
                    v1.X = 5;
                else if (p1.MoveLeft)
                    v1.X = -5;
                else
                    v1.X = 0;
                    
            }
        }
    }
}
