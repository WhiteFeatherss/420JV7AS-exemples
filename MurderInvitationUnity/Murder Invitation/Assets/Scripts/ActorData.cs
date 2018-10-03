using System.Collections.Generic;

namespace MurderInvitation
{
    class ActorData
    {
        Actor actor;

        public Location CurrentLocation
        {
            get
            {
                return actor.currentLocation;
            }
        }

        public int Hp
        {
            get
            {
                return actor.hp;
            }
        }

        public List<Item> Items
        {
            get
            {
                return new List<Item>(actor.items);
            }
        }

        public string Name
        {
            get
            {
                return actor.name;
            }
        }

        public ActorData(Actor actor)
        {
            this.actor = actor;
        }
    }
}
