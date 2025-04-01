namespace Timelesss
{
    public class Transition : ITransition
    {

        public IState To { get; }
        public IPredicate Condition { get; }

        public Transition(IState to, IPredicate condition)
        {
            To = to;
            Condition = condition;
        }

    }
    public class TransitionAs : ITransition
    {

        public IState To { get; }
        public IPredicate Condition { get; }

        public TransitionAs(IState to)
        {
            To = to;
            //Condition = condition;
        }

    }
}