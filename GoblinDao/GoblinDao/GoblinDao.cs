using System.Collections.Generic;
using System.IO;

namespace GoblinDao
{
    static class GoblinDao
    {
        const string DatabasePath = @".\GoblinDatabase.txt";
        const char Separator = ',';

        public static List<Goblin> GetAll()
        {
            var goblins = new List<Goblin>();

            if (File.Exists(DatabasePath))
            {
                string[] lines = File.ReadAllLines(DatabasePath);

                foreach (var line in lines)
                {
                    string[] data = line.Split(Separator);
                    string name = data[0];
                    int hp = int.Parse(data[1]);
                    goblins.Add(new Goblin(name, hp));
                }
            }

            return goblins;
        }

        public static void InsertAll(List<Goblin> goblins)
        {
            if (goblins.Count == 0)
            {
                return;
            }

            string[] lines = new string[goblins.Count];

            for (int i = 0; i < goblins.Count; i++)
            {
                lines[i] = goblins[i].name + Separator + goblins[i].hp;
            }

            File.WriteAllLines(DatabasePath, lines);
        }
    }
}
