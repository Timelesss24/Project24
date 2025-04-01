using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    [System.Serializable]
    public class ActiveQuestInfo
    {
        public int questID;
        public int progress;
        public int goal;

        public ActiveQuestInfo(int questID, int goal)
        {
            this.questID = questID;
            this.progress = 0;
            this.goal = goal;
        }

        public bool IsComplete() => progress >= goal;
    }
}
