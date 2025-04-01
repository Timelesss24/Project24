// using System;
// using System.Collections.Generic;
// using System.Linq;
// using KBCore.Refs;
// using UnityEngine;
//
// namespace Timelesss
// {
//     public class PlayerEquip : MonoBehaviour
//     {
//         public WeaponData Weapon { get; private set; }
//         public EquipItemData Helmet { get; private set; }
//         
//         public List<ItemInstance>  EquipItems { get; private set; }
//
//
//         [SerializeField, Self] Animator animator;
//         public GameObject helmet;
//
//         void OnValidate() => this.ValidateRefs();
//
//         public void ChangeMaterial(EquipItemData equipItemData)
//         {
//             
//             switch (equipItemData.EquipType)
//             {
//                 case EquipType.Helmet:
//                     helmet.SetActive(true);
//                     Material[] materials = helmet.GetComponent<SkinnedMeshRenderer>().materials;
//                    // materials[0] = equipItemData.Material;
//                     helmet.GetComponent<SkinnedMeshRenderer>().materials = materials;
//                     break;
//                 default:
//                     break;
//             }
//         }
//         
//         // public void EquipWeaponObject(ItemInstance item)
//         // {
//         //     var holder = animator.GetBoneTransform(item.Data.GetHolder());
//         //     GameObject obj = null;
//         //     if (data.EquipType == EquipType.Sword)
//         //         GetComponent<CombatController>().EquipWeapon((WeaponData)data);
//         //     else
//         //         obj = Instantiate(data.EquipPrefab, holder, true);   
//         //     
//         //     EquipItems[data] = obj;
//         // }
//         //
//         // public void UnEquipWeaponObject(ItemInstance item)
//         // {
//         //   
//         //    if (!EquipItems.TryGetValue(data, out var obj)) return;
//         //
//         //     if (data.EquipType == EquipType.Sword)
//         //         GetComponent<CombatController>().UnEquipWeapon();
//         //     
//         //     Destroy(obj);
//         //     EquipItems.Remove(data);
//         // }
//         
//     }
// }
