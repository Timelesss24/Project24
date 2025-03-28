using System;

namespace Timelesss
{
    /// <summary>
    /// 특정 동작(Action)을 캡슐화하고, 해당 동작이 호출되었을 때 true로 평가되는 조건(Predicate)을 나타냅니다.
    /// </summary>
    public class ActionPredicate : IPredicate {
        public bool Flag { get; private set; }

        public ActionPredicate(ref Action eventReaction) => eventReaction += () => { Flag = true; };

        public bool Evaluate() {
            bool result = Flag;
            Flag = false;
            return result;
        }
    }
}