using UnityEngine;

public class Elk : NonWolf {
    
    [SerializeField]
    private AIDetectionModule m_detectionModule;
    [SerializeField]
    private AIMovementModule m_movementModule;
    private GameObject m_currentTarget;

    private bool closeToOthers;
    private AnimalGroup m_group;

    void Start () {
        InitValues();
        m_strength = AnimalStrength.Strong;
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

        //return to group
        BaseRoutine returnToGroup_SequenceContainer = treeBuilder
            .BeginSequence("Return to group")
                .AddAction("Select elk location", () => {
                    m_currentTarget = (m_group.GetMember(0) != gameObject)
                        ? m_group.GetMember(0).gameObject
                        : m_group.GetMember(1).gameObject;
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
                                //set a random nearby Vector3 as target
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
                    //set target in the opposite direction of the wolves
                    return RoutineState.Succeded;
                })
                .AttachTree(moveToTarget_SequenceContainer)
            .FinishNode();

        //fight behavior
        BaseRoutine fight_SequenceContainer = treeBuilder
            //damage dependent on age & increased if bravery not 0,
            //frequency of attacks dependent on health, choice of target dependent on wolf health
            .AddAction("Win", () => RoutineState.Succeded);

        //stay behavior
        BaseRoutine stayAndWait_SequenceContainer = treeBuilder
            //stay and wait
            .AddAction("Wait", () => RoutineState.Succeded);

        //attacked and alone behavior
        BaseRoutine attackedIndividual_SelectorContainer = treeBuilder
            .BeginSelector("How much bravery I have left")
                .BeginCondition("none", () => (Bravery == 0))
                    .BeginSelector("Fight or flight")
                        .BeginCondition("Are the wolves too many", () => {
                            //count attacking wolves
                            //for Elk if (count > 2)
                            return true;
                        })
                            .BeginSelector("Can I run away")
                                .BeginCondition("Do I have energy left", () => {
                                    //if energy > 10
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
                                .BeginCondition("Am I member 0 of my group", () => (m_group.GetMember(0) == gameObject))
                                    .AttachTree(runAway_SequenceContainer)    
                                .FinishNode()
                                .BeginCondition("Have the others ran away", () => !closeToOthers)
                                    .AttachTree(returnToGroup_SequenceContainer)
                                .FinishNode()
                                //else stay in the same place
                            .FinishNode()
                        .FinishNode()
                        .AttachTree(attackedIndividual_SelectorContainer)
                    .FinishNode()
                .FinishNode()
                .AttachTree(attackedIndividual_SelectorContainer)
            .FinishNode();

        return treeBuilder;
    }
}
