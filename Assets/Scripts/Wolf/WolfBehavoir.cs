using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WolfBehavoir : MonoBehaviour {

    [SerializeField]
    private AIDetectionModule m_detectionModule;
    [SerializeField]
    private AIMovementModule m_movementModule;
    [SerializeField]
    private AIMemoryModule m_memoryModule;

    private string m_TargetTag;
    private GameObject m_Target;
    private List<GameObject> memorizedObjects;
    private SimpleBT m_Behavior;

    // Use this for initialization
    void Awake () {

        m_detectionModule.Initialize(this);
        m_movementModule.Initialize(this);
        m_memoryModule.Initialize(this);
        m_Behavior = GetComponent<SimpleBT>();
    }

    void OnEnable()
    {
        m_detectionModule.Initialize(this);
        m_movementModule.Initialize(this);
        m_memoryModule.Initialize(this);
        m_Behavior = GetComponent<SimpleBT>();
    }
	// Update is called once per frame
	void Update () {
	    if (m_Target == null) {
	        m_TargetTag = m_Behavior.GetTarget();
	        if (m_TargetTag != null) {
	            memorizedObjects = m_memoryModule.GetRememberedObjects();
	            if (memorizedObjects.Count > 0) {
	                foreach (GameObject mem in memorizedObjects) {
	                    if (mem.tag == m_TargetTag) {
	                        //normally would add to list of correct targets but for now just take the first one
	                        m_Target = mem;
	                        m_movementModule.Move(m_Target);
	                        break;
	                    }
	                }
	            }
	        }
	    } else {
	        if (m_movementModule.IsTargetReached()) {
	            m_Target = null;
                Debug.Log("Success");
	        }
	    }
	}

    void OnDisable()
    {
        m_detectionModule.Shutdown();
        m_movementModule.Shutdown();
        m_memoryModule.Shutdown();
    }

    void OnDrawGizmosSelected()
    {
        m_detectionModule.DrawGizmos();
        m_movementModule.DrawGizmos();
    }
}
