using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WolfAIController : MonoBehaviour {



    [SerializeField]
    private AIDetectionModule m_detectionModule;
    [SerializeField]
    private AIMovementModule m_movementModule;
    public bool hasPack;
    BaseRoutine behaviorTree;
    public Needs needs;


    BaseRoutine CreateBehaviorTree()
    {

        BehaviorTreeBuilder treeBuilder = new BehaviorTreeBuilder();

        BaseRoutine nonPackBehavior = treeBuilder
            .BeginSequence("move around")
            .AddAction("move to current target", () => 
            {
                Debug.Log("Called movement");
                return RoutineState.Succeded;
            })
            .FinishNode();

        BaseRoutine packBehavior = treeBuilder
            .BeginSequence("move with pack")
            .AddAction("Pack movement", () =>
             {
                 Debug.Log("Called pack movement");
                 return RoutineState.Succeded;
             })
             .FinishNode();


        treeBuilder
            .BeginRepeater("Tree repeater", 0)
            .BeginSelector("Initial State Selector")
            .BeginCondition("Non-Pack Behavior", () =>
            {
                Debug.Log("Called non-pack Condition");
                return !hasPack;
            })
            .AttachTree(nonPackBehavior)
            .FinishNode()
            .BeginCondition("Pack Behvior", () =>
            {
                Debug.Log("Called pack Condition");
                return hasPack;
            })
            .AttachTree(packBehavior)
            .FinishNode()
            .FinishNode()
            .FinishNode();

        return treeBuilder;
    }




    // Use this for initialization
    private void Awake () {

        needs.Initialize(this);
        m_detectionModule.Initialize(this);
        m_movementModule.Initialize(this);
        behaviorTree = CreateBehaviorTree();
        behaviorTree.Start();
    }

    private void OnEnable()
    {
        needs.Initialize(this);
        m_detectionModule.Initialize(this);
        m_movementModule.Initialize(this);
    }

    // Update is called once per frame
    private void Update()
    {
        behaviorTree.Tick();
    }

    private void OnDisable()
    {
        needs.Shutdown();
        m_detectionModule.Shutdown();
        m_movementModule.Shutdown();
    }

    void OnDrawGizmosSelected()
    {
        m_detectionModule.DrawGizmos();
        m_movementModule.DrawGizmos();
    }
}
