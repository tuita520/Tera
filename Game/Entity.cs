﻿using System;

namespace Tera.Game
{
    // An object with an Id that can be spawned or deswpawned in the game world
    public class Entity : IEquatable<object>
    {
        public Entity(EntityId id)
        {
            Id = id;
        }
        public Entity(EntityId id, Vector3f position, Angle heading, Vector3f? finish = null, int speed=0, long time=0)
        {
            Id = id;
            Position = position;
            Heading = heading;
            Finish = finish??Position;
            Speed = speed;
            StartTime = time;
        }

        public EntityId Id { get; }

        public override string ToString()
        {
            var result = $"{GetType().Name} {Id}";
            if (RootOwner != this)
                result = $"{result} owned by {RootOwner}";
            return result;
        }

        public Entity RootOwner
        {
            get
            {
                var entity = this;
                var ownedEntity = entity as IHasOwner;
                while (ownedEntity != null && ownedEntity.Owner != null)
                {
                    entity = ownedEntity.Owner;
                    ownedEntity = entity as IHasOwner;
                }
                return entity;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Entity)obj);
        }

        public bool Equals(Entity other)
        {
            return Id.Equals(other.Id);
        }

        public static bool operator ==(Entity a, Entity b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public Vector3f Position { get; set; }
        public Angle Heading { get; set; }
        public Vector3f Finish { get; set; }
        public Angle EndAngle { get; set; }
        public int Speed { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
    }
}