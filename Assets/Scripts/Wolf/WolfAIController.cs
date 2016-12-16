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
    public GameObject[] patrolPoints;
    private int currentPatrolPointIDX = -1;
    private GameObject m_currentTarget;
    private bool m_onPatrol = false;

    private const float WANDER_CHANCE = 0.35f;
    private const float PATROL_CHANCE = 0.25f;
    private const float BEHAVIOR_TREE_UPDATE_RATE = 0.2f; // 5 times a second is enough
    private WaitForSeconds m_behaviorTreeTick;
    private Coroutine treeRunner;
    BaseRoutine CreateBehaviorTree()
    {

        BehaviorTreeBuilder treeBuilder = new BehaviorTreeBuilder();


        BaseRoutine moveToTarget_SequenceContainer = treeBuilder
    .BeginSequence("Move TO")
    .AddAction("Movement", () =>
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

        //Needs behavior container
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
            .AttachTree(moveToTarget_SequenceContainer)
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
            .AttachTree(moveToTarget_SequenceContainer)
            .AddAction("Eat", () =>
            {
                Debug.Log("Feast");
                m_currentTarget = gameObject;
                if (m_currentTarget.tag == "Food") m_currentTarget.GetComponent<Perishable>().Reduce(10);
                m_wolf.needs.ModNeed(NeedType.Hunger, 10);
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
            .AttachTree(moveToTarget_SequenceContainer)
            .AddAction("Drink", () =>
            {
                Debug.Log("Drink");
                m_currentTarget = gameObject;
                //check if object still exists because it might have expired
                if (m_currentTarget.tag == "Water") m_currentTarget.GetComponent<Perishable>().Reduce(5);
                m_wolf.needs.ModNeed(NeedType.Thirst, 5);
                m_wolf.needs.SetNeed(NeedType.Thirst, 100);
                return RoutineState.Succeded;

            })
            .FinishNode()
            .FinishNode()
            .FinishNode();

        //Patrol behavior
        BaseRoutine patrolBehaviorBlock_SequenceContainer = treeBuilder
            .BeginSequence("Patrol Sequence")
            .AddAction("Select Waypoint", () => 
            {
                if (currentPatrolPointIDX == -1)
                    return RoutineState.Failed;
                m_currentTarget = patrolPoints[currentPatrolPointIDX];
                return RoutineState.Succeded;
            })
            .AttachTree(moveToTarget_SequenceContainer)
            .AddAction("Next Waypoint",()=>
            {
                currentPatrolPointIDX++;
                if (currentPatrolPointIDX == patrolPoints.Length)
                {
                    currentPatrolPointIDX = -1;
                    m_onPatrol = false;
                }
                return RoutineState.Succeded;
            })
            .FinishNode();
        
        ////Protect behavior
        //BaseRoutine protectBehaviorBlock_SelectorContainer = treeBuilder
        //    .BeginSelector("Defend Target From")
        //    .FinishNode();

        //Solo behavior container
        BaseRoutine soloBehaviorBlock_SelectorContainer = treeBuilder
            .BeginSelector("Solo behavior")
            .BeginCondition("Has Needs", () => { return m_wolf.needs.InNeed(); })
            .AttachTree(needsBlock_SelectorContainer)
            .FinishNode()
            .BeginCondition("On Patrol", () => 
            {
                if (m_onPatrol)
                    return true;
                if (PATROL_CHANCE >= Random.value)
                {
                    currentPatrolPointIDX = 0;
                    return true;
                }
                return false;
            })
            .AttachTree(patrolBehaviorBlock_SequenceContainer)
            .FinishNode()
            .FinishNode();

        //Pack behavior container
        BaseRoutine packBehaviorBlock_SequenceContainer = treeBuilder
            .BeginSequence("move with pack")
            .AddAction("Pack movement", () =>
             {
                // Debug.Log("Called pack movement");
                 return RoutineState.Succeded;
             })
             .FinishNode();

        //return final behavior tree by adding pack and non-pack behaviors
        treeBuilder
            .BeginRepeater("Tree repeater", 0)
            .BeginSelector("Initial State Selector")
            .BeginCondition("Pack Behavior", () =>
            {
                return hasPack;
            })
            .AttachTree(packBehaviorBlock_SequenceContainer) // pack behavior not implemented currently
            .FinishNode()
            .BeginCondition("Non-Pack Behvaior", () =>
            {
                return !hasPack;
            })
            .AttachTree(soloBehaviorBlock_SelectorContainer)
            .FinishNode()
            .FinishNode()
            .FinishNode();

        return treeBuilder;
    }




    // Use this for initialization
    private void Start () {

        m_behaviorTreeTick = new WaitForSeconds(BEHAVIOR_TREE_UPDATE_RATE);
        m_currentTarget = gameObject;
        m_wolf = GetComponent<Wolf>();
        m_detectionModule.Initialize(this);
        m_movementModule.Initialize(this);
        m_behaviorTree = CreateBehaviorTree();
        treeRunner = StartCoroutine(BehaviorTreeRunner());


    }

    private void OnEnable()
    {
        m_detectionModule.Initialize(this);
        m_movementModule.Initialize(this);
        if(treeRunner==null)
            treeRunner = StartCoroutine(BehaviorTreeRunner());
    }

    IEnumerator BehaviorTreeRunner()
    {
        if (m_behaviorTree == null)
            yield break;
        m_behaviorTree.Start();
        while (true)
        {
            m_behaviorTree.Tick();
            yield return m_behaviorTreeTick;
        }
    }

    private void OnDisable()
    {
        m_detectionModule.Shutdown();
        m_movementModule.Shutdown();
        treeRunner = null;
    }

    void OnDrawGizmosSelected()
    {
        m_detectionModule.DrawGizmos();
        m_movementModule.DrawGizmos();
    }
}
