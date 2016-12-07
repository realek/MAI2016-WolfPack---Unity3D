using UnityEngine;
using System.Collections;

public class Animal : MonoBehaviour {

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
    /// <summary>
    /// Sets current health to max health
    /// </summary>
    protected void Heal()
    {
        m_currentHealth = m_maxHealth;
    }



}
