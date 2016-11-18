using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WolfAIController : MonoBehaviour {



    [SerializeField]
    private AIDetectionModule m_detectionModule;
    [SerializeField]
    private AIMovementModule m_movementModule;
    [SerializeField]
    private AIMemoryModule m_memoryModule;
    public bool hasPack;



    BaseRoutine CreateBehavoirTree()
    {

        BehaviorTreeBuilder treeBuilder = new BehaviorTreeBuilder();



        treeBuilder.BeginSelector("Initial State Selector")
            .BeginCondition("Non-Pack Behavior", () => { return hasPack; })
            .AttachTree(null)
            .FinishNode()
            .FinishNode();





        return null;
    }




    // Use this for initialization
    void Awake () {

        m_detectionModule.Initialize(this);
        m_movementModule.Initialize(this);
        m_memoryModule.Initialize(this);
    }

    void OnEnable()
    {
        m_detectionModule.Initialize(this);
        m_movementModule.Initialize(this);
        m_memoryModule.Initialize(this);
    }
	// Update is called once per frame
	void Update () {
	   
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
