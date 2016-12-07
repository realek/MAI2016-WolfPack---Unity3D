using UnityEngine;
using System.Collections;

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



    [SerializeField]
    private AnimalGender m_gender;
    public AnimalGender gender
    {
        get
        {
            return m_gender;
        }
    }
    [SerializeField]
    private AnimalAge m_age;
    public AnimalAge age
    {
        get
        {
            return m_age;
        }
    }
    [SerializeField]
    private AnimalStrength m_strength;
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
    protected int m_currentHealth;
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
    /// <summary>
    /// Sets current health to max health
    /// </summary>
    protected void Heal()
    {
        m_currentHealth = m_maxHealth;
    }



}
