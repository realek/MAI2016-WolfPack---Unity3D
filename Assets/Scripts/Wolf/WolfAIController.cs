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
    public bool onPatrol;
    private BaseRoutine m_behaviorTree;
    private Wolf m_wolf;
    public GameObject food;
    public GameObject drink;
    public GameObject sleepArea;
    public Wolf otherWolf;
    public GameObject[] patrolPoints;
    public int currentPatrolPointIDX = -1;
    private GameObject m_currentTarget;


    BaseRoutine CreateBehaviorTree()
    {

        BehaviorTreeBuilder treeBuilder = new BehaviorTreeBuilder();


        BaseRoutine moveToTarget = treeBuilder
    .BeginSequence("Move Towards")
    .AddAction("move to current target", () =>
    {
        if (m_currentTarget == null || m_currentTarget == gameObject)
            return RoutineState.Failed;

        m_movementModule.Move(m_currentTarget);
        if (m_movementModule.reachedTarget && !m_movementModule.unreachableTarget)
            return RoutineState.Succeded;
        else if (!m_movementModule.reachedTarget && !m_movementModule.unreachableTarget)
            return RoutineState.Running;
        else
            return RoutineState.Failed;
    })
    .FinishNode();


        BaseRoutine needsBlock_SelectorContainer = treeBuilder
            .BeginSelector("Needs Selection")
            .BeginCondition("Energy", () =>
            {
                if (m_wolf.needs.IsNeedTriggered(NeedType.Energy))
                {
                    m_currentTarget = sleepArea;
                    return true;
                }
                else return false;
            })
            .BeginSequence("Go Rest")
            .AttachTree(moveToTarget)
            .AddAction("Rest", () =>
            {
                Debug.Log("Rested");
                m_currentTarget = gameObject;
                m_wolf.needs.SetNeed(NeedType.Energy, 100);
                return RoutineState.Succeded;

            })
            .FinishNode()
            .FinishNode()
            .BeginCondition("Food", () =>
            {
                if (m_wolf.needs.IsNeedTriggered(NeedType.Hunger))
                {
                    m_currentTarget = food;
                    return true;
                }
                else return false;
            })
            .BeginSequence("Go Eat")
            .AttachTree(moveToTarget)
            .AddAction("Eat", () =>
            {
                Debug.Log("Feast");
                m_currentTarget = gameObject;
                m_wolf.needs.SetNeed(NeedType.Hunger, 100);
                return RoutineState.Succeded;

            })
            .FinishNode()
            .FinishNode()
            .BeginCondition("Water", () =>
            {
                if (m_wolf.needs.IsNeedTriggered(NeedType.Thirst))
                {
                    m_currentTarget = drink;
                    return true;
                }
                else return false;
            })
            .BeginSequence("Go Drink")
            .AttachTree(moveToTarget)
            .AddAction("Drink", () =>
            {
                Debug.Log("Drink");
                m_currentTarget = gameObject;
                m_wolf.needs.SetNeed(NeedType.Thirst, 100);
                return RoutineState.Succeded;

            })
            .FinishNode()
            .FinishNode()
            .FinishNode();

        BaseRoutine patrolBehavior_containerSequence = treeBuilder
            .BeginSequence("Patrol Sequence")
            .AddAction("Select Waypoint", () => 
            {
                if (currentPatrolPointIDX == -1)
                    return RoutineState.Failed;
                m_currentTarget = patrolPoints[currentPatrolPointIDX];
                return RoutineState.Succeded;
            })
            .AttachTree(moveToTarget)
            .AddAction("Next Waypoint",()=>
            {
                currentPatrolPointIDX++;
                if (currentPatrolPointIDX == patrolPoints.Length)
                    currentPatrolPointIDX = -1;
                return RoutineState.Succeded;
            })
            .FinishNode();

        BaseRoutine soloBehavior_SelectorContainer = treeBuilder
            .BeginSelector("Solo behavior")
            .BeginCondition("Has Needs", () => { return m_wolf.needs.InNeed(); })
            .AttachTree(needsBlock_SelectorContainer)
            .FinishNode()
            .BeginCondition("On Patrol", () => { return onPatrol; })
            .AttachTree(patrolBehavior_containerSequence)
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

        //return final behavior tree
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
            .AttachTree(soloBehavior_SelectorContainer)
            .FinishNode()
            .FinishNode()
            .FinishNode();

        return treeBuilder;
    }




    // Use this for initialization
    private void Start () {

        m_currentTarget = gameObject;
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
