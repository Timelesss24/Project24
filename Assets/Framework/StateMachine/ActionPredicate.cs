using System;

namespace Timelesss
{
    /// <summary>
    /// Represents a predicate that encapsulates an action and evaluates to true once the action has been invoked.
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