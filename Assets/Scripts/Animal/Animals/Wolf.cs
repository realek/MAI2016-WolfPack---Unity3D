using UnityEngine;
using System.Collections;

public enum WolfPackRole
{
    None,
    Alpha,
    Beta,
    Omega
}



public class Wolf : Animal {


    [SerializeField]
    private WolfPackRole m_packRole;
    public WolfPackRole packRole
    {
        get
        {
            return m_packRole;
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
