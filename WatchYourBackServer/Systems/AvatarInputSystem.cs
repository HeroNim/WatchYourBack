using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBackServer
{

    /*
     * Defines the behaviour of the player's movement
     */
    class AvatarInputSystem : ESystem
    {
        public AvatarInputSystem()
            : base(false, true, 3)
        {
            components += AvatarInputComponent.bitMask;
            components += VelocityComponent.bitMask;
        }

        public override void update(GameTime gameTime)
        {
            foreach (Entity entity in activeEntities)
            {
                AvatarInputComponent avatarInput = (AvatarInputComponent)entity.Components[typeof(AvatarInputComponent)];
                VelocityComponent velocityComponent = (VelocityComponent)entity.Components[typeof(VelocityComponent)];
                float xVel = 0;
                float yVel = 0;

                if (avatarInput.MoveDown)
                    yVel = 5;
                else if (avatarInput.MoveUp)
                    yVel = -5;
                else
                    yVel = 0;

                if (avatarInput.MoveRight)
                    xVel = 5;
                else if (avatarInput.MoveLeft)
                    xVel = -5;
                else
                    xVel = 0;

                velocityComponent.Y = yVel;
                velocityComponent.X = xVel;

                if(entity.hasComponent(Masks.WIELDER))
                {
                    if (((WielderComponent)entity.Components[typeof(WielderComponent)]).hasWeapon)
                    {
                        Entity weapon = ((WielderComponent)entity.Components[typeof(WielderComponent)]).Weapon;
                        VelocityComponent weaponVelocityComponent = (VelocityComponent)weapon.Components[typeof(VelocityComponent)];
                        weaponVelocityComponent.Y = yVel;
                        weaponVelocityComponent.X = xVel;
                    }
                }
            }
        }
    }
}
