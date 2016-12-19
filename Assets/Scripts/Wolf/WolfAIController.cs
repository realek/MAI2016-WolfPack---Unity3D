using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using CustomConsts;
using System.Linq;
using UnityEngine.UI;

[RequireComponent(typeof(Wolf))]
public class WolfAIController : MonoBehaviour {


    private string m_status;
    public string status
    {
        get
        {
            return m_status;
        }
    }
    [SerializeField]
    private AIDetectionModule m_detectionModule;
    [SerializeField]
    private AIMovementModule m_movementModule;
    public bool hasPack;
    private BaseRoutine m_behaviorTree;
    private Wolf m_wolf;
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
    private GameObject m_roughTerrain;
    BaseRoutine CreateBehaviorTree()
    {

        BehaviorTreeBuilder treeBuilder = new BehaviorTreeBuilder();

#region Movement
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
        #endregion

        ///Move with pack
        BaseRoutine followPackLeader_SequenceContainer = treeBuilder
            .BeginSequence("Follow pack")
            .AddAction("set current Target", () =>
             {
                 if (m_wolf.packRole == WolfPackRole.Alpha)
                     return RoutineState.Failed;
                 else
                     m_currentTarget = m_wolf.currentGroup.GetMemberBeforeMe(m_wolf).gameObject;
                 return RoutineState.Succeded;
             })
             .AttachTree(moveToTarget_SequenceContainer)
            .FinishNode();
#region basicNeeds
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
        #endregion

        #region MarkTerritory
        //MarkTerritory behavior
        BaseRoutine markTerritoryBehaviorBlockSequenceContainer = treeBuilder
            .BeginSequence("MarkTerritory Sequence")
            .AddAction("Select Waypoint", () =>
            {
                if (currentPatrolPointIDX == -1)
                    return RoutineState.Failed;
                m_currentTarget = WolfPackManager.Instance.patrolPoints[currentPatrolPointIDX];
                return RoutineState.Succeded;
            })
            .AttachTree(moveToTarget_SequenceContainer)
            .AddAction("Mark Waypoint", () => {
                WolfPackManager.Instance.patrolPoints[currentPatrolPointIDX].GetComponent<ParticleSystem>().Play();
                return RoutineState.Succeded;
            })
            .AddAction("Next Waypoint", () =>
             {
                 currentPatrolPointIDX++;
                 if (currentPatrolPointIDX == WolfPackManager.Instance.patrolPoints.Length)
                 {
                     currentPatrolPointIDX = -1;
                     m_onPatrol = false;
                 }
                 return RoutineState.Succeded;
             })
            .FinishNode();
        #endregion

#region Wander
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
        #endregion

        #region CreatePups

        BaseRoutine createPups_SequenceContainer = treeBuilder
            .BeginSequence("Make pups")
                .AddAction("Wait for pregnancy period", () => {
                    m_wolf.WaitTime = 10f;
                    return RoutineState.Succeded;
                })
                .AddAction("Spawn pups", () => {
                    WolfPackManager.Instance.SpawnWolves(Random.Range(3, 6));
                    return RoutineState.Succeded;
                })
            .FinishNode();
        #endregion

        #region Pregnancy

        BaseRoutine pregnancy_SequenceContainer = treeBuilder
            .BeginSelector("ToDo before children are born")
                .BeginCondition("Am I male", () => {
                    if (m_wolf.gender == AnimalGender.Male) {
                        return true;
                    }
                    return false;
                })
                    .AttachTree(markTerritoryBehaviorBlockSequenceContainer)
                .FinishNode()
                .BeginCondition("Are there enough wolves", () => {
                    if (m_wolf.currentGroup.GetSize() > 4) return false;
                    return true;
                })
                    .AttachTree(createPups_SequenceContainer)
                .FinishNode()
            .FinishNode();

        #endregion

        #region Mate
        BaseRoutine mate_SequenceContainer = treeBuilder
            .BeginSelector("Check")
                .BeginCondition("Is male with female alpha", () => {
                    if (m_wolf.packRole == WolfPackRole.Alpha) {
                        bool withMate = false;
                        foreach (var obj in m_detectionModule.DetectedGameObjects) {
                            if (!withMate && obj.gameObject.tag == "Wolf" &&
                                obj.GetComponent<Wolf>().packRole == WolfPackRole.Alpha) {
                                withMate = true;
                            }
                        }
                        if (withMate) {
                            return true;
                        }
                    }
                    return false;
                })
                        .AddAction("Copulate", () => {
                            m_status = "Multiplying";
                            m_wolf.WaitTime = 2f;
                            currentPatrolPointIDX = 0;
                            return RoutineState.Succeded;
                        })
                .FinishNode()
            .FinishNode();
        #endregion

        #region FindMate
        //before that check if wolf is not in a pack
        BaseRoutine findMate_SequenceContainer = treeBuilder
            .BeginSelector("CreateWolfpack")
                .BeginCondition("Is it stage 0", () => WolfPackManager.Instance.mateStage == 0)
                    .BeginSelector("What is my gender")
                        .BeginCondition("Am I male", () => {
                            if (m_wolf.gender == AnimalGender.Male) {
                                m_currentTarget = WolfPackManager.Instance.targetDict[m_wolf];
                                m_status = "Finding mate";
                                return true;
                            }
                            return false;
                        })
                            .BeginSequence("Find female")
                                .AttachTree(moveToTarget_SequenceContainer)
                                .AddAction("increment stage", () => {
                                    WolfPackManager.Instance.mateStage++;
                                    return RoutineState.Succeded;
                                })
                            .FinishNode()
                        .FinishNode()
                        .BeginCondition("Am I female", () => true)
                            .AttachTree(wanderBehavioir_SequenceContainer)
                        .FinishNode()
                    .FinishNode()
                .FinishNode()
                .BeginCondition("Is it stage 1", () => WolfPackManager.Instance.mateStage == 1)
                    .AddAction("Register Pack", () => {
                        WolfPackManager.Instance.RegisterPack();
                        WolfPackManager.Instance.mateStage++;
                        return RoutineState.Succeded;
                    })
                .FinishNode()
                .BeginCondition("Is it stage 2", () => {
                    if (WolfPackManager.Instance.mateStage == 2) {
                        return true;
                    }
                    return false;
                })
                    .BeginSequence("Find cave")
                        .AddAction("", () => {
                            m_currentTarget = WolfPackManager.Instance.targetDict[m_wolf];
                            return RoutineState.Succeded;
                        })
                        .AttachTree(moveToTarget_SequenceContainer)
                        .AddAction("increment stage", () => {
                            WolfPackManager.Instance.mateStage++;
                            return RoutineState.Succeded;
                        })
                    .FinishNode()
                .FinishNode()
                .BeginCondition("Is it stage 3", () => {
                    if (WolfPackManager.Instance.mateStage == 3) {
                        return true;
                        }
                        return false;
                })
                    .BeginSequence("Mark territory while children are born")
                        .AttachTree(mate_SequenceContainer)
                        .AddAction("increment stage", () => {
                            WolfPackManager.Instance.mateStage++;
                            return RoutineState.Succeded;
                        })
                    .FinishNode()
                .FinishNode()
                .BeginCondition("Is it stage 4", () => {
                    if (WolfPackManager.Instance.mateStage == 4) {
                        return true;
                    }
                    return false;
                })
                    .AttachTree(pregnancy_SequenceContainer)
                .FinishNode()
            .FinishNode();
        #endregion

        #region Chase
        //Get target and move to behavior
        BaseRoutine chaseBehaviorBlock_SelectorContainer = treeBuilder
            .BeginSequence("Chase target")
            .AddAction("Is there a target target that we can chase", () => 
            {
                var result = m_detectionModule.DetectedGameObjects.Where(entity => entity.tag == "Rabbit" || entity.tag == "Elk" || entity.tag == "Prey").ToList();
                if (result.Count == 0) return RoutineState.Failed;
                else
                {
                    m_currentTarget = (result.OrderBy(entity => (entity.transform.position - transform.position).sqrMagnitude).ToList()[0]).gameObject;
                    return RoutineState.Succeded;
                }

            })
            .AttachTree(moveToTarget_SequenceContainer)
            .FinishNode();
        #endregion

        #region Solo
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
            .AttachTree(markTerritoryBehaviorBlockSequenceContainer)
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
        #endregion

#region PackBehavior
        //Pack behavior container
        BaseRoutine packBehaviorBlock_SequenceContainer = treeBuilder
            .BeginSequence("move with pack")
            .AddAction("Pack movement", () =>
             {
                // Debug.Log("Called pack movement");
                 return RoutineState.Succeded;
             })
             .FinishNode();
        #endregion

#region Attack
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
                    m_status = "Attacking";
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
                m_status = "ChoosingAtkTarget";
                return RoutineState.Succeded;
            })
            .AttachTree(moveToTarget_SequenceContainer)
            .AttachTree(attack_SequenceContainer)
            .AddAction("Win", () => RoutineState.Succeded)
            .FinishNode();
        //************************************************************************************************************************

        #endregion

        #region Chase and Attack
        BaseRoutine chaseAttackBehaviorBlock_SequenceContainer = treeBuilder
            .BeginSequence("Chase and attack")
            .AttachTree(chaseBehaviorBlock_SelectorContainer)
            .AttachTree(fight_SequenceContainer)
            .FinishNode();
        #endregion

        #region Chase Towards Zone


        BaseRoutine chaseTowardsBehaviorBlock_sequenceContainer = treeBuilder
         .BeginSequence("Chase target")
         .AddAction("Is there a target target that we can chase", () =>
         {
             var result = m_detectionModule.DetectedGameObjects.Where(entity => entity.tag == "Rabbit" || entity.tag == "Elk" || entity.tag == "Prey").ToList();
             if (result.Count == 0) return RoutineState.Failed;
             else
             {
                 m_currentTarget = (result.OrderBy(entity => (entity.transform.position - transform.position).sqrMagnitude).ToList()[0]).gameObject;
                 return RoutineState.Succeded;
             }

         })
         .AddAction("Compute chase direction based on rough area", () => 
         {
             Vector3 desiredDirection = m_roughTerrain.transform.position - m_currentTarget.transform.position;
             if (desiredDirection == Vector3.zero)
                 return RoutineState.Succeded;
             Vector3 goBehindTarget = m_currentTarget.transform.position - desiredDirection;
             m_wanderPoint.transform.position = goBehindTarget;
             m_currentTarget = m_wanderPoint;
             return RoutineState.Succeded;
         })
         .AttachTree(moveToTarget_SequenceContainer)
         .FinishNode();


        #endregion

        #region Chase Towards Zone and Attack
        BaseRoutine chaseTowardsZoneAttackBehaviorBlock_SequenceContainer = treeBuilder
           .BeginSequence("Chase and attack")
           .AttachTree(chaseTowardsBehaviorBlock_sequenceContainer)
           .AttachTree(fight_SequenceContainer)
           .FinishNode();
        #endregion

        #region MainTree
        //return final behavior tree by adding pack and non-pack behaviors

        BaseRoutine btTree = treeBuilder
            .BeginRepeater("Repeater", 0)
                .BeginSelector("A")
                    .BeginCondition("Am I mating", () => {
                        if (m_wolf.packRole == WolfPackRole.Alpha && WolfPackManager.Instance.mateStage < 5) {
                            return true;
                        }
                        return false;
                    })
                        .AttachTree(findMate_SequenceContainer)
                    .FinishNode()
                .FinishNode()
            .FinishNode()
            //.AttachTree(chaseAttackBehaviorBlock_SequenceContainer)
            //.BeginSelector("A")
            //    .BeginCondition("B", () =>
            //    {
            //        m_currentTarget = sleepArea;
            //        return true;
            //    })
            //    .AttachTree(moveToTarget_SequenceContainer)
            //    //.AttachTree(findMate_SequenceContainer)
            //    .FinishNode()
            //.FinishNode()
            //.FinishNode();
            /*
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
            .FinishNode()
            */
            ;
        #endregion

        #region WaitTree

        //the actual tree
        treeBuilder
            .BeginRepeater("Tree repeater", 0)
                .BeginSelector("To do or not to do")
                    .BeginCondition("Do I have to wait", () => (m_wolf.WaitTime > 0.001f))
                            .AddAction("Reduce wait time", () => {
                                m_status = "Waiting";
                                m_wolf.WaitTime -= BEHAVIOR_TREE_UPDATE_RATE;
                                if (m_wolf.WaitTime <= 0) {
                                    m_wolf.WaitTime = 0;
                                    return RoutineState.Failed;
                                }
                                return RoutineState.Succeded;
                            })
                    .FinishNode()
                    .BeginCondition("Dont wait", () => true)
                        .AttachTree(btTree)
                    .FinishNode()
                .FinishNode()
            .FinishNode();

        #endregion

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
        m_roughTerrain = GameObject.FindGameObjectWithTag("RoughTerrain");
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
