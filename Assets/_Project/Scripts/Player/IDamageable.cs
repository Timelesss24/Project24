using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public interface IDamageable
    {
        public event Action OnDamageTaken;
        public void TakeDamage(int value);
    }
}
