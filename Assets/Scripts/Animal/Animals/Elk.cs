using System.Collections;
using CustomConsts;
using UnityEngine;
using UnityEngine.AI;

public class Elk : NonWolf {
    
    [SerializeField]
    private AIDetectionModule m_detectionModule;
    [SerializeField]
    private AIMovementModule m_movementModule;
    private GameObject m_currentTarget;

    private GameObject m_wanderPoint;
    [SerializeField]
    private bool m_wandering = false;

    private bool closeToOthers;
    private AnimalGroup m_group;
    private int atkDmg;
    private int dmg1 = 0;
    private int dmg2 = 2;
    private int dmg3 = 6;
    private int dmg4 = 4;

    private const float BEHAVIOR_TREE_UPDATE_RATE = 0.2f;
    private WaitForSeconds m_behaviorTreeTick;
    private Coroutine treeRunner;

    void Start () {
        InitValues();
        m_strength = AnimalStrength.Strong;
        CarcassQnt = GlobalVars.ElkCarcassQnt;
        m_wanderPoint = new GameObject("Wander point for " + gameObject.name + " id: " + gameObject.GetInstanceID());
        m_behaviorTreeTick = new WaitForSeconds(BEHAVIOR_TREE_UPDATE_RATE);
        treeRunner = StartCoroutine(BehaviorTreeRunner());
    }

    IEnumerator BehaviorTreeRunner() {
        if (m_behaviorTree == null)
            yield break;
        m_behaviorTree.Start();
        while (true) {
            m_behaviorTree.Tick();
            yield return m_behaviorTreeTick;
        }
    }



    protected override BaseRoutine CreateBehaviorTree() {
        BehaviorTreeBuilder treeBuilder = new BehaviorTreeBuilder();

        //move to target
        BaseRoutine moveToTarget_SequenceContainer = treeBuilder
            .BeginSequence("Move TO")
                .AddAction("Movement", () => {
                    if (m_currentTarget == null || m_currentTarget == gameObject)
                        return RoutineState.Failed;

                    m_movementModule.Move(m_currentTarget);
                    if (m_movementModule.reachedTarget && !m_movementModule.unreachableTarget)
                        return RoutineState.Succeded;
                    if (!m_movementModule.reachedTarget && !m_movementModule.unreachableTarget)
                        return RoutineState.Running;
                    return RoutineState.Failed;
                })
            .FinishNode();

        //wander behavior
        BaseRoutine wanderBehavioir_SequenceContainer = treeBuilder
            .BeginSequence("Wander Sequence")
            .AddAction("Select Random Point On Navmesh", () => {
                if (!m_wandering) {
                    m_wandering = true;
                    NavMeshHit hit;
                    Vector3 source = transform.position + (Random.insideUnitSphere * m_detectionModule.DetectionAreaRadius);
                    if (NavMesh.SamplePosition(source, out hit, m_detectionModule.DetectionAreaRadius, -1)) {
                        m_wanderPoint.transform.position = hit.position;
                        m_currentTarget = m_wanderPoint;
                    } else {
                        Debug.Log("Failed to get random point on navmesh with source at" + source + "hit at: " + hit.position);
                        return RoutineState.Failed;
                    }

                }

                return RoutineState.Succeded;
            })
            .AttachTree(moveToTarget_SequenceContainer)
            .AddAction("EndWander", () => {
                m_currentTarget = null;
                m_wandering = false;
                return RoutineState.Succeded;
            })
            .FinishNode();

        //return to group
        BaseRoutine returnToGroup_SequenceContainer = treeBuilder
            .BeginSequence("Return to group")
                .AddAction("Select elk location", () => {
                    //m_currentTarget = (m_group.GetClosestMember(0) != gameObject)
                    //    ? m_group.GetClosestMember(0).gameObject
                    //    : m_group.GetClosestMember(1).gameObject;
                    return RoutineState.Succeded;
                })
                .AttachTree(moveToTarget_SequenceContainer)
            .FinishNode();

        //non-attacked behavior when in a group
        BaseRoutine groupBehavior_SelectorContainer = treeBuilder
                .BeginSelector("Choice of action")
                    .BeginCondition("Are others far away", () => !closeToOthers)
                        .AttachTree(returnToGroup_SequenceContainer)
                    .FinishNode()
                    .BeginCondition("Should I move around", () => (Random.value > 0.5f))
                        .BeginSequence("Move around")
                            .AddAction("Select random location", () => {
                                //TODO set a random nearby Vector3 as target
                                return RoutineState.Succeded;
                            })
                            .AttachTree(moveToTarget_SequenceContainer)
                        .FinishNode()
                    .FinishNode()
                .FinishNode();
        
        //run away from wolves
        BaseRoutine runAway_SequenceContainer = treeBuilder
            .BeginSequence("Run away from wolves")
                .AddAction("Choose run direction", () => {
                    //TODO set target in the opposite direction of the wolves
                    return RoutineState.Succeded;
                })
                .AttachTree(moveToTarget_SequenceContainer)
            .FinishNode();

        //deal damage
        BaseRoutine attack_SequenceContainer = treeBuilder
            .BeginSequence("Run away from wolves")
                .AddAction("Animate and deal damage", () => {
                    //TODO add animator animation
                    if (m_currentTarget == null || m_currentTarget == gameObject || m_currentTarget.GetComponent<Animal>() == null)
                        return RoutineState.Failed;
                    switch (m_age) {
                            case AnimalAge.YoungAdult:
                            atkDmg = dmg2;
                            break;
                            case AnimalAge.Adult:
                            atkDmg = dmg3;
                            break;
                            case AnimalAge.Elder:
                            atkDmg = dmg4;
                            break;
                        default:
                            atkDmg = dmg1;
                            break;
                    }
                    m_currentTarget.GetComponent<Animal>().DealtDmg(atkDmg + Bravery * 2);
                    return RoutineState.Succeded;
                })
                .AddAction("Cooldown", () => {
                    //TODO: wait animation time to finish
                    float atkLag;
                    if (m_currentHealth > 30) atkLag = 2;
                    else atkLag = 5;
                    //TODO: wait atkLag time
                    return RoutineState.Succeded;
                })
            .FinishNode();

        //fight behavior
        BaseRoutine fight_SequenceContainer = treeBuilder
            //TODO choose lowest health target
            .AttachTree(moveToTarget_SequenceContainer)
            .AttachTree(attack_SequenceContainer)
            .AddAction("Win", () => RoutineState.Succeded);

        //stay behavior
        BaseRoutine stayAndWait_SequenceContainer = treeBuilder
            //TODO: stay and wait
            .AddAction("Wait", () => RoutineState.Succeded);

        //attacked and alone behavior
        BaseRoutine attackedIndividual_SelectorContainer = treeBuilder
            .BeginSelector("How much bravery I have left")
                .BeginCondition("none", () => (Bravery == 0))
                    .BeginSelector("Fight or flight")
                        .BeginCondition("Are the wolves too many", () => {
                            //TODO count attacking wolves
                            //for Elk if (count > 2)
                            return true;
                        })
                            .BeginSelector("Can I run away")
                                .BeginCondition("Do I have energy left", () => {
                                    //TODO if energy > 10
                                    return true;
                                })
                                    .AttachTree(runAway_SequenceContainer)
                                .FinishNode()
                                .AttachTree(fight_SequenceContainer) //if not enough energy to run, fight
                            .FinishNode()
                        .FinishNode()
                        .AttachTree(fight_SequenceContainer) //if wolves are not too many, fight
                    .FinishNode()
                .FinishNode()
                .BeginCondition("Are wolves not attacking me", () => (m_currentHealth > 60))
                    .AttachTree(stayAndWait_SequenceContainer)
                .FinishNode()
                .AttachTree(fight_SequenceContainer) //if wolves are attacking although bravery is not 0, fight (with increased damage)
            .FinishNode();

        //attacked behavior
        BaseRoutine attackedBehavior_SequenceContainer = treeBuilder
            .BeginSelector("Attacked action")
                .BeginCondition("Am I in a group", () => (m_group.GetSize() > 1))
                    .BeginSelector("Stay or flee group")
                        .BeginCondition("Am I unharmed", () => (m_currentHealth > 79))
                            .BeginSelector("Run away with group")
                          //      .BeginCondition("Am I member 0 of my group", () => (m_group.GetClosestMember(0) == gameObject))
                                    .AttachTree(runAway_SequenceContainer)    
                                .FinishNode()
                                .BeginCondition("Have the others ran away", () => !closeToOthers)
                                    .AttachTree(returnToGroup_SequenceContainer)
                                .FinishNode()
                                .AttachTree(stayAndWait_SequenceContainer) //else stay in the same place
                            .FinishNode()
                        .FinishNode()
                        .AttachTree(attackedIndividual_SelectorContainer)
                    .FinishNode()
                .FinishNode()
                .AttachTree(attackedIndividual_SelectorContainer)
            .FinishNode();

        //the behavior tree
        BaseRoutine btTree = treeBuilder
            ;

        //wait or execute tree
        BaseRoutine waitOrDo_SelectorContainer = treeBuilder
            .BeginSelector("To do or not to do")
                .BeginSequence("Waiting")
                    .BeginCondition("Do I have to wait", () => (WaitTime > 0.001f))
                    .AddAction("Reduce wait time", () => {
                        WaitTime -= BEHAVIOR_TREE_UPDATE_RATE;
                        if (WaitTime < 0) WaitTime = 0;
                        return RoutineState.Succeded;
                    })
                    .FinishNode()
                .FinishNode()
                .AttachTree(btTree)
            .FinishNode();

        return treeBuilder;
    }
}
