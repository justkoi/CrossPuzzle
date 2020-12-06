using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 순회용, 간단한 기능만 정의
/// </summary>
public class CustomDictionary<TKey, TValue> {
    private List<TKey> m_KeyList = new List<TKey>();
    //public List<TValue> m_ValueList = new List<TValue>();
    private Dictionary<TKey, TValue> m_Dictionary = new Dictionary<TKey, TValue>();

    public Dictionary<TKey, TValue>.ValueCollection Values
    {
        get
        {
            return m_Dictionary.Values;
        }
    }

    public int Count
    {
        get
        {
            return m_Dictionary.Count;
        }
    }

    public void Add(TKey Key, TValue Value)
    {
        ///체크는 외부에서 하고 들어오는걸로 간주
        //if (m_Dictionary.ContainsKey(Key))
        //{
        //    return;
        //}
        //else
        //{
            m_KeyList.Add(Key);
            //m_ValueList.Add(Value);
        m_Dictionary.Add(Key, Value);
        //}
    }

    public void Remove(TKey Key)
    {
    //    if (!m_Dictionary.ContainsKey(Key))
    //    {
    //        return;
    //    }
    //    else
    //    {
            m_KeyList.Remove(Key);
            //m_ValueList.Remove(m_Dictionary[Key]);
            m_Dictionary.Remove(Key);
        //}
    }

    public void Clear()
    {
        m_Dictionary.Clear();
        m_KeyList.Clear();
    }

    public bool ContainsKey(TKey Key)
    {
        return m_Dictionary.ContainsKey(Key);
    }

    public TValue ElementAt(int nIndex)
    {
        if (nIndex < 0 || nIndex >= m_KeyList.Count)
            return default(TValue);

        return m_Dictionary[m_KeyList[nIndex]];
    }

    public TKey KeyAt(int nIndex)
    {
        if (nIndex < 0 || nIndex >= m_KeyList.Count)
            return default(TKey);

        return m_KeyList[nIndex];
    }

    public TValue this[TKey key]
    {
        get
        {
            return m_Dictionary[key];
        }

        set
        {

            m_Dictionary[key] = value;
        }
    }

}
