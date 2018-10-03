using System.Collections.Generic;

namespace MurderInvitation
{
    abstract class ActorController
    {
        public readonly string name;
        public List<GameMove> moveHistory = new List<GameMove>();

        public ActorController(string name)
        {
            this.name = name;
        }

        abstract public GameMove GenerateMove(GameData gameData);
    }

    abstract class ActorControllerFactory
    {
        public readonly string name;

        protected ActorControllerFactory(string name)
        {
            this.name = name;
        }

        abstract public ActorController CreateSurvivorController();
        abstract public ActorController CreateKillerController();
    }
}
