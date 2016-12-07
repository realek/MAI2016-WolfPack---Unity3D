using UnityEngine;
using System.Collections;

public class Wolf : Animal {

    [System.Serializable]
    private sealed class WolfData
    {

    }


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
