using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AIDetectionMovementBlock))]
public class AIBehaviour : MonoBehaviour {

    private AIDetectionMovementBlock m_agent;
	// Use this for initialization
	void Start ()
    {
        m_agent = GetComponent<AIDetectionMovementBlock>();
        if (m_agent == null || !m_agent.enabled)
        {
            Debug.LogError("AI agent is missing or disabled,", gameObject);
            enabled = false;
            return;

          
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
