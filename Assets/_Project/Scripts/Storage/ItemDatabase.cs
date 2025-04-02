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
                itemDetailsDictionary.Add(item.Id, item);
            }
        }

        public static ItemDetails GetDetailsById(SerializableGuid id)
        {
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