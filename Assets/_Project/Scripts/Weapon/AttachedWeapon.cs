using UnityEngine;
using UnityEditor;


namespace Timelesss
{
    public class AttachedWeapon : MonoBehaviour
    {
        [field: SerializeField] public WeaponDetails Weapon;
        [field: SerializeField] public GameObject Trail;
    }
}
