using System;

namespace Timelesss
{
    public class ActionPredicate : IPredicate
    {
        private readonly Action action;
        private readonly Func<bool> condition;

        public ActionPredicate(Action action, Func<bool> condition)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
            this.condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        public bool Evaluate()
        {
            action?.Invoke();
            return condition.Invoke();
        }
    }
}