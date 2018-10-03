namespace GoblinDao
{
    class Goblin
    {
        public string name;
        public int hp;

        public Goblin(string name, int hp)
        {
            this.name = name;
            this.hp = hp;
        }

        public override string ToString()
        {
            return $"Goblin named {name} has {hp} hp.";
        }
    }
}
