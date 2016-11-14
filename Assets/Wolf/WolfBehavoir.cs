using UnityEngine;
using System.Collections;

public class WolfBehavoir : MonoBehaviour {

    [SerializeField]
    private AIDetectionModule m_detectionModule;
    [SerializeField]
    private AIMovementModule m_movementModule;

    // Use this for initialization
    void Awake () {

        m_detectionModule.Initialize(this);
        m_movementModule.Initialize(this);
	
	}

    void OnEnable()
    {
        m_detectionModule.Initialize(this);
        m_movementModule.Initialize(this);
    }
	// Update is called once per frame
	void Update () {
	
	}

    void OnDisable()
    {
        m_detectionModule.Initialize(this);
        m_movementModule.Initialize(this);
    }

    void OnDrawGizmosSelected()
    {
        m_detectionModule.DrawGizmos();
        m_movementModule.DrawGizmos();
    }
}
