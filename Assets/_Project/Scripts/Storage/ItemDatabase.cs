using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public static class ItemDatabase
    {
        static Dictionary<SerializableGuid, ItemDetails> itemDetailsDictionary;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Initialize()
        {
            itemDetailsDictionary = new Dictionary<SerializableGuid, ItemDetails>();

            var itemDetails = Resources.LoadAll<ItemDetails>("Item");
            foreach (var item in itemDetails)
            {
                Debug.Log($"Loaded item details: {item.Id.ToGuid()}");
                itemDetailsDictionary.Add(item.Id, item);
            }
            Debug.Log($"Loaded {itemDetails.Length} item details");
        }

        public static ItemDetails GetDetailsById(SerializableGuid id)
        {
            Debug.Log($"Loaded item details: {id.ToGuid()}");
            try
            {
                return itemDetailsDictionary[id];
            }
            catch
            {
                Debug.LogError($"Cannot find item details with id {id.ToGuid()}");
                return null;
            }
        }
    }
}