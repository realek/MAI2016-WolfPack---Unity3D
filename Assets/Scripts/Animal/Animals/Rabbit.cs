using UnityEngine;

public class Rabbit : NonWolf {

    void Start () {
        InitValues();
        m_strength = AnimalStrength.Weak;
	}

    protected override BaseRoutine CreateBehaviorTree() {
        BehaviorTreeBuilder treeBuilder = new BehaviorTreeBuilder();


        return treeBuilder;
    }
}
