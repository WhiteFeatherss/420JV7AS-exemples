using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurderInvitation
{
    class Actor
    {
        public ActorData actorData;
        public Location currentLocation = Location.Basement;
        public int hp = 100;
        public string name;
        public List<Item> items = new List<Item>();
        public bool isKiller;

        public Actor(string name)
        {
            this.name = name;
            actorData = new ActorData(this);
        }
    }
}
