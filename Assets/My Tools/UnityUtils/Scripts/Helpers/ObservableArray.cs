using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

public interface IObservableArray<T>
{
    event Action<T[]> AnyValueChanged;

    int Count { get; }
    T this[int index] { get; }

    void Swap(int index1, int index2);
    void Clear();
    bool TryAdd(T item);
    bool TryAddAt(int index, T item);
    bool TryRemove(T item);
    bool TryRemoveAt(int index);
}

[Serializable]
public class ObservableArray<T> : IObservableArray<T>
{

    [FormerlySerializedAs("items")]
    public T[] Items;

    public event Action<T[]> AnyValueChanged = delegate { };
    public int Count => Items.Count(i => i != null);
    public int Length => Items.Length;
    public T this[int index] => Items[index];
    
    public ObservableArray(int size = 20, IList<T> initialList = null)
    {
        Items = new T[size];
        if (initialList != null)
        {
            initialList.Take(size).ToArray().CopyTo(Items, 0);
            Invoke();
        }
    }

    void Invoke() => AnyValueChanged.Invoke(Items);

    public void Swap(int index1, int index2)
    {
        (Items[index1], Items[index2]) = (Items[index2], Items[index1]);
        Invoke();
    }

    public void Clear()
    {
        Items = new T[Items.Length];
        Invoke();
    }

    public bool TryAdd(T item)
    {
        for (var i = 0; i < Items.Length; i++)
        {
            if (TryAddAt(i, item)) return true;
        }
        return false;
    }

    public bool TryAddAt(int index, T item)
    {
        if (index < 0 || index >= Items.Length) return false;

        if (Items[index] != null) return false;

        Items[index] = item;
        Invoke();
        return true;
    }

    public bool TryRemove(T item)
    {
        for (var i = 0; i < Items.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(Items[i], item) && TryRemoveAt(i)) return true;
        }
        return false;
    }

    public bool TryRemoveAt(int index)
    {
        if (index < 0 || index >= Items.Length) return false;

        if (Items[index] == null) return false;

        Items[index] = default;
        Invoke();
        return true;
    }
}