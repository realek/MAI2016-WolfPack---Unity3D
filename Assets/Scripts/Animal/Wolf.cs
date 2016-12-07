using UnityEngine;
using System.Collections;

public class Wolf : Animal {

    public Needs needs
    {
        get
        {
            return m_needs;
        }
    }

	// Use this for initialization
	private void Awake () {

        Heal();
        m_needs.Initialize(this);
	
	}

    private void OnEnable()
    {
        m_needs.Initialize(this);
    }

    private void OnDisable()
    {
        m_needs.Shutdown();
    }

}
