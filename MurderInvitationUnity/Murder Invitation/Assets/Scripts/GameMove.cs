using System;

namespace MurderInvitation
{
    public enum Item { Gun, Medkit };

    public enum Location { Armory, Bathroom, Basement, Exit }

    public enum GameAction
    {
        Nothing,
        NormalAttack, // Punch or shoot
        StabAttack, // Only killer can use
        RepairGenerator,
        RepairGate,
        UnlockSafe,
        TakeGun,
        TakeMedkit,
        UseMedkit
    }

    class GameMove
    {
        public string actionAuthorName; // Name of actor doing the move
        public Location nextLocation = Location.Exit;
        public GameAction gameAction;
        public string actionTargetName; // Name of actor being the target
        static Random random = new Random();
        public string message;

        public GameMove(Location nextLocation, GameAction gameAction, string actionTargetName = "", string message = "")
        {
            this.nextLocation = nextLocation;
            this.gameAction = gameAction;
            this.actionTargetName = actionTargetName;
            this.message = message;
        }

        public static Location GetRandomLocation()
        {
            return (Location) random.Next(0, Enum.GetValues(typeof(Location)).Length);
        }

        public static GameAction GetRandomAction()
        {
            return (GameAction)random.Next(0, Enum.GetValues(typeof(GameAction)).Length);
        }

        public GameMove Clone()
        {
            var move = new GameMove(nextLocation, gameAction, actionTargetName, message);
            move.actionAuthorName = actionAuthorName;
            return move;
        }

        public override string ToString()
        {
            return $"author: {actionAuthorName}, nextLocation: {nextLocation}, action: {gameAction}, target: {actionTargetName}\n message: {message}";
        }
    }
}
