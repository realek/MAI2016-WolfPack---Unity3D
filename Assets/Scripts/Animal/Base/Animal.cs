using UnityEngine;
using CustomConsts;

public enum AnimalGender
{
    Male,
    Female
}

public enum AnimalAge
{
    Infant,
    YoungAdult,
    Adult,
    Elder
}

public enum AnimalStrength
{
    Weak,
    Medium,
    Strong,
    VeryStrong
}

public class Animal : MonoBehaviour {


    protected AnimalGroup currentGroup = null;
    [SerializeField]
    protected AnimalGender m_gender;
    public AnimalGender gender
    {
        get
        {
            return m_gender;
        }
    }
    [SerializeField]
    protected AnimalAge m_age;
    public AnimalAge age
    {
        get
        {
            return m_age;
        }
    }
    [SerializeField]
    protected AnimalStrength m_strength;
    public AnimalStrength strength
    {
        get
        {
            return m_strength;
        }
    }
    public bool Alive
    {
        get
        {
            if (m_currentHealth <= 0)
                return false;
            else
                return true;
        }
    }
    public int m_currentHealth;
    [SerializeField]
    protected int m_maxHealth;
    [SerializeField]
    protected Needs m_needs;
    public Needs needs
    {
        get
        {
            return m_needs;
        }
    }

    [HideInInspector]
    public float WaitTime;

    protected int CarcassQnt;

    /// <summary>
    /// Sets current health to max health
    /// </summary>
    protected void Heal()
    {
        m_currentHealth = m_maxHealth;
    }

    public void DealtDmg(int dmg) {
        m_currentHealth -= dmg;
        Debug.Log("Hit! Curr health: " + m_currentHealth);
        if (m_currentHealth < 1) {
            GameObject myCarcass = (GameObject) Instantiate((Resources.Load("Carcass")));
            myCarcass.transform.position = transform.position + Vector3.up * 0.1f;
            myCarcass.GetComponent<Perishable>().SetExpiration(GlobalVars.AllCarcassExp);
            myCarcass.GetComponent<Perishable>().SetQuantity(CarcassQnt);
            Destroy(gameObject);
        }
    }

    public void SetGroup(AnimalGroup group)
    {
        currentGroup = group;
        currentGroup.AddMember(this);
    }

    public void LeaveGroup()
    {
        currentGroup.RemoveMember(this);
        currentGroup = null;
    }

    public Vector3 GetGroupCenter() {
        return currentGroup.GetGroupPosition(this);
    }
}
