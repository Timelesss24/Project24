using UnityEngine;
using UnityEditor;


namespace Timelesss
{
    public class AttachedWeapon : MonoBehaviour
    {
        [field: SerializeField] public WeaponData Weapon;
        [field: SerializeField] public GameObject Trail;
    }
}
