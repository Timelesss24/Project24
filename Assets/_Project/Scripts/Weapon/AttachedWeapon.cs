using UnityEngine;
using UnityEditor;


namespace Timelesss
{
    public class AttachedWeapon : MonoBehaviour
    {
        [field: SerializeField] public ItemDetails Weapon;
        [field: SerializeField] public GameObject Trail;
    }
}
