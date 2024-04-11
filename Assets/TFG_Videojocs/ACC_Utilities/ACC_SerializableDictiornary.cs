using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class ACC_SerializableDictiornary<TKey, TValue>: ICloneable
{
    [System.Serializable]
    public class ACC_KeyValuePair
    {
        public TKey key;
        public TValue value;

        public ACC_KeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (ACC_KeyValuePair)obj;

            return key.Equals(other.key) && value.Equals(other.value);
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ key.GetHashCode();
                hash = (hash * 16777619) ^ value.GetHashCode();
                return hash;
            }
        }
    }
    
    public List<ACC_KeyValuePair> Items = new();

    public void AddOrUpdate(TKey key, TValue value)
    {
        var existingItem = Items.Find(item => item.key.Equals(key));
        if (existingItem != null)
        {
            existingItem.value = value;
        }
        else
        {
            Items.Add(new ACC_KeyValuePair(key, value));
        }
    }

    public object Clone()
    {
        ACC_SerializableDictiornary<TKey, TValue> clone = new ACC_SerializableDictiornary<TKey, TValue>();
        foreach (var item in Items)
        {
            clone.Items.Add(new ACC_KeyValuePair(item.key, item.value));
        }
        return clone;
    }
}
