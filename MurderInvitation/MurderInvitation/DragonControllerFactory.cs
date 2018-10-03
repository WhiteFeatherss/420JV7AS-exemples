using System;
using System.Collections.Generic;
using System.Linq;

namespace MurderInvitation
{
    class DragonSurvivorController : ActorController
    {
        public DragonSurvivorController(string name) : base(name)
        {
        }

        public override GameMove GenerateMove(GameData gameData)
        {
            var query = from actor in gameData.actorDataList
                        where actor.Name == name
                        select actor;

            ActorData myData = query.First();

            if (myData.Hp <= 50 && myData.Items.Contains(Item.Medkit))
            {
                return new GameMove(myData.CurrentLocation, GameAction.UseMedkit, myData.Name);
            }
            else if (!gameData.isMedkitTaken)
            {
                return new GameMove(Location.Bathroom, GameAction.TakeMedkit);
            }
            else if (gameData.gateHp > 0)
            {
                return new GameMove(Location.Exit, GameAction.RepairGate);
            }
            else if (gameData.generatorHp > 0)
            {
                return new GameMove(Location.Basement, GameAction.RepairGenerator);
            }
            else
            {
                return new GameMove(GameMove.GetRandomLocation(), GameMove.GetRandomAction());
            }
        }
    }

    class DragonKillerController : ActorController
    {
        public DragonKillerController(string name) : base(name)
        {
        }


        public override GameMove GenerateMove(GameData gameData)
        {
            var query = from actor in gameData.actorDataList
                        where actor.Name == name
                        select actor;

            bool tryStabArmory = false;

            //var hLQuery = from actor in gameData.actorDataList
            //              where actor.Name != name
            //              where actor.Hp<=50
            //              where actor.Hp>0
            //              select actor;

            var ennemies = from actor in gameData.actorDataList
                           where actor.Name != name
                           select actor;

            var gunner = from actor in gameData.actorDataList
                         where actor.Items.Contains(Item.Gun)
                         select actor;

            var medic = from actor in gameData.actorDataList
                         where actor.Items.Contains(Item.Medkit)
                         select actor;

            

            int nbLActor = 0;

            foreach(ActorData a in gameData.actorDataList)
            {
                if (a.Hp > 0) nbLActor++;
            }

            List<GameMove> lastGM = new List<GameMove>();
            foreach (GameMove m in moveHistory) lastGM.Add(m);

            

            ActorData myData = query.First();
            ActorData gunOwner=new ActorData(new Actor(""));
            if(gunner.Count()>0) gunOwner = gunner.First();
            ActorData medOwner = null;
            if (medic.Count() > 0) medOwner = medic.First();

                var gunLM = from move in lastGM
                        where move.actionAuthorName == gunOwner.Name
                        select move;
            

            List<ActorData> ennemiesData = ennemies.ToList();
            GameMove gunnerLM = null;
            if (lastGM.Count> nbLActor)
            {
                lastGM.RemoveRange(0, moveHistory.Count - nbLActor);
                if(gunLM.Count()>0)
                    gunnerLM = gunLM.First();
            }


            {
                if (myData.Hp <= 50 && myData.Items.Contains(Item.Medkit))
                {
                    return new GameMove(myData.CurrentLocation, GameAction.UseMedkit, myData.Name);
                }
                else if (!gameData.isMedkitTaken)
                {
                    return new GameMove(Location.Bathroom, GameAction.TakeMedkit);
                }
                else if (gameData.isSafeUnlocked && !gameData.isGunTaken)
                {
                    return new GameMove(Location.Armory, GameAction.TakeGun);
                }
                else if (myData.Items.Contains(Item.Gun))
                {
                    if (gameData.generatorHp > 0)
                    {
                        return new GameMove(Location.Basement, GameAction.StabAttack, GetLTarget(gameData));
                    }
                    else
                    {
                        return new GameMove(Location.Exit, GameAction.StabAttack, GetLTarget(gameData));
                    }
                }
                else if (GetFLTarget(gameData) == null)
                {
                    if (!tryStabArmory)
                    {
                        tryStabArmory = true;
                        return new GameMove(Location.Armory, GameAction.UnlockSafe);
                    }
                    else
                    {
                        tryStabArmory = false;
                        return new GameMove(Location.Armory, GameAction.StabAttack, GetGunOwner(gameData));
                    }
                }
                else if (gameData.generatorHp > 0)
                {
                    return new GameMove(Location.Basement, GameAction.StabAttack, GetLTarget(gameData));
                }
                else
                {
                    return new GameMove(Location.Exit, GameAction.StabAttack, GetLTarget(gameData));
                }
            }

            {

            //if (myData.Hp <= 50 && myData.Items.Contains(Item.Medkit))
            //{
            //    return new GameMove(myData.CurrentLocation, GameAction.UseMedkit, myData.Name);
            //}
            //else if (!gameData.isMedkitTaken)
            //{
            //    return new GameMove(Location.Bathroom, GameAction.TakeMedkit);
            //}
            //else if (gameData.isSafeUnlocked && !gameData.isGunTaken)
            //{
            //    return new GameMove(Location.Armory, GameAction.TakeGun);
            //}
            //else if(myData.Items.Contains(Item.Gun))
            //{
            //    string baseTarget = GetLTarget(gameData, Location.Basement);
            //    string exitTarget = GetLTarget(gameData, Location.Exit);
            //    string bathTarget = GetLTarget(gameData, Location.Bathroom);
            //    string armTarget = GetLTarget(gameData, Location.Armory);

            //    if(baseTarget!=null)
            //    {
            //        return new GameMove(Location.Basement, GameAction.StabAttack,baseTarget);

            //    }
            //    else if(exitTarget!=null)
            //    {
            //        return new GameMove(Location.Exit, GameAction.StabAttack, exitTarget);

            //    }
            //    else if (armTarget != null)
            //    {
            //        return new GameMove(Location.Armory, GameAction.StabAttack, armTarget);

            //    }
            //    else 
            //    {
            //        return new GameMove(Location.Bathroom, GameAction.StabAttack, bathTarget);

            //    }

            //}
            //else if(GetFLTarget(gameData)==null)
            //{
            //    if(!gameData.isSafeUnlocked)
            //    {
            //        return new GameMove(Location.Armory, GameAction.UnlockSafe);
            //    }
            //    else
            //    {
            //        if(!gameData.isGunTaken)
            //        {
            //            return new GameMove(Location.Armory, GameAction.TakeGun);
            //        }
            //        else
            //        {
            //            return new GameMove(gunnerLM.nextLocation, GameAction.StabAttack, gunnerLM.actionAuthorName);
            //        }
            //    }
            //}
            //else
            //{
            //    string baseTarget = GetFLTarget(gameData, Location.Basement);
            //    string exitTarget = GetFLTarget(gameData, Location.Exit);
            //    string bathTarget = GetFLTarget(gameData, Location.Bathroom);
            //    string armTarget = GetFLTarget(gameData, Location.Armory);

            //    if (baseTarget != null)
            //    {
            //        return new GameMove(Location.Basement, GameAction.StabAttack, baseTarget);

            //    }
            //    else if (exitTarget != null)
            //    {
            //        return new GameMove(Location.Exit, GameAction.StabAttack, exitTarget);

            //    }
            //    else if (armTarget != null)
            //    {
            //        return new GameMove(Location.Armory, GameAction.StabAttack, armTarget);

            //    }
            //    else if(bathTarget!=null)
            //    {
            //        return new GameMove(Location.Bathroom, GameAction.StabAttack, bathTarget);

            //    }
            //    else
            //    {
            //        baseTarget = GetHLTarget(gameData, Location.Basement);
            //        exitTarget = GetHLTarget(gameData, Location.Exit);
            //        bathTarget = GetHLTarget(gameData, Location.Bathroom);
            //        armTarget = GetHLTarget(gameData, Location.Armory);
            //        if (baseTarget != null)
            //        {
            //            return new GameMove(Location.Basement, GameAction.StabAttack, baseTarget);

            //        }
            //        else if (exitTarget != null)
            //        {
            //            return new GameMove(Location.Exit, GameAction.StabAttack, exitTarget);

            //        }
            //        else if (armTarget != null)
            //        {
            //            return new GameMove(Location.Armory, GameAction.StabAttack, armTarget);

            //        }
            //        else if (bathTarget != null)
            //        {
            //            return new GameMove(Location.Bathroom, GameAction.StabAttack, bathTarget);

            //        }
            //        else
            //        {
            //            baseTarget = GetLTarget(gameData, Location.Basement);
            //            exitTarget = GetLTarget(gameData, Location.Exit);
            //            bathTarget = GetLTarget(gameData, Location.Bathroom);
            //            armTarget = GetLTarget(gameData, Location.Armory);
            //            if (baseTarget != null)
            //            {
            //                return new GameMove(Location.Basement, GameAction.StabAttack, baseTarget);

            //            }
            //            else if (exitTarget != null)
            //            {
            //                return new GameMove(Location.Exit, GameAction.StabAttack, exitTarget);

            //            }
            //            else if (armTarget != null)
            //            {
            //                return new GameMove(Location.Armory, GameAction.StabAttack, armTarget);

            //            }
            //            else
            //            {
            //                return new GameMove(Location.Bathroom, GameAction.StabAttack, bathTarget);

            //            }
            //        }
            //    }
            //}
            }


        }

        string GetFLTarget(GameData gameData)
        {
            var actors = new List<string>();
            foreach (ActorData a in gameData.actorDataList)
            {
                if (a.Hp > 50 && a.Name != name)
                {
                    actors.Add(a.Name);
                }
            }
            if (actors.Count > 0)
            {
                Random rdm = new Random();
                return actors[rdm.Next(0, actors.Count - 1)];

            }
            return null;
        }

        string GetFLTarget(GameData gameData,Location l)
        {
            var actors = new List<string>();
            foreach (ActorData a in gameData.actorDataList)
            {
                if (a.Hp > 50 && a.Name != name && a.CurrentLocation == l)
                {
                    actors.Add(a.Name);
                }
            }
            if (actors.Count > 0)
            {
                Random rdm = new Random();
                return actors[rdm.Next(0, actors.Count - 1)];

            }
            return null;
        }

        string GetHLTarget(GameData gameData)
        {
            var actors = new List<string>();
            foreach (ActorData a in gameData.actorDataList)
            {
                if (a.Hp <= 50 && a.Hp > 0 && a.Name != name)
                {
                    actors.Add(a.Name);
                }
            }
            if (actors.Count > 0)
            {
                Random rdm = new Random();
                return actors[rdm.Next(0, actors.Count - 1)];

            }
            return null;
        }

        string GetHLTarget(GameData gameData,Location l)
        {
            var actors = new List<string>();
            foreach (ActorData a in gameData.actorDataList)
            {
                if (a.Hp <= 50 && a.Hp > 0 && a.Name != name && a.CurrentLocation == l)
                {
                    actors.Add(a.Name);
                }
            }
            if (actors.Count > 0)
            {
                Random rdm = new Random();
                return actors[rdm.Next(0, actors.Count - 1)];

            }
            return null;
        }

        string GetLTarget(GameData gameData)
        {
            var actors = new List<string>();
            foreach (ActorData a in gameData.actorDataList)
            {
                if (a.Hp > 0 && a.Name != name)
                {
                    actors.Add(a.Name);
                }
            }
            if (actors.Count > 0)
            {
                Random rdm = new Random();
                return actors[rdm.Next(0, actors.Count - 1)];

            }
            return null;
        }

        string GetLTarget(GameData gameData,Location l)
        {
            var actors = new List<string>();
            foreach (ActorData a in gameData.actorDataList)
            {
                if (a.Hp > 0 && a.Name != name&&a.CurrentLocation==l)
                {
                    actors.Add(a.Name);
                }
            }
            if (actors.Count > 0)
            {
                Random rdm = new Random();
                return actors[rdm.Next(0, actors.Count - 1)];

            }
            return null;
        }

        string GetGunOwner(GameData gameData)
        {
            foreach (ActorData a in gameData.actorDataList)
            {
                if (a.Items.Contains(Item.Gun))
                {
                    return a.Name;
                }
            }
            return null;
        }
    }

    class DragonControllerFactory : ActorControllerFactory
    {
        public DragonControllerFactory(string name) : base(name)
        {
        }

        public override ActorController CreateSurvivorController()
        {
            return new DragonSurvivorController(name);
        }

        public override ActorController CreateKillerController()
        {
            return new DragonKillerController(name);
        }
    }
}
