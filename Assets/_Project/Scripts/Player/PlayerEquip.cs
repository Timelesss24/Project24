using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class PlayerEquip : MonoBehaviour
    {
        public GameObject helmet;

        public void ChangeMaterial(EquipItemData equipItemData)
        {
            switch (equipItemData.equipType)
            {
                case EquipType.Helmet:
                    helmet.SetActive(true);
                    Material[] materials = helmet.GetComponent<SkinnedMeshRenderer>().materials;
                    materials[0] = equipItemData.Material;
                    helmet.GetComponent<SkinnedMeshRenderer>().materials = materials;
                    break;
                default:
                    break;
            }
        }
    }
}
