using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using CustomConsts;
using UnityEngine.UI;

[RequireComponent(typeof(Wolf))]
public class WolfAIController : MonoBehaviour {


    [SerializeField]
    private Text status;
    [SerializeField]
    private AIDetectionModule m_detectionModule;
    [SerializeField]
    private AIMovementModule m_movementModule;
    public bool hasPack;
    private BaseRoutine m_behaviorTree;
    private Wolf m_wolf;
    public GameObject[] patrolPoints;
    private int currentPatrolPointIDX = -1;
    private GameObject m_currentTarget;
    private GameObject m_wanderPoint;
    [SerializeField]
    private bool m_onPatrol = false;
    [SerializeField]
    private bool m_wandering = false;
    private const float WANDER_CHANCE = 0.5f;
    private const float PATROL_CHANCE = 0.1f;
    private const float BEHAVIOR_TREE_UPDATE_RATE = 0.2f; // 5 times a second is enough
    private WaitForSeconds m_behaviorTreeTick;
    private Coroutine treeRunner;

    private GameObject sleepArea;

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
                    m_currentTarget = sleepArea;
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
                if (m_currentTarget.tag == "Food") m_currentTarget.GetComponent<Perishable>().Reduce(GlobalVars.WolfEatQnt);
                m_wolf.needs.ModNeed(NeedType.Hunger, GlobalVars.WolfEatQnt);
                return RoutineState.Succeded;

            })
            .FinishNode()
            .FinishNode()
            .BeginCondition("Water", () =>
            {
                if (m_wolf.needs.IsNeedTriggered(NeedType.Thirst))
                {
                    m_currentTarget = sleepArea;
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
                if (m_currentTarget.tag == "Water") m_currentTarget.GetComponent<Perishable>().Reduce(GlobalVars.WolfDrinkQnt);
                m_wolf.needs.ModNeed(NeedType.Thirst, GlobalVars.WolfDrinkQnt);
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
            .AddAction("Next Waypoint", () =>
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

        //wander behavior
        BaseRoutine wanderBehavioir_SequenceContainer = treeBuilder
            .BeginSequence("Wander Sequence")
            .AddAction("Select Random Point On Navmesh", () =>
            {
                if (!m_wandering)
                {
                    m_wandering = true;
                    NavMeshHit hit;
                    Vector3 source = transform.position + (Random.insideUnitSphere * m_detectionModule.DetectionAreaRadius);
                    if (NavMesh.SamplePosition(source, out hit, m_detectionModule.DetectionAreaRadius, -1))
                    {
                        m_wanderPoint.transform.position = hit.position;
                        m_currentTarget = m_wanderPoint;
                    }
                    else
                    {
                        Debug.Log("Failed to get random point on navmesh with source at"+source+"hit at: "+hit.position);
                        return RoutineState.Failed;
                    }

                }

                return RoutineState.Succeded;
            })
            .AttachTree(moveToTarget_SequenceContainer)
            .AddAction("EndWander", () =>
            {
                m_currentTarget = null;
                m_wandering = false;
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
                if (m_wandering)
                    return false;
                if (m_onPatrol)
                    return true;
                if (PATROL_CHANCE >= Random.value)
                {
                    currentPatrolPointIDX = 0;
                    m_onPatrol = true;
                    return true;
                }
                return false;
            })
            .AttachTree(patrolBehaviorBlock_SequenceContainer)
            .FinishNode()
            .BeginCondition("Start Wander", () => 
            {
                if (m_wandering)
                    return true;
                float rnd = Random.value;
                if (rnd <= WANDER_CHANCE+PATROL_CHANCE && rnd > PATROL_CHANCE)
                    return true;
                return false;
            })
            .AttachTree(wanderBehavioir_SequenceContainer)
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


        //***************************************************************
        //ATTACK STUFF

        //deal damage
        BaseRoutine attack_SequenceContainer = treeBuilder
            .BeginSequence("Attack Prey")
                .AddAction("Animate and deal damage", () => {
                    if (m_currentTarget == null || m_currentTarget == gameObject || m_currentTarget.GetComponent<Animal>() == null)
                        return RoutineState.Failed;
                    switch (m_wolf.age) {
                        case AnimalAge.YoungAdult:
                            m_wolf.atkDmg = m_wolf.dmg2;
                            break;
                        case AnimalAge.Adult:
                            m_wolf.atkDmg = m_wolf.dmg3;
                            break;
                        case AnimalAge.Elder:
                            m_wolf.atkDmg = m_wolf.dmg4;
                            break;
                        default:
                            m_wolf.atkDmg = m_wolf.dmg1;
                            break;
                    }
                    m_currentTarget.GetComponent<Animal>().DealtDmg(m_wolf.atkDmg);
                    return RoutineState.Succeded;
                })
                .AddAction("Cooldown", () => {
                    if (m_wolf.m_currentHealth > 30) m_wolf.WaitTime = 1;
                    else m_wolf.WaitTime = 2;
                    status.text = "Attacking";
                    return RoutineState.Succeded;
                })
            .FinishNode();

        //fight behavior
        BaseRoutine fight_SequenceContainer = treeBuilder
            .BeginSequence("Combat")
            .AddAction("Find prey with lowest hp", () => {
                int weakest = 101;
                foreach (Collider t in m_detectionModule.DetectedGameObjects) {
                    var m_wolf = t.GetComponent<NonWolf>();
                    if (m_wolf && m_wolf.m_currentHealth < weakest) {
                        weakest = m_wolf.m_currentHealth;
                        m_currentTarget = m_wolf.gameObject;
                    }
                }
                status.text = "ChoosingAtkTarget";
                return RoutineState.Succeded;
            })
            .AttachTree(moveToTarget_SequenceContainer)
            .AttachTree(attack_SequenceContainer)
            .AddAction("Win", () => RoutineState.Succeded)
            .FinishNode();
        //************************************************************************************************************************

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

        m_wanderPoint = new GameObject("Wander point for "+gameObject.name+" id: "+gameObject.GetInstanceID());
        m_behaviorTreeTick = new WaitForSeconds(BEHAVIOR_TREE_UPDATE_RATE);
        m_currentTarget = gameObject;
        m_wolf = GetComponent<Wolf>();
        m_detectionModule.Initialize(this);
        m_movementModule.Initialize(this);
        m_behaviorTree = CreateBehaviorTree();
        treeRunner = StartCoroutine(BehaviorTreeRunner());

        sleepArea = GameObject.FindGameObjectWithTag("RestArea");
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

    private void OnDestroy()
    {
        Destroy(m_wanderPoint);
    }

    void OnDrawGizmosSelected()
    {
        m_detectionModule.DrawGizmos();
        m_movementModule.DrawGizmos();
    }
}
