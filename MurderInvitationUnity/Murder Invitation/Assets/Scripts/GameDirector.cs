using MurderInvitation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    Init,
    GenerateMoves,
    ProcessingMoves,
    SurvivorVictory,
    KillerVictory
}

public class GameDirector : MonoBehaviour
{
    [SerializeField] PlayerToken[] playerTokens;
    [SerializeField] GameLocation armory;
    [SerializeField] GameLocation basement;
    [SerializeField] GameLocation bathroom;
    [SerializeField] GameLocation exitGate;
    [SerializeField] PlayerInfo[] playerInfos;

    [SerializeField] PlayerInfo generatorInfo;
    [SerializeField] PlayerInfo gateLockInfo;

    List<GameMove> moves = new List<GameMove>();
    private GameData gameData;
    private GameState gameState;
    private Actor killer;
    private List<Actor> actors;
    private List<ActorController> actorControllers;
    private int moveIndex;
    string endMessage;
    int endSequence;
    bool isWaiting = true;
    const int objectiveRepairRate = 10;

    public void Start()
    {
        MyRandom.UseUnityRandom();
        Play();
    }

    public void Play()
    {
        actors = new List<Actor>();
        actorControllers = new List<ActorController>();
        var factories = new List<ActorControllerFactory>()
            {
                new FelixControllerFactory("Alice"),
                new FelixControllerFactory("Bob"),
                new FelixControllerFactory("Chris"),
                new FelixControllerFactory("David"),
                new FelixControllerFactory("Eve"),
            };

        // Create 1 killer and 4 survivors
        while (actors.Count < 5)
        {
            ActorControllerFactory factory = factories[MyRandom.Next(0, factories.Count)];
            factories.Remove(factory);

            if (actors.Count == 0)
            {
                killer = new Actor(factory.name)
                {
                    isKiller = true
                };
                actors.Add(killer);
                actorControllers.Add(factory.CreateKillerController());
            }
            else
            {
                int insertionIndex = MyRandom.Next(0, actors.Count + 1);
                actors.Insert(insertionIndex, new Actor(factory.name));
                actorControllers.Insert(insertionIndex, factory.CreateSurvivorController());
            }
        }

        for (int i = 0; i < actors.Count; i++)
        {
            playerInfos[i].playerName.text = actors[i].name;
        }

        var actorDataQuery = from actor in actors
                             select actor.actorData;

        var actorDataList = actorDataQuery.ToList();

        gameData = new GameData(actorDataList);
        gameState = GameState.GenerateMoves;


        for (int i = 0; i < playerTokens.Count(); i++)
        {
            playerTokens[i].TargetPosition = exitGate.playerLocations[i].position;
        }
    }

    void Update()
    {
        UpdateHP();
        if (isWaiting)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isWaiting = false;
            }
            return;
        }

        if (gameState == GameState.SurvivorVictory || gameState == GameState.KillerVictory)
        {
            GameOuput.Output(endMessage);
        }

        if (gameState == GameState.GenerateMoves)
        {
            // Generate every actor's move
            moves.Clear();
            for (int i = 0; i < actors.Count; i++)
            {
                Actor actor = actors[i];
                if (actor.hp <= 0)
                {
                    continue;
                }
                ActorController controller = actorControllers[i];
                GameMove move = controller.GenerateMove(gameData.Clone());
                move.actionAuthorName = actor.name;
                moves.Add(move);
            }

            gameState = GameState.ProcessingMoves;
            moveIndex = 0;
        }

        if (gameState == GameState.ProcessingMoves)
        {
            if (moveIndex >= moves.Count)
            {
                gameState = GameState.GenerateMoves;
                return;
            }

            ProcessOneMove();
        }
    }

    void UpdateHP()
    {
        for (int i = 0; i < actors.Count; i++)
        {
            playerInfos[i].SetHp(actors[i].hp);
            playerTokens[i].UpdateHp(actors[i].hp);
        }
        generatorInfo.SetHp(gameData.generatorHp, 50);
        gateLockInfo.SetHp(gameData.gateHp, 50);
    }

    void ProcessOneMove()
    {
        for (int i = moveIndex; i < moves.Count; i++)
        {
            moveIndex++;
            GameMove move = moves[i];

            Actor activeActor = actors.Find(a => a.name == move.actionAuthorName);

            if (activeActor == null)
            {
                Debug.LogError("Couldn't find actor: " + move.actionAuthorName);
            }

            if (activeActor.hp <= 0)
            {
                continue;
            }
            bool hasProcessedAMove = false;
            if (ProcessMove(move, actors, gameData))
            {
                if (move.gameAction == GameAction.StabAttack)
                {
                    move.actionAuthorName = "";
                }
                foreach (var controller in actorControllers)
                {
                    controller.moveHistory.Add(move.Clone());
                }
                hasProcessedAMove = true;
            }
            gameState = GetGameState(gameData, killer);
            if (hasProcessedAMove)
            {
                isWaiting = true;
                break;
            }
        }
    }

    GameState GetGameState(GameData gameData, Actor killer)
    {
        // Everything is repaired
        if (gameData.gateHp == 0 && gameData.generatorHp == 0)
        {
            endMessage = ($"Generator and exit gate repaired!\nSurvivors escape! {killer.name} disappears in the shadows...");
            killer.hp -= 100;
            return GameState.SurvivorVictory;
        }

        var aliveQuery = from actor in gameData.actorDataList
                         where actor.Hp > 0
                         select actor;

        // Killer is dead
        if (killer.hp <= 0)
        {
            endMessage = ($"Survivors have defeated {killer.name} the killer!");
            return GameState.SurvivorVictory;
        }

        // Only killer alive
        if (aliveQuery.Count() == 1)
        {
            endMessage = ($"The {killer.name} the killer has eliminated all survivors!");
            return GameState.KillerVictory;
        }
        return GameState.ProcessingMoves;
    }

    bool ProcessMove(GameMove gameMove, List<Actor> actors, GameData gameData)
    {
        Actor currentActor = actors.Find(a => a.name == gameMove.actionAuthorName);
        currentActor.currentLocation = gameMove.nextLocation;
        int index = actors.IndexOf(currentActor);
        playerTokens[index].TargetPosition = GetLocation(gameMove.nextLocation).playerLocations[index].position;

        switch (gameMove.gameAction)
        {
            default:
            case GameAction.Nothing:
                break;
            case GameAction.NormalAttack:
            case GameAction.StabAttack:
                return ProcessAttack(gameMove, actors, currentActor);
            case GameAction.RepairGenerator:
                if (currentActor.currentLocation == Location.Basement && gameData.generatorHp > 0)
                {
                    gameData.generatorHp -= objectiveRepairRate;
                    GameOuput.Output($"{currentActor.name} repairs the generator in the basement.");
                    if (gameData.generatorHp <= 0)
                    {
                        basement.ClearObjective();
                    }
                    return true;
                }
                break;
            case GameAction.RepairGate:
                if (currentActor.currentLocation == Location.Exit && gameData.gateHp > 0)
                {
                    gameData.gateHp -= objectiveRepairRate;
                    GameOuput.Output($"{currentActor.name} repairs the exit gate.");
                    if (gameData.gateHp <= 0)
                    {
                        exitGate.ClearObjective();
                    }
                    return true;
                }
                break;
            case GameAction.UnlockSafe:
                if (currentActor.currentLocation == Location.Armory && !gameData.isSafeUnlocked)
                {
                    gameData.isSafeUnlocked = true;
                    GameOuput.Output($"{currentActor.name} unlocks the safe containing the gun.");
                    armory.ChangeObjective();
                    return true;
                }
                break;
            case GameAction.TakeGun:
                if (currentActor.currentLocation == Location.Armory && !gameData.isGunTaken && gameData.isSafeUnlocked)
                {
                    gameData.isGunTaken = true;
                    currentActor.items.Add(Item.Gun);
                    GameOuput.Output($"{currentActor.name} takes the gun in the safe.");
                    armory.ClearObjective();
                    return true;
                }
                break;
            case GameAction.TakeMedkit:
                if (currentActor.currentLocation == Location.Bathroom && !gameData.isMedkitTaken)
                {
                    gameData.isMedkitTaken = true;
                    currentActor.items.Add(Item.Medkit);
                    GameOuput.Output($"{currentActor.name} takes the medkit in the bathroom.");
                    bathroom.ClearObjective();
                    return true;
                }
                break;
            case GameAction.UseMedkit:
                if (currentActor.items.Contains(Item.Medkit))
                {
                    Actor healTarget = actors.Find(actor => actor.name == gameMove.actionTargetName);
                    if (healTarget == null)
                    {
                        var anyActorQuery = from actor in actors
                                            where actor.currentLocation == currentActor.currentLocation
                                            select actor;
                        var anyActorList = anyActorQuery.ToList();
                        healTarget = anyActorList[MyRandom.Next(0, anyActorList.Count)];
                    }

                    if (healTarget.currentLocation == currentActor.currentLocation)
                    {
                        currentActor.items.Remove(Item.Medkit);
                        healTarget.hp = 100;
                        GameOuput.Output($"{currentActor.name} heals {healTarget.name} with the medkit!");
                        return true;
                    }
                }
                break;
        }
        return false;
    }

    static bool ProcessAttack(GameMove gameMove, List<Actor> actors, Actor attacker)
    {
        Actor attackTarget = actors.Find(actor => actor.name == gameMove.actionTargetName);
        if (attackTarget == null)
        {
            var otherActorsQuery = from actor in actors
                                   where actor.currentLocation == gameMove.nextLocation
                                   && actor.hp > 0
                                   && actor != attacker
                                   select actor;

            var result = otherActorsQuery.ToList();
            if (result.Count > 0)
            {
                attackTarget = result[MyRandom.Next(0, result.Count)];
            }
        }
        if (attackTarget != null)
        {
            bool isAttackGood = false;
            string attackText = "";
            if (gameMove.gameAction == GameAction.NormalAttack)
            {
                if (attacker.items.Contains(Item.Gun))
                {
                    attackText += ($"BANG! {attacker.name} shoots {attackTarget.name} with a gun!");
                    attackTarget.hp -= 100;
                    isAttackGood = true;
                }
                else
                {
                    attackText += ($"{attacker.name} punches {attackTarget.name} in the face!");
                    attackTarget.hp -= 25;
                    isAttackGood = true;
                }
            }
            else if (gameMove.gameAction == GameAction.StabAttack && attacker.isKiller)
            {
                attackText += ($"{attackTarget.name} is stabbed by the killer in the shadows.");
                attackTarget.hp -= 50;
                isAttackGood = true;
            }

            if (attackTarget.hp <= 0)
            {
                attackText += ($"\n{attackTarget.name} has died...");
                attacker.items.AddRange(attackTarget.items);
                attackTarget.items.Clear();
            }

            if (isAttackGood)
            {
                GameOuput.Output(attackText);
            }
            return isAttackGood;
        }
        else
        {
            return false;
        }
    }

    GameLocation GetLocation(Location location)
    {
        switch (location)
        {
            case Location.Armory:
                return armory;
            case Location.Bathroom:
                return bathroom;
            case Location.Basement:
                return basement;
            case Location.Exit:
            default:
                return exitGate;
        }
    }
}
