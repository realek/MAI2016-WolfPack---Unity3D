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
    protected int m_numeric_age = 0;
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

    protected AnimalGender OppositeSex() {
        return m_gender == AnimalGender.Male ? AnimalGender.Female : AnimalGender.Male;
    }

    protected void InitAge(int value) {
        SetAge(value * value + 1); // 0 is 1;   1 is 2;   2 is 5;   3 is 10
    }

    protected void SetAge(int age) {
        m_numeric_age = age;
        if (age < 2) {
            m_age = AnimalAge.Infant;
            transform.localScale = new Vector3(.5f,.5f,.5f);
        }
        else if (age < 5) {
            m_age = AnimalAge.YoungAdult;
            transform.localScale = new Vector3(.8f, .8f, .8f);
        }
        else if (age < 8) {
            m_age = AnimalAge.Adult;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else {
            m_age = AnimalAge.Elder;
            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
    }

    public void UpdateAge() {
        m_numeric_age++;
        SetAge(m_numeric_age);
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
