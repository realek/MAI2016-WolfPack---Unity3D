using CustomConsts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Rabbit : NonWolf {

    private int dmg1 = GlobalVars.RabbitAtk1;
    private int dmg2 = GlobalVars.RabbitAtk2;
    private int dmg3 = GlobalVars.RabbitAtk3;
    private int dmg4 = GlobalVars.RabbitAtk4;

    public Text status;

    void Start() {
        m_strength = AnimalStrength.Weak;
        CarcassQnt = GlobalVars.RabbitCarcassQnt;
        InitValues();

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
                            status.text = "Wandering";
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
                    status.text = "GoToHerd";
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
                    status.text = "RunAwayFromWolf";
                    return RoutineState.Succeded;
                })
                .AttachTree(moveToTarget_SequenceContainer)
            .FinishNode();

        //run away from wolves, place condition have enough energy
        BaseRoutine groupRunAway_SequenceContainer = treeBuilder
            .BeginSequence("Run away from wolves")
                //go in the direction opposite of the wolf, twice the distance from the center of the group to wolf
                .AddAction("Choose run direction", () => {
                    Vector3 center = GetGroupCenter();
                    float xPos = 3 * center.x - 2 * m_currentTarget.transform.position.x;
                    float zPos = 3 * center.z - 2 * m_currentTarget.transform.position.z;

                    //if the goal point ends up beyond the field limits, instead of running away from the wolf
                    // charge straight through him
                    if (xPos < 210 || xPos > 710) xPos = 2 * m_currentTarget.transform.position.x - center.x;
                    if (zPos < 120 || zPos > 860) xPos = 2 * m_currentTarget.transform.position.z - center.z;

                    m_wanderPoint.transform.position = new Vector3(xPos, transform.position.y, zPos);
                    m_currentTarget = m_wanderPoint;
                    status.text = "GroupRunAway";
                    return RoutineState.Succeded;
                })
                .AttachTree(moveToTarget_SequenceContainer)
            .FinishNode();

        //deal damage
        BaseRoutine attack_SequenceContainer = treeBuilder
            .BeginSequence("Run away from wolves")
                .AddAction("Animate and deal damage", () => {
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
                    if (m_currentHealth > 30) WaitTime = 1;
                    else WaitTime = 2;
                    status.text = "Attacking";
                    return RoutineState.Succeded;
                })
            .FinishNode();

        //fight behavior
        BaseRoutine fight_SequenceContainer = treeBuilder
            .BeginSequence("Combat")
            .AddAction("Find wolf with lowest hp", () => {
                int weakest = 101;
                foreach (Collider t in m_detectionModule.DetectedGameObjects) {
                    var m_wolf = t.GetComponent<Wolf>();
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

        //stay behavior
        BaseRoutine stayAndWait_SequenceContainer = treeBuilder
            .BeginSequence("Wait timer")
            .AddAction("Wait", () => {
                status.text = "Waiting";
                WaitTime = 1;
                needs.ModNeed(NeedType.Energy, GlobalVars.RestEnIncrease);
                return RoutineState.Succeded;
            })
            .FinishNode();

        //attacked and alone behavior
        BaseRoutine attackedIndividual_SelectorContainer = treeBuilder
            .BeginSelector("How much bravery I have left")
                .BeginCondition("none", () => (Bravery == 0))
                        .BeginCondition("Are the wolves too many", () => {
                            int wolfCount = 0;
                            foreach (Collider t in m_detectionModule.DetectedGameObjects) {
                                if (t.tag == "Wolf") {
                                    wolfCount++;
                                }
                            }
                            return (wolfCount > 0); //how many wolves are too many
                        })
                            .BeginSelector("Can I run away")
                                .BeginCondition("Do I have energy left", () => (needs.GetNeed(NeedType.Energy) > 30))
                                    .AttachTree(runAway_SequenceContainer)
                                .FinishNode()
                                .BeginCondition("Out of energy, fight", () => true)
                                    .AttachTree(fight_SequenceContainer) //if not enough energy to run, fight
                                .FinishNode()
                            .FinishNode()
                        .FinishNode()
                .FinishNode()
                .BeginCondition("Are wolves not attacking me", () => (m_currentHealth > 60))
                    .AttachTree(stayAndWait_SequenceContainer)
                .FinishNode()
                .BeginCondition("Is brave enough", () => Bravery > 0)
                    .AttachTree(fight_SequenceContainer) //if wolves are attacking although bravery is not 0, fight (with increased damage)
                .FinishNode()
            .FinishNode();

        //attacked behavior
        BaseRoutine attackedBehavior_SequenceContainer = treeBuilder
            .BeginSelector("Attacked action")
                .BeginCondition("Am I in a group", () => (m_group != null))
                    .BeginSelector("Stay or flee group")
                        //am I healthy enough to stay in the herd
                        .BeginCondition("Am I unharmed and not tired", () => (m_currentHealth > 79 && needs.GetNeed(NeedType.Energy) > 10))
                            .AttachTree(groupRunAway_SequenceContainer)
                        .FinishNode()
                        .BeginCondition("Unhealthy", () => true)
                            .AttachTree(attackedIndividual_SelectorContainer)
                        .FinishNode()
                    .FinishNode()
                .FinishNode()
                .BeginCondition("Not in group", () => true)
                    .AttachTree(attackedIndividual_SelectorContainer)
                .FinishNode()
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
                    .AttachTree(attackedBehavior_SequenceContainer)
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
                                status.text = "Waiting";
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
