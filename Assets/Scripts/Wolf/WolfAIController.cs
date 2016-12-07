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
    private BaseRoutine m_behaviorTree;
    private Wolf m_wolf;
    public GameObject food;
    public GameObject drink;
    public GameObject sleepArea;
    public Wolf otherWolf;
    private GameObject currentTarget;


    BaseRoutine CreateBehaviorTree()
    {

        BehaviorTreeBuilder treeBuilder = new BehaviorTreeBuilder();


        BaseRoutine moveToTarget = treeBuilder
    .BeginSequence("Move Towards")
    .AddAction("move to current target", () =>
    {
        if (currentTarget == null)
            return RoutineState.Failed;

        m_movementModule.Move(currentTarget);
        if (m_movementModule.reachedTarget && !m_movementModule.unreachableTarget)
            return RoutineState.Succeded;
        else if (!m_movementModule.reachedTarget && !m_movementModule.unreachableTarget)
            return RoutineState.Running;
        else
            return RoutineState.Failed;
    })
    .FinishNode();


        BaseRoutine needsBehavoir = treeBuilder
            .BeginSelector("Needs Selection")
            .BeginCondition("Energy", () =>
            {
                if (m_wolf.needs.IsNeedTriggered(NeedType.Energy))
                {
                    currentTarget = sleepArea;
                    return true;
                }
                else return false;
            })
            .BeginSequence("Go Rest")
            .AttachTree(moveToTarget)
            .AddAction("Rest", () =>
            {
                Debug.Log("Rested");
                m_wolf.needs.SetNeed(NeedType.Energy, 100);
                return RoutineState.Succeded;

            })
            .FinishNode()
            .FinishNode()
            .BeginCondition("Food", () =>
            {
                if (m_wolf.needs.IsNeedTriggered(NeedType.Hunger))
                {
                    currentTarget = food;
                    return true;
                }
                else return false;
            })
            .BeginSequence("Go Eat")
            .AttachTree(moveToTarget)
            .AddAction("Eat", () =>
            {
                Debug.Log("Feast");
                m_wolf.needs.SetNeed(NeedType.Hunger, 100);
                return RoutineState.Succeded;

            })
            .FinishNode()
            .FinishNode()
            .BeginCondition("Water", () =>
            {
                if (m_wolf.needs.IsNeedTriggered(NeedType.Thirst))
                {
                    currentTarget = drink;
                    return true;
                }
                else return false;
            })
            .BeginSequence("Go Drink")
            .AttachTree(moveToTarget)
            .AddAction("Drink", () =>
            {
                Debug.Log("Drink");
                m_wolf.needs.SetNeed(NeedType.Thirst, 100);
                return RoutineState.Succeded;

            })
            .FinishNode()
            .FinishNode()
            .FinishNode();



        BaseRoutine packBehavior = treeBuilder
            .BeginSequence("move with pack")
            .AddAction("Pack movement", () =>
             {
                // Debug.Log("Called pack movement");
                 return RoutineState.Succeded;
             })
             .FinishNode();


        treeBuilder
            .BeginRepeater("Tree repeater", 0)
            .BeginSelector("Initial State Selector")
            .BeginCondition("Pack Behavior", () =>
            {
                return hasPack;
            })
            .AttachTree(packBehavior) // pack behavior no implemented currently
            .FinishNode()
            .BeginCondition("Non-Pack Behvaior", () =>
            {
                return !hasPack;
            })
            .AttachTree(needsBehavoir)
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
        m_behaviorTree = CreateBehaviorTree();
        if (m_behaviorTree == null)
        {
            return;
        }
        m_behaviorTree.Start();
    }

    private void OnEnable()
    {
        m_detectionModule.Initialize(this);
        m_movementModule.Initialize(this);
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_behaviorTree == null)
        {
            enabled = false;
            return;
        }
        m_behaviorTree.Tick();
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
