using System;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

namespace Timelesss
{
    public class EquipmentVisualHandler : MonoBehaviour
    {
        [SerializeField, Self] Animator animator;
        
        Dictionary<EquipmentType, GameObject> instances = new();
        
        void OnValidate() => this.ValidateRefs();
        
        public void Equip(EquipmentItem item)
        {
            if (item?.EquipmentPrefab == null)
                return;

            var type = item.EquipmentType;

            // 기존 장비 제거
            if (instances.TryGetValue(type, out var old))
            {
                Destroy(old);
            }

            // 새 장비 생성
            
            var mount = animator.GetBoneTransform(item.Mount);
            if (mount == null) return;

            var instance = Instantiate(item.EquipmentPrefab, mount);
            instance.transform.localPosition = item.LocalPosition;
            instance.transform.localEulerAngles = item.LocalRotation;

            instances[type] = instance;
        }
    }
}