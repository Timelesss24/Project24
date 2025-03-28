using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public abstract class ItemBase : MonoBehaviour, IEntity
    {
        [SerializeField] protected ItemData itemData {  get; private set; }
    }
}
