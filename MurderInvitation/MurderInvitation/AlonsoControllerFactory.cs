using System;
using System.Linq;

namespace MurderInvitation
{
    class AlonsoSurvivorController : ActorController
    {
        public AlonsoSurvivorController(string name) : base(name)
        {
        }

        public override GameMove GenerateMove(GameData gameData)
        {   

            var hasGun = from actor in gameData.actorDataList
                         where actor.Name != name && actor.Items.Contains(Item.Gun) && actor.Hp > 0
                         select actor;
           
            if (hasGun.Count() != 0)
            {
                ActorData badGuy = hasGun.First();
                return new GameMove(badGuy.CurrentLocation, GameAction.NormalAttack, badGuy.Name);

            }
           
            else if (gameData.actorDataList.Count <= 2)
            {
                var BadGuyLocation = from actor in gameData.actorDataList
                                     where actor.Name != name
                                     select actor.CurrentLocation;
                                        
                                        
                return new GameMove(BadGuyLocation.First(), GameAction.NormalAttack, "*punch*");
            }
            if (gameData.generatorHp > 0)
            {
                return new GameMove(Location.Basement, GameAction.RepairGenerator, "I'm workwing werry hawd fwixing twis, owo");
            }
            else if (gameData.gateHp > 0) {
                return new GameMove(Location.Exit, GameAction.RepairGate, "*hits gate* owo, I dwid an oopsie woopsie");

            }
            return new GameMove(GameMove.GetRandomLocation(), GameMove.GetRandomAction());
        }
    }

    class AlonsoKillerController : ActorController
    {
        public AlonsoKillerController(string name) : base(name)
        {
        }
        GameAction attack= GameAction.StabAttack;
        int Damage = 50;
        string messageOwo = "owo *stab stab* wuts twis uwu";
        public override GameMove GenerateMove(GameData gameData)
        {
            foreach (var actor in gameData.actorDataList) {

                Console.WriteLine(actor.Name + "  =  " + actor.Hp);

            }
            if (!gameData.isGunTaken)
            {
                if (!gameData.isSafeUnlocked)
                {
                   // return new GameMove(Location.Armory, GameAction.UnlockSafe);

                }
                else {
                    attack = GameAction.NormalAttack;
                    Damage = 100;
                    messageOwo = "*pew pew* OwO I did an oopsie woopsie";
                    return new GameMove(Location.Armory, GameAction.TakeGun);
                }
            }
            
            var ezKill = from actor in gameData.actorDataList
                         where actor.Hp <= Damage && actor.Hp > 0 && actor.Name !=name
                         orderby actor.Hp ascending
                         select actor;
            var everybody = from actor in gameData.actorDataList
                            where actor.Hp > 0 && actor.Name != name
                            orderby actor.Hp descending
                            select actor;
            ActorData target = everybody.First();
            if (ezKill.Count() != 0) {
                ActorData ripActor = ezKill.First();

                return new GameMove(ripActor.CurrentLocation, attack, ripActor.Name, messageOwo);

            }
            
            return new GameMove(target.CurrentLocation , attack, target.Name, messageOwo);
        }

    }

    class AlonsoControllerFactory : ActorControllerFactory
    {
        public AlonsoControllerFactory(string name) : base(name)
        {
        }

        public override ActorController CreateSurvivorController()
        {
            return new AlonsoSurvivorController(name);
        }

        public override ActorController CreateKillerController()
        {
            return new AlonsoKillerController(name);
        }
    }
}
