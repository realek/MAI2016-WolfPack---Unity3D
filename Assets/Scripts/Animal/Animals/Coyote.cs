using CustomConsts;
using UnityEngine;

public class Coyote : NonWolf {

    void Start () {
        InitValues();
        m_strength = AnimalStrength.Medium;
        CarcassQnt = GlobalVars.CoyoteCarcassQnt;
    }

    protected override BaseRoutine CreateBehaviorTree() {
        BehaviorTreeBuilder treeBuilder = new BehaviorTreeBuilder();


        return treeBuilder;
    }
}
