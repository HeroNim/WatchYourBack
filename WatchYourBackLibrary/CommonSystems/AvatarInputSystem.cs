using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using WatchYourBackLibrary;

namespace WatchYourBackLibrary
{

    /*
     * Defines the behaviour of the player's movement
     */
    public class AvatarInputSystem : ESystem
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
                AvatarInputComponent avatarInput = (AvatarInputComponent)entity.Components[Masks.PLAYER_INPUT];
                VelocityComponent velocityComponent = (VelocityComponent)entity.Components[Masks.VELOCITY];
                float xVel = 0;
                float yVel = 0;

                if (avatarInput.MoveY == 1)
                    yVel = 5;
                else if (avatarInput.MoveY == -1)
                    yVel = -5;
                else
                    yVel = 0;

                if (avatarInput.MoveX == 1)
                    xVel = 5;
                else if (avatarInput.MoveX == -1)
                    xVel = -5;
                else
                    xVel = 0;

                velocityComponent.Y = yVel;
                velocityComponent.X = xVel;

                if(entity.hasComponent(Masks.WIELDER))
                {
                    if (((WielderComponent)entity.Components[Masks.WIELDER]).hasWeapon)
                    {
                        Entity weapon = ((WielderComponent)entity.Components[Masks.WIELDER]).Weapon;
                        VelocityComponent weaponVelocityComponent = (VelocityComponent)weapon.Components[Masks.VELOCITY];
                        weaponVelocityComponent.Y = yVel;
                        weaponVelocityComponent.X = xVel;
                    }
                }
            }
        }
    }
}
