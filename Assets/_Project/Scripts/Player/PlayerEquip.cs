using System;
using System.Collections.Generic;
using System.Linq;
using KBCore.Refs;
using UnityEngine;

namespace Timelesss
{
    public class PlayerEquip : MonoBehaviour
    {
        public WeaponData Weapon { get; private set; }
        public EquipItemData Helmet { get; private set; }
        
        public Dictionary<EquipItemData, GameObject>  EquipItems { get; private set; }


        [SerializeField, Self] Animator animator;
        public GameObject helmet;

        public void ChangeMaterial(EquipItemData equipItemData)
        {
            switch (equipItemData.EquipType)
            {
                case EquipType.Helmet:
                    helmet.SetActive(true);
                    Material[] materials = helmet.GetComponent<SkinnedMeshRenderer>().materials;
                   // materials[0] = equipItemData.Material;
                    helmet.GetComponent<SkinnedMeshRenderer>().materials = materials;
                    break;
                default:
                    break;
            }
        }
        
        void EquipWeaponObject(EquipItemData data)
        {
            var holder = animator.GetBoneTransform(data.GetHolder());
            GameObject obj;
            if (data.EquipType == EquipType.Sword)
                GetComponent<CombatController>().EquipWeapon((WeaponData)data);
            
            else
            
                Instantiate(data.EquipPrefab, holder, true);   
            }
         
            EquipItems[data].Add();
        }
        
        void UnEquipWeaponObject(EquipItemData data)
        {
          
           if (!EquipItems.TryGetValue(data, out var obj)) return;

            if (data.EquipType == EquipType.Sword)
                GetComponent<CombatController>().UnEquipWeapon();
            
            Destroy(obj);
            EquipItems.Remove(data);
        }
        
    }
}
