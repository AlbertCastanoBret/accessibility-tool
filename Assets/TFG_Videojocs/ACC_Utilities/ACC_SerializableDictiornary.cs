using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ACC_SerializableDictiornary<TKey, TValue>
{
    [System.Serializable]
    public class KeyValuePair
    {
        public TKey Key;
        public TValue Value;

        public KeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
    [System.Serializable]
    public class ACC_KeyValuePair<TKey, TValue>
    {
        public TKey key;
        public TValue value;

        public ACC_KeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
    }
    
    public List<KeyValuePair> Items = new();

    public void AddOrUpdate(TKey key, TValue value)
    {
        var existingItem = Items.Find(item => item.Key.Equals(key));
        if (existingItem != null)
        {
            existingItem.Value = value;
        }
        else
        {
            Items.Add(new KeyValuePair(key, value));
        }
    }
}
