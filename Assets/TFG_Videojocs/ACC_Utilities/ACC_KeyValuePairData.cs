using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ACC_KeyValuePairData<TKey, TValue>
{
    public TKey key;
    public TValue value;
    public ACC_KeyValuePairData(TKey key, TValue value)
    {
        this.key = key;
        this.value = value;
    }
}
