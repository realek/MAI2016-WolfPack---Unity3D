using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> m_keys = new List<TKey>();

    [SerializeField]
    private List<TValue> m_values = new List<TValue>();

    public void OnBeforeSerialize()
    {
        m_keys.Clear();
        m_values.Clear();
        if (typeof(TKey).IsSubclassOf(typeof(UnityEngine.Object)) || typeof(TKey) == typeof(UnityEngine.Object))
        {
            foreach (var element in this)
            {
                if (element.Key != null)
                {
                    m_keys.Add(element.Key);
                    m_values.Add(element.Value);
                }

            }
        }
        else
        {
            foreach (var element in this)
            {
                m_keys.Add(element.Key);
                m_values.Add(element.Value);
            }
        }
    }

    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        this.Clear();

        if (m_keys.Count != m_values.Count)
            throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

        for (int i = 0; i < m_keys.Count; i++)
            this.Add(m_keys[i], m_values[i]);
    }
}
