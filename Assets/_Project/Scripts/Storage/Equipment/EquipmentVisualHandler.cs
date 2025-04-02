using System;
using System.Collections.Generic;
using System.Linq;
using KBCore.Refs;
using UnityEngine;


namespace Timelesss
{
    [Serializable]
    internal class BoneTarget
    {
        public EquipmentType Type;
        public SkinnedMeshRenderer Renderer;
    }

    public class EquipmentVisualHandler : MonoBehaviour
    {
        [SerializeField, Self] Animator animator;
        [SerializeField, Self] CombatController combatController;

        [SerializeField] List<BoneTarget> targets = new();
        readonly Dictionary<EquipmentType, SkinnedMeshRenderer> targetDic = new();
        readonly Dictionary<EquipmentType, GameObject> instances = new();

        void OnValidate() => this.ValidateRefs();

        void Awake()
        {
            foreach (var target in targets)
            {
                targetDic.TryAdd(target.Type, target.Renderer);
            }
        }

        public void Equip(Item item)
        {
            if (item?.Details.EquipmentPrefab == null)
                return;
            
            
            var type = item.Details.EquipmentType;

            // 기존 장비 제거
            Unequip(type);

            if (type == EquipmentType.Weapon)
            {
                var holder = animator.GetBoneTransform(HumanBodyBones.RightHand);
                var weapon = Instantiate(item.Details.EquipmentPrefab, holder, true);
                combatController.SetWeaponObject(weapon, item.Details);
                instances[type] = weapon;
                return;
            }
            

            var prefabSMR = item.Details.EquipmentPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
            if (prefabSMR == null)
                return;

            var characterRenderer = GetRenderer(type);
            if (characterRenderer == null)
            {
                Debug.LogWarning($"No renderer for type {type}");
                return;
            }

            Transform[] mappedBones = MapBonesByName(prefabSMR.bones, characterRenderer.bones);

            var instance = Instantiate(item.Details.EquipmentPrefab, transform);
            var instanceSMR = instance.GetComponentInChildren<SkinnedMeshRenderer>();
            if (instanceSMR == null)
            {
                Destroy(instance);
                return;
            }

            instanceSMR.bones = mappedBones;
            instanceSMR.rootBone = characterRenderer.rootBone;

            instances[type] = instance;
        }

        public void Unequip(EquipmentType type)
        {
            if (instances.TryGetValue(type, out var instance))
            {
                Destroy(instance);
                instances.Remove(type);
            }

            if (type == EquipmentType.Weapon)
            {
                combatController.UnEquipWeapon();
            }
        }

        Transform[] MapBonesByName(Transform[] sourceBones, Transform[] targetBones)
        {
            return sourceBones
                .Select(source => targetBones.FirstOrDefault(target => target.name == source.name))
                .ToArray();
        }

        SkinnedMeshRenderer GetRenderer(EquipmentType type)
        {
            return targetDic.GetValueOrDefault(type);
        }
    }
}