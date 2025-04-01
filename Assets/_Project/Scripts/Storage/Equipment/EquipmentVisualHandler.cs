using System;
using System.Collections.Generic;
using System.Linq;
using KBCore.Refs;
using UnityEngine;


namespace Timelesss
{
    [Serializable]
    class BoneTarget
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


            if (type == EquipmentType.Weapon)
            {
                var holder = animator.GetBoneTransform(HumanBodyBones.RightHand);
                var weapon = Instantiate(item.EquipmentPrefab, holder, true);
                combatController.SetWeaponObject(weapon, (WeaponDetails)item.Details);
                instances[type] = weapon;

                return;
            }

            // 프리팹에서 SkinnedMeshRenderer 가져오기
            var prefabSMR = item.EquipmentPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
            if (prefabSMR == null)
                return;

            var characterRenderer = GetRenderer(type);

            if (characterRenderer == null)
            {
                Debug.LogWarning($"No renderer for type {type}");
                return;
            }
            // 본 이름 기준으로 캐릭터의 본과 매핑
            Transform[] mappedBones = MapBonesByName(prefabSMR.bones, characterRenderer.bones);

            // 인스턴스 생성
            var instance = Instantiate(item.EquipmentPrefab, transform);
            var instanceSMR = instance.GetComponentInChildren<SkinnedMeshRenderer>();
            if (instanceSMR == null)
            {
                Destroy(instance);
                return;
            }

            // 본, 루트본 연결
            instanceSMR.bones = mappedBones;
            instanceSMR.rootBone = characterRenderer.rootBone;

            instances[type] = instance;
        }

        /// <summary>
        /// SkinnedMeshRenderer 본 배열 기준으로, 이름이 같은 본을 타겟에서 찾아 매핑함
        /// </summary>
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