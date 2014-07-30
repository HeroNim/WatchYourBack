using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using WatchYourBackLibrary;

namespace WatchYourBackLibrary
{

  

    /// <summary>
    /// The system responsible for adjusting properties of the avatars given various inputs by the players
    /// </summary>
    public class AvatarInputSystem : ESystem
    {
        public AvatarInputSystem()
            : base(false, true, 3)
        {
            components += (int)Masks.PlayerInput;
            components += (int)Masks.Velocity;
            components += (int)Masks.Transform;
            components += (int)Masks.Wielder;
        }

        public override void update(TimeSpan gameTime)
        {
            foreach (Entity entity in activeEntities)
            {
                float xDir = 0;
                float yDir = 0;
                AvatarInputComponent input = (AvatarInputComponent)entity.Components[Masks.PlayerInput];
                StatusComponent status = (StatusComponent)entity.Components[Masks.Status];
                VelocityComponent velocity = (VelocityComponent)entity.Components[Masks.Velocity];
                TransformComponent transform = (TransformComponent)entity.Components[Masks.Transform];
                WielderComponent wielder = (WielderComponent)entity.Components[Masks.Wielder];

                Vector2 rotationVector = HelperFunctions.AngleToVector(transform.Rotation);
                float relativeAngle = HelperFunctions.Angle(velocity.Velocity, rotationVector);

                status.IterateTimers((float)gameTime.TotalMilliseconds);

                if (input.Dash == true)
                    status.ApplyStatus(Status.Dashing, 200f, 1000f);
                input.Dash = false;
                
                if (relativeAngle > Math.PI / 2)
                    velocity.VelocityModifier = 1.0f / 2.0f;
                else
                    velocity.VelocityModifier = 1;

                if (status.getDuration(Status.Dashing) > 0)
                    velocity.VelocityModifier *= 2;
                    
                velocity.Velocity = Vector2.Zero;
                velocity.RotationSpeed = 0;

                if (status.getDuration(Status.Paralyzed) <= 0)
                {
                    if (input.MoveY == 1)
                        velocity.Y = 4;
                    else if (input.MoveY == -1)
                        velocity.Y = -4;

                    if (input.MoveX == 1)
                        velocity.X = 4;
                    else if (input.MoveX == -1)
                        velocity.X = -4;

                    if (input.LookX > -1 && input.LookY > -1)
                    {
                        xDir = input.LookX - transform.Center.X;
                        yDir = input.LookY - transform.Center.Y;
                    }

                    Vector2 dir = new Vector2(xDir, yDir);
                    dir.Normalize();
                    transform.LookDirection = dir;
                    float angle = transform.LookAngle - transform.Rotation;
                    angle = HelperFunctions.Normalize(angle);
                    
                    if (!wielder.hasWeapon)
                    {
                        if (angle > Math.PI)
                            velocity.RotationSpeed = -5;
                        if (angle < Math.PI)
                            velocity.RotationSpeed = 5;
                        if (angle < 0.1f || angle > (float)Math.PI * 2 - 0.1f)
                            velocity.RotationSpeed = 0;
                    }
                }


                //if(entity.hasComponent(Masks.WIELDER))
                //{
                //    if (((WielderComponent)entity.Components[Masks.WIELDER]).hasWeapon)
                //    {
                //        Entity weapon = ((WielderComponent)entity.Components[Masks.WIELDER]).Weapon;
                //        VelocityComponent weaponVelocityComponent = (VelocityComponent)weapon.Components[Masks.VELOCITY];
                //        weaponVelocityComponent.Y = velocity.Y;
                //        weaponVelocityComponent.X = velocity.X;
                //    }
                //}
            }
        }
    }
}
