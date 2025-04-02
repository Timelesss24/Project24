using System;
using System.Collections.Generic;
using System.Linq;

namespace Timelesss
{
    public class ComplexPredicate : IPredicate
    {
        readonly IEnumerable<IPredicate> predicates;

        public ComplexPredicate(IEnumerable<IPredicate> predicates)
        {
            this.predicates = predicates ?? throw new ArgumentNullException(nameof(predicates));
        }

        public bool Evaluate()
        {
            return predicates.All(predicate => predicate.Evaluate());
        }
    }
}