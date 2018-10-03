using System.Linq;

namespace MurderInvitation
{
    class FelixSurvivorController : ActorController
    {
        public FelixSurvivorController(string name) : base(name)
        {
        }

        public override GameMove GenerateMove(GameData gameData)
        {
            var myQuery = from actor in gameData.actorDataList
                        where actor.Name == name
                        select actor;

            ActorData myData = myQuery.First();

            var actorsAliveQuery = from actor in gameData.actorDataList
                                   where actor.Hp > 0
                                   select actor;

            if (myData.Hp < 100 && myData.Items.Contains(Item.Medkit))
            {
                return new GameMove(myData.CurrentLocation, GameAction.UseMedkit, myData.Name, "I feel better.");
            }
            else if (actorsAliveQuery.Count() <= 2)
            {
                return new GameMove(GameMove.GetRandomLocation(), GameAction.NormalAttack, "", "I don't wanna die!");
            }
            return new GameMove(GameMove.GetRandomLocation(), GameMove.GetRandomAction(), "", "Hmm...");
        }
    }

    class FelixKillerController : ActorController
    {
        public FelixKillerController(string name) : base(name)
        {
        }

        public override GameMove GenerateMove(GameData gameData)
        {
            return new GameMove(GameMove.GetRandomLocation(), GameAction.StabAttack, "", "My blade thirsts...");
        }
    }

    class FelixControllerFactory : ActorControllerFactory
    {
        public FelixControllerFactory(string name) : base(name)
        {
        }

        public override ActorController CreateSurvivorController()
        {
            return new FelixSurvivorController(name);
        }

        public override ActorController CreateKillerController()
        {
            return new FelixKillerController(name);
        }
    }
}
