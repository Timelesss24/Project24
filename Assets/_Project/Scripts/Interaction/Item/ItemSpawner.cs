using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtils;

namespace Timelesss
{
    public class ItemSpawner : Singleton<ItemSpawner>
    {
        [SerializeField] private DropItem dropItemPrefab;
        [SerializeField] private int poolSize = 10;
        [SerializeField] public List<ItemDetails> itemDataList = new List<ItemDetails>();

        private Queue<DropItem> itemPool = new Queue<DropItem>();

        private void Start()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                CreateNewItem();
            }
        }

        private void CreateNewItem()
        {
            DropItem newItem = Instantiate(dropItemPrefab, transform);
            newItem.gameObject.SetActive(false);
            itemPool.Enqueue(newItem);
        }

        public void SpawnItem(ItemDetails itemData, Vector3 spawnPosition)
        {
            if (itemPool.Count == 0)
            {
                CreateNewItem();
            }

            DropItem itemToSpawn = itemPool.Dequeue();
            itemToSpawn.gameObject.SetActive(true);

            itemToSpawn.transform.position = spawnPosition;

            itemToSpawn.Initialize(itemData);
        }

        public void ReturnItem(DropItem item)
        {
            item.gameObject.SetActive(false);
            itemPool.Enqueue(item);
        }
    }

    [CustomEditor(typeof(ItemSpawner))]
    public class ItemSpawnerButton : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ItemSpawner itemSpawner = (ItemSpawner)target;

            if (EditorApplication.isPlaying)
            {
                GameObject player = FindObjectOfType<CharacterController>()?.gameObject;

                if (player == null)
                {
                    EditorGUILayout.HelpBox("CharacterController가 씬에 존재하지 않습니다.", MessageType.Warning);
                    return;
                }

                if (GUILayout.Button("랜덤 아이템 생성"))
                {
                    itemSpawner.SpawnItem(
                        itemSpawner.itemDataList[Random.Range(0, itemSpawner.itemDataList.Count)],
                        player.transform.position + 
                        (player.transform.forward * 5) + 
                        (player.transform.right * Random.Range(-2f, 2f)) + 
                        (player.transform.up * 0.5f)
                    );
                }
            }
        }
    }
}
