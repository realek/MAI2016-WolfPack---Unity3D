using CustomConsts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Rabbit : NonWolf {

    void Start() {
        m_strength = AnimalStrength.Weak;
        CarcassQnt = GlobalVars.RabbitCarcassQnt;
        InitValues();

        dmg1 = GlobalVars.RabbitAtk1;
        dmg2 = GlobalVars.RabbitAtk2;
        dmg3 = GlobalVars.RabbitAtk3;
        dmg4 = GlobalVars.RabbitAtk4;

        m_needs.Initialize(this, true, 50, 100);

        m_detectionModule.Initialize(this);
        m_movementModule.Initialize(this);
        m_behaviorTree = CreateBehaviorTree();
        treeRunner = StartCoroutine(BehaviorTreeRunner());
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
                            status = "Wandering";
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
                .AddAction("Select herd location", () => {
                    m_wanderPoint.transform.position = GetGroupCenter();
                    m_currentTarget = m_wanderPoint;
                    status = "GoToHerd";
                    return RoutineState.Succeded;
                })
                .AttachTree(moveToTarget_SequenceContainer)
            .FinishNode();

        //non-attacked behavior when in a group
        BaseRoutine groupBehavior_SelectorContainer = treeBuilder
                .BeginSelector("Choice of action")
                    .BeginCondition("Are others far away", () => ((transform.position - GetGroupCenter()).sqrMagnitude > GlobalVars.ElkHerdRadius))
                        .AttachTree(returnToGroup_SequenceContainer)
                    .FinishNode()
                    .BeginCondition("Should I move around", () => true)//(Random.value > 0.5f))
                        .AttachTree(wanderBehavioir_SequenceContainer)
                    .FinishNode()
                .FinishNode();

        //run away from wolves, place condition have enough energy
        BaseRoutine runAway_SequenceContainer = treeBuilder
            .BeginSequence("Run away from wolves")
                //go in the direction opposite of the wolf, same distance to wolf
                .AddAction("Choose run direction", () => {
                    m_wanderPoint.transform.position = new Vector3(2 * transform.position.x - m_currentTarget.transform.position.x,
                        transform.position.y, 2 * transform.position.z - m_currentTarget.transform.position.z);
                    m_currentTarget = m_wanderPoint;
                    status = "RunAwayFromWolf";
                    return RoutineState.Succeded;
                })
                .AttachTree(moveToTarget_SequenceContainer)
            .FinishNode();

        //the executed behavior tree
        BaseRoutine btTree = treeBuilder
            .BeginSelector("What to do")
                .BeginCondition("Are wolves nearby", () => {
                    foreach (Collider t in m_detectionModule.DetectedGameObjects) {
                        if (t.GetComponent<Wolf>()) return true;
                    }
                    return false;
                })
                    .AttachTree(runAway_SequenceContainer)
                .FinishNode()
                .BeginCondition("Am I in a group", () => (currentGroup != null))
                    .AttachTree(groupBehavior_SelectorContainer)
                .FinishNode()
                .BeginCondition("Should I wander", () => true)//(Random.value > 0.3f))
                    .AttachTree(wanderBehavioir_SequenceContainer)
                .FinishNode()
            .FinishNode();

        //the actual tree
        treeBuilder
            .BeginRepeater("Tree repeater", 0)
                .BeginSelector("To do or not to do")
                    .BeginCondition("Do I have to wait", () => (WaitTime > 0.001f))
                            .AddAction("Reduce wait time", () => {
                                status = "Waiting";
                                WaitTime -= BEHAVIOR_TREE_UPDATE_RATE;
                                if (WaitTime <= 0) {
                                    WaitTime = 0;
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


        return treeBuilder;
    }

    private void OnEnable() {
        m_detectionModule.Initialize(this);
        m_movementModule.Initialize(this);
        if (treeRunner == null)
            treeRunner = StartCoroutine(BehaviorTreeRunner());
    }

    private void OnDrawGizmosSelected() {
        m_detectionModule.DrawGizmos();
        m_movementModule.DrawGizmos();
    }

    private void OnDisable() {
        m_detectionModule.Shutdown();
        m_movementModule.Shutdown();
        treeRunner = null;
    }

    private void OnDestroy() {
        Destroy(m_wanderPoint);
    }

}
