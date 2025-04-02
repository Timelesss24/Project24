using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ObservableDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
{
    Dictionary<TKey, TValue> dict = new();

    /// <summary>
    /// 내부 값이 변경될 때 발생하는 이벤트입니다.
    /// </summary>
    public event Action<Dictionary<TKey, TValue>> AnyValueChanged = delegate { };

    public TValue this[TKey key]
    {
        get => dict[key];
        set
        {
            dict[key] = value;
            Invoke();
        }
    }

    public int Count => dict.Count;

    public bool ContainsKey(TKey key) => dict.ContainsKey(key);

    public TValue GetValueOrDefault(TKey key, TValue defaultValue = default)
    {
        return dict.GetValueOrDefault(key, defaultValue);
    }

    public bool TryAdd(TKey key, TValue value)
    {
        if (!dict.TryAdd(key, value)) return false;
        Invoke();
        return true;
    }

    public bool TrySet(TKey key, TValue value)
    {
        dict[key] = value;
        Invoke();
        return true;
    }

    public bool TryRemove(TKey key)
    {
        if (!dict.ContainsKey(key)) return false;
        dict.Remove(key);
        Invoke();
        return true;
    }

    public void Clear()
    {
        dict.Clear();
        Invoke();
    }

    void Invoke()
    {
        AnyValueChanged.Invoke(dict);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dict.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}