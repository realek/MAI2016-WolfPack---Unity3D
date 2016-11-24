using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Wolf))]
public class WolfAIController : MonoBehaviour {



    [SerializeField]
    private AIDetectionModule m_detectionModule;
    [SerializeField]
    private AIMovementModule m_movementModule;
    public bool hasPack;
    BaseRoutine behaviorTree;
    private Wolf m_wolf;
    public GameObject food;
    public GameObject drink;
    public GameObject sleepArea;
    public Wolf otherWolf;
    private GameObject currentTarget;
    BaseRoutine CreateBehaviorTree()
    {

        BehaviorTreeBuilder treeBuilder = new BehaviorTreeBuilder();

        BaseRoutine needsBehavoir = treeBuilder
            .BeginSelector("Needs Selection")
            .BeginCondition("Energy", () =>
            {
                
                return m_wolf.IsNeeded(NeedType.Energy);
            })
            .AddAction("Rest"()=>{ });

        BaseRoutine nonPackBehavior = treeBuilder
            .BeginSequence("Move Towards")
            .AddAction("move to current target", () => 
            {
                if (currentTarget == null)
                    return RoutineState.Failed;

                if (m_movementModule.Move(currentTarget))
                {
                    return RoutineState.Running;
                }
                else
                {
                    if (m_movementModule.reachedTarget && !m_movementModule.unreachableTarget)
                        return RoutineState.Succeded;
                    else
                        return RoutineState.Failed;
                }
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
    private void Start () {

        m_wolf = GetComponent<Wolf>();
        m_detectionModule.Initialize(this);
        m_movementModule.Initialize(this);
        behaviorTree = CreateBehaviorTree();
        behaviorTree.Start();
    }

    private void OnEnable()
    {
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
        m_detectionModule.Shutdown();
        m_movementModule.Shutdown();
    }

    void OnDrawGizmosSelected()
    {
        m_detectionModule.DrawGizmos();
        m_movementModule.DrawGizmos();
    }
}
