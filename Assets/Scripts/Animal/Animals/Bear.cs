using CustomConsts;
using UnityEngine;

public class Bear : NonWolf {

    void Start () {
        InitValues();
        m_strength = AnimalStrength.VeryStrong;
        CarcassQnt = GlobalVars.BearCarcassQnt;
    }

    protected override BaseRoutine CreateBehaviorTree() {
        BehaviorTreeBuilder treeBuilder = new BehaviorTreeBuilder();


        return treeBuilder;
    }
}
