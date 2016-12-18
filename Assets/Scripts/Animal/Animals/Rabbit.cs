using CustomConsts;
using UnityEngine;

public class Rabbit : NonWolf {

    void Start () {
        InitValues();
        m_strength = AnimalStrength.Weak;
        CarcassQnt = GlobalVars.RabbitCarcassQnt;
        treeRunner = StartCoroutine(BehaviorTreeRunner());
    }

    protected override BaseRoutine CreateBehaviorTree() {
        BehaviorTreeBuilder treeBuilder = new BehaviorTreeBuilder();


        return treeBuilder;
    }


    private void OnEnable()
    {
        treeRunner = StartCoroutine(BehaviorTreeRunner());
    }


    private void OnDisable()
    {
        treeRunner = null;
    }

    private void OnDestroy()
    {
        Destroy(m_wanderPoint);
    }


}
