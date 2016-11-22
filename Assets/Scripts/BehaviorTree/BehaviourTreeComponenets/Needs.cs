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
    }


    /// <summary>
    /// Serializeable dummy class, must be removed after tweaking is done
    /// </summary>
    [System.Serializable]
    private class SerializeableNeedtypeNeedDictionary : SerializableDictionary<NeedType, Need> { }
    ///

    /// <summary>
    /// Inner class used to process needs
    /// </summary>
    [System.Serializable]
    private class Need
    {
        public float value;
        public float decayValue;
    }

    [SerializeField]
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


/*
public class Needs : MonoBehaviour {

    private const int MIN_VALUE = 0;
    private const int MAX_VALUE = 100;

    [SerializeField] private float _hunger = MIN_VALUE;
    [SerializeField] private float _thirst = MIN_VALUE;
    [SerializeField] private float _playfulness = MIN_VALUE;
    [SerializeField] private float _fear = MIN_VALUE;
    [SerializeField] private float _energy = MAX_VALUE;
    [SerializeField] private float _curiosity = MAX_VALUE;
    [SerializeField] private float _health = MAX_VALUE;

    public void SetNeed(WolfNeedType need, float value) {
        switch (need) {
          case WolfNeedType.Hunger: _hunger = value; break;
          case WolfNeedType.Playfulness: _playfulness = value; break;
          case WolfNeedType.Fear: _fear = value; break;
          case WolfNeedType.Energy: _energy = value; break;
          case WolfNeedType.Curiousity: _curiosity = value; break;
          case WolfNeedType.Thirst: _thirst = value; break;
          case WolfNeedType.Health: _health = value; break;
          default: Debug.Log("Unknown need set"); break;
        }
    }

    public void EditNeed(WolfNeedType need, float value) {
        switch (need) {
            case WolfNeedType.Hunger: _hunger += value; if (_hunger < MIN_VALUE) _hunger = MIN_VALUE; if (_hunger > MAX_VALUE) _hunger = MAX_VALUE; break;
            case WolfNeedType.Playfulness: _playfulness += value; if (_playfulness < MIN_VALUE) _playfulness = MIN_VALUE; if (_playfulness > MAX_VALUE) _playfulness = MAX_VALUE; break;
            case WolfNeedType.Fear: _fear += value; if (_fear < MIN_VALUE) _fear = MIN_VALUE; if (_fear > MAX_VALUE) _fear = MAX_VALUE; break;
            case WolfNeedType.Energy: _energy += value; if (_energy < MIN_VALUE) _energy = MIN_VALUE; if (_energy > MAX_VALUE) _energy = MAX_VALUE; break;
            case WolfNeedType.Curiousity: _curiosity += value; if (_curiosity < MIN_VALUE) _curiosity = MIN_VALUE; if (_curiosity > MAX_VALUE) _curiosity = MAX_VALUE; break;
            case WolfNeedType.Thirst: _thirst += value; if (_thirst < MIN_VALUE) _thirst = MIN_VALUE; if (_thirst > MAX_VALUE) _thirst = MAX_VALUE; break;
            case WolfNeedType.Health: _health += value; if (_health < MIN_VALUE) _health = MIN_VALUE; if (_health > MAX_VALUE) _health = MAX_VALUE; break;
            default: Debug.Log("Unknown need set"); break;
        }
    }

    public float GetNeed(WolfNeedType need) {
        switch (need) {
            case WolfNeedType.Hunger: return _hunger;
            case WolfNeedType.Playfulness: return _playfulness;
            case WolfNeedType.Fear: return _fear;
            case WolfNeedType.Energy: return _energy;
            case WolfNeedType.Curiousity: return _curiosity;
            case WolfNeedType.Thirst: return _thirst;
            case WolfNeedType.Health: return _health;
            default: Debug.Log("Unknown need requested"); return MIN_VALUE;
        }
    }
}
*/
