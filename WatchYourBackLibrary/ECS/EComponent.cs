using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WatchYourBackLibrary
{
    public enum Masks
    {
        Transform = 1 << 0,
        Velocity = 1 << 1,
        Graphics = 1 << 2,
        Collider = 1 << 3,
        PlayerInput = 1 << 4,
        Tile = 1 << 5,
        Level = 1 << 6,
        Button = 1 << 7,
        Wielder = 1 << 8,
        Audio = 1 << 9,
        SoundEffect = 1 << 10,
        LineCollider = 1 << 11,
        Weapon = 1 << 12,
        Allegiance = 1 << 13,
        PlayerHitbox = 1 << 14,
        RectangleCollider = 1 << 15,
        PlayerInfo = 1 << 16,
        CircleCollider = 1 << 17,
        Status = 1 << 18,
    };

   

    /// <summary>
    /// Components that make up each entity. Ideally contains only data and the methods needed to access them.
    /// Also contains the value of the component-specific bitmask used to quickly lookup and compare components and entities.
    /// When creating components, the mask is the identity of each component, while the bitmask is the number representing the component itself.
    /// </summary>
    /// <remarks>
    /// For smaller programs and tightly coupled components, it may be simpler to include the logic in the components themselves.
    /// If you wish to have a component which is the 'subclass' of another component, add the value of the parent's mask to the subclass's bitmask.
    /// </remarks>
    public abstract class EComponent
    {
        public abstract Masks Mask { get; }
        public abstract int BitMask { get; }

        private Entity entity;


        //Set the component to an entity
        public void setEntity(Entity entity)
        {
            this.entity = entity;
        }

        //Get the entity the component is attached to
        public Entity getEntity()
        {
            return this.entity;
        }


    }
}
