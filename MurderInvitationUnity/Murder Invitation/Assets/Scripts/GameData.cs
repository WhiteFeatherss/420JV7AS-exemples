using System.Collections.Generic;

namespace MurderInvitation
{
    class GameData
    {
        public List<ActorData> actorDataList;
        public int gateHp;
        public int generatorHp;
        public bool isSafeUnlocked;
        public bool isGunTaken;
        public bool isMedkitTaken;

        public GameData(List<ActorData> actorDataList, int gateHp = 50, int generatorHp = 50, bool isSafeUnlocked = false, bool isGunTaken = false, bool isMedkitTaken = false)
        {
            this.actorDataList = actorDataList;
            this.gateHp = gateHp;
            this.generatorHp = generatorHp;
            this.isSafeUnlocked = isSafeUnlocked;
            this.isGunTaken = isGunTaken;
            this.isMedkitTaken = isMedkitTaken;
        }

        public GameData Clone()
        {
            return new GameData(new List<ActorData>(actorDataList), gateHp, generatorHp, isSafeUnlocked, isGunTaken, isMedkitTaken);
        }
    }
}
