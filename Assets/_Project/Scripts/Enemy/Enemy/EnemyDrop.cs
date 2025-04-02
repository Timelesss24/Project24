using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class EnemyDrop : MonoBehaviour
    {
        [System.Serializable]
        public class EnemyDropItem
        {
            public ItemDetails itemDetails;
            public float probability; // 0~1 사이의 확률 (예: 0.1은 10%)
        }

        [SerializeField] private DropItem dropItemPrefab;
        [SerializeField] public List<EnemyDropItem> itemDataList = new List<EnemyDropItem>();

        
        public ItemDetails RandomItemDetails()
        {
            float total = 0f;
            foreach (var item in itemDataList)
                total += item.probability;

            float randomPoint = Random.value * total;

            float current = 0f;
            foreach (var item in itemDataList)
            {
                current += item.probability;
                if (randomPoint <= current)
                    return item.itemDetails;
            }

            return null;
        }
    }
}
