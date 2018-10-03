using System;
using System.Collections.Generic;

namespace GoblinDao
{
    class Program
    {
        static void Main(string[] args)
        {
            //GoblinDao.InsertAll(CreateStartingGoblins());

            List<Goblin> goblins = GoblinDao.GetAll();

            if (goblins.Count > 0)
            {
                //ApplyPoison(goblins);

                foreach (var goblin in goblins)
                {
                    Console.WriteLine(goblin);
                }
            }

            GoblinDao.InsertAll(goblins);
        }

        static List<Goblin> CreateStartingGoblins()
        {
            return new List<Goblin>()
            {
                new Goblin("Alice", 100),
                new Goblin("Bob", 200),
                new Goblin("Charlie", 300)
            };
        }

        static void ApplyPoison(List<Goblin> goblins)
        {
            foreach (var goblin in goblins)
            {
                goblin.hp -= 10;
            }
        }
    }
}
