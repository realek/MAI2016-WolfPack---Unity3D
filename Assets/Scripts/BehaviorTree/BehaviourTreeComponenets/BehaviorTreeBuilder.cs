using UnityEngine;
using System.Collections;

/// <summary>
/// Fluent builder to create behavoir trees
/// </summary>
public class BehaviorTreeBuilder {

    private Selector RootNode;
    private Sequence EnergyRegainNode;

    private BaseRoutine GoToRestArea;
    private BaseRoutine EnterSleepState;

    private BaseRoutine EnterThirstState;
    private BaseRoutine EnterHungerState;
    private BaseRoutine EnterPlayfulState;
    
    private Needs currentNeeds;

    private void Start() {
        //set children to to nodes
    }
    
    public GameManager.WolfState ExecuteTree(Needs wolfNeeds) {
        currentNeeds = wolfNeeds;

        return GameManager.WolfState.Idle;
    }

}
