using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NeedType
{
    Hunger =  0,
    Energy = 1,
    Thirst = 2,
    Social = 3,
    NEED_COUNT = 4
}

[System.Serializable]
public class Needs
{
    private const int MIN_NEED_VALUE = 0;
    private const int MAX_NEED_VALUE = 100;
    /// <summary>
    /// Inner struct used to describe needs
    /// </summary>
    [System.Serializable]
    private struct NeedDescription
    {
        public NeedType type;
        [Range(MIN_NEED_VALUE,MAX_NEED_VALUE)]
        public float startValue;
        public float decayValue;
        public float decayRate;
        public float threshold;
        [Tooltip("0 is highest priority.")]
        public int priority;
    }


    /// <summary>
    /// Inner class used to process needs
    /// </summary>
    [System.Serializable]
    private class Need
    {
        public float value;
        public float decayValue;
        public float threshold;
        public int priority;

        /// <summary>
        /// returns true if the need value is below its theshold
        /// </summary>
        public bool Triggered
        {
            get
            {
                if (value <= threshold)
                    return true;
                else
                    return false;
            }
        }
    }


    /// <summary>
    /// Serializeable dummy class, must be removed after tweaking is done
    /// </summary>
    [System.Serializable]
    private class SerializeableNeedtypeNeedDictionary : SerializableDictionary<NeedType, Need> { }


    [SerializeField,Tooltip("Start values for needs, current need count is specified in the NeedType enum as NEED_COUNT")]
    private List<NeedDescription> m_StartValues;
    [SerializeField]
    private SerializeableNeedtypeNeedDictionary m_needs;
    private MonoBehaviour m_owner;
    private Coroutine m_executor;
    private bool m_init;
    private WaitForSeconds m_minDecayYield;
    private float m_minDecayRate;
    /// <summary>
    /// Used to initializes the needs and provides the owner of the instance
    /// </summary>
    /// <param name="owner"></param>
    public void Initialize(MonoBehaviour owner)
    {
        if (!m_init)
        {
            m_owner = owner;
            m_needs = new SerializeableNeedtypeNeedDictionary();

            if (m_StartValues != null && m_StartValues.Count == (int)NeedType.NEED_COUNT)
            {
                //sort needs by decay rate
                m_StartValues.Sort((need1, need2) => need1.decayRate.CompareTo(need2.decayRate));

                for (int i = 0; i < m_StartValues.Count; i++) 
                {
                    Need need = new Need();
                    need.value = m_StartValues[i].startValue;
                    need.decayValue = m_StartValues[i].decayValue;
                    need.threshold = m_StartValues[i].threshold;
                    if (i == 0)
                    {
                        m_minDecayYield = new WaitForSeconds(m_StartValues[i].decayRate);
                        m_minDecayRate = m_StartValues[i].decayRate;

                    }
                    else
                        need.decayValue /= (m_StartValues[i].decayRate/m_minDecayRate);

                    m_needs.Add(m_StartValues[i].type, need);
                }
            }
            else
            {
                throw new System.Exception("Missing one or more needs from the list, current need count: "
                    +m_needs.Count+" expected: "+(int)NeedType.NEED_COUNT);
            }

            m_executor = m_owner.StartCoroutine(NeedsTicker());
            m_init = true;
        }
     
    }

    public void Shutdown()
    {
        if (m_executor != null)
            m_owner.StopCoroutine(m_executor);
        m_executor = null;
        m_init = false;

    }
    /// <summary>
    /// Get the specified need value
    /// </summary>
    /// <param name="need"></param>
    /// <returns></returns>
    public float GetNeed(NeedType need)
    {
        return m_needs[need].value;
    }

    public bool IsNeedTriggered(NeedType need)
    {
        return m_needs[need].Triggered;
    }

    /// <summary>
    /// Set the specified need to the provided value, all values are clamped according to the MIN_NEED_VALUE and MAX_NEED_VALUE constants.
    /// </summary>
    /// <param name="need"></param>
    /// <param name="value"></param>
    public void SetNeed(NeedType need, float value)
    {
        if (value > MAX_NEED_VALUE || value < MIN_NEED_VALUE)
            value = Mathf.Clamp(value, MIN_NEED_VALUE, MAX_NEED_VALUE);
        m_needs[need].value = value;
    }

    /// <summary>
    /// Modify a need by providing an positive or negative value
    /// </summary>
    /// <param name="need"></param>
    /// <param name="modValue"></param>
    public void ModNeed(NeedType need, float modValue)
    {
        Need cValue = m_needs[need];
        cValue.value += modValue;
        if (cValue.value > MAX_NEED_VALUE || cValue.value < MIN_NEED_VALUE)
            cValue.value = Mathf.Clamp(cValue.value, MIN_NEED_VALUE, MAX_NEED_VALUE);
    }

    private IEnumerator NeedsTicker()
    {
        while (true)
        {
            yield return m_minDecayYield;
            foreach (KeyValuePair<NeedType, Need> need in m_needs)
            {
                if (need.Value.value > MIN_NEED_VALUE)
                    need.Value.value -= need.Value.decayValue;
                else if (need.Value.value < MIN_NEED_VALUE)
                    need.Value.value = MIN_NEED_VALUE;
            }
        }

    }
}

