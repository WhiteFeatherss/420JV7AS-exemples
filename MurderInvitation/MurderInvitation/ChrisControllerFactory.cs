using System.Linq;

namespace MurderInvitation
{
    class ChrisSurvivorController : ActorController
    {
        public ChrisSurvivorController(string name) : base(name)
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
            
            if ((Location.Armory == myData.CurrentLocation) && (myData.Items.Contains(Item.Gun)))
            {
                return new GameMove(myData.CurrentLocation, GameAction.NormalAttack, "", "");
            }

            else if ((Location.Armory == myData.CurrentLocation) && (gameData.isSafeUnlocked))
            {
                return new GameMove(myData.CurrentLocation, GameAction.TakeGun, "", "ILL FOKIN SHOOT YOU MATE");
            }

            else if (Location.Armory == myData.CurrentLocation)
            {
                
                return new GameMove(myData.CurrentLocation, GameAction.UnlockSafe, myData.Name, "Be gentle...");
            }

            return new GameMove(Location.Armory, GameAction.Nothing, "", "Fuck a duck");
            //return new GameMove(GameMove.GetRandomLocation(), GameMove.GetRandomAction(), "", "Hmm...");
        }
    }

    class ChrisKillerController : ActorController
    { 
        public ChrisKillerController(string name) : base(name)
        {
        }

        public override GameMove GenerateMove(GameData gameData)
        {

            var query = from actor in gameData.actorDataList
                        where actor.Name == name
                        select actor;

            ActorData myData = query.First();

            var gunner = from actor in gameData.actorDataList
                         where actor.Items.Contains(Item.Gun)
                         select actor;

            if ((myData.Items.Contains(Item.Gun)) && (gameData.generatorHp < 100))
            {
                return new GameMove(Location.Basement, GameAction.StabAttack, "", "*Maniacal laugh*");
            }

            else if (gameData.isGunTaken == true)
            {
                foreach(var actor in gunner)
                {
                    if(actor.Items.Contains(Item.Gun))
                    {
                        return new GameMove(actor.CurrentLocation, GameAction.StabAttack, actor.Name, "I've... found you!");
                    }
                }
            }

            else if(gameData.generatorHp <=0)
            {
                return new GameMove(Location.Exit, GameAction.StabAttack, "", "Oh you sweet child!");
            }
            return new GameMove(Location.Exit, GameAction.StabAttack, "", "YOU WILL NEVER ESCAPE!");
        }
    }

    class ChrisControllerFactory : ActorControllerFactory
    {
        public ChrisControllerFactory(string name) : base(name)
        {
        }

        public override ActorController CreateSurvivorController()
        {
            return new ChrisSurvivorController(name);
        }

        public override ActorController CreateKillerController()
        {
            return new ChrisKillerController(name);
        }
    }
}
