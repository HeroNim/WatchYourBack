﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WatchYourBackServer
{
    public enum Masks
    {
        TRANSFORM = 1 << 0,
        VELOCITY = 1 << 1,
        GRAPHICS = 1 << 2,
        COLLIDER = 1 << 3,
        PLAYER_INPUT = 1 << 4,
        TILE = 1 << 5,
        LEVEL = 1 << 6,
        BUTTON = 1 << 7,
        WIELDER = 1 << 8,
        DEBUG = 1 << 9,
        TIMER = 1 << 10,
        LINE_COLLIDER = 1 << 11,
        WEAPON = 1 << 12,
        ALLEGIANCE = 1 << 13
    };

    /* Components that make up each entity. Ideally contains only data and the methods needed to access them; 
     * however, for smaller programs and tightly coupled components, it may be simpler to include the logic in the components themselves.
     * Also contains the value of the component-specific bitmask used to quickly lookup and compare components and entities.
     */
    public abstract class EComponent
    {
        public abstract Masks Mask { get; }

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
