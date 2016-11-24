using UnityEngine;
using System.Collections;

public class Wolf : Animal {

    [SerializeField]
    private Needs m_needs;
	// Use this for initialization
	private void Awake () {

        Heal();
        m_needs.Initialize(this);
	
	}

    public bool IsNeeded(NeedType need)
    {
        return m_needs.IsNeedTriggered(need);
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
