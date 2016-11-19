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
    BaseRoutine behaviorTree;


    BaseRoutine CreateBehaviorTree()
    {

        BehaviorTreeBuilder treeBuilder = new BehaviorTreeBuilder();

        BaseRoutine nonPackBehavior = treeBuilder
            .BeginSequence("move around" )
            .AddAction("move to current target", () => 
            {
                Debug.Log("Called movement" );
                return RoutineState.Succeded;
            })
            .FinishNode();

        treeBuilder
            .BeginRepeater("Tree repeater", 0)
            .BeginSelector("Initial State Selector")
            .BeginCondition("Non-Pack Behavior", () =>
            {
                return !hasPack;
            })
            .AttachTree(nonPackBehavior)
            .FinishNode()
            .FinishNode()
            .FinishNode();

        return treeBuilder;
    }




    // Use this for initialization
    void Awake () {

        m_detectionModule.Initialize(this);
        m_movementModule.Initialize(this);
        m_memoryModule.Initialize(this);
        behaviorTree = CreateBehaviorTree();
        behaviorTree.Start();
    }

    void OnEnable()
    {
        m_detectionModule.Initialize(this);
        m_movementModule.Initialize(this);
        m_memoryModule.Initialize(this);
    }
	// Update is called once per frame
	void Update () {

        behaviorTree.Tick();
	   
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
