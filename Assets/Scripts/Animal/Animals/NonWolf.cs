using UnityEngine;
using System.Collections;

public class NonWolf : Animal {

    protected int Bravery = 0;        //sets how many wolves (and taunts by wolves) are needed to make it run away instead of standing its ground
    //protected float NormalSpeed = 1f; //running speed
    //protected float SpeedDrop = 0.5f; //how much speed it loses when running on the RoughTerrain area

    
    [SerializeField]
    protected AIDetectionModule m_detectionModule;
    [SerializeField]
    protected AIMovementModule m_movementModule;
    [SerializeField]
    protected bool m_wandering = false;

    protected GameObject m_wanderPoint;
    protected GameObject m_currentTarget;
    protected AnimalGroup m_group;
    protected int atkDmg;

    protected BaseRoutine m_behaviorTree;
    protected Coroutine treeRunner;
    protected const float BEHAVIOR_TREE_UPDATE_RATE = 0.2f;
    protected WaitForSeconds m_behaviorTreeTick;

    protected void InitValues() {
        switch ((int)(Random.value * 100) % 4) {
            case 0:
                m_age = AnimalAge.Infant;
                break;
            case 1:
                m_age = AnimalAge.YoungAdult;
                break;
            case 2:
                m_age = AnimalAge.Adult;
                break;
            case 3:
                m_age = AnimalAge.Elder;
                break;
            default:
                Debug.Log("Impossible NonWolf age");
                break;
        }

        m_currentHealth = Random.Range(80, m_maxHealth);
        m_wanderPoint = new GameObject("Wander point for " + gameObject.name + " id: " + gameObject.GetInstanceID());
        m_behaviorTreeTick = new WaitForSeconds(BEHAVIOR_TREE_UPDATE_RATE);
    }

    protected IEnumerator BehaviorTreeRunner() {
        if (m_behaviorTree == null)
            yield break;
        m_behaviorTree.Start();
        while (true) {
            m_behaviorTree.Tick();
            yield return m_behaviorTreeTick;
        }
    }




    protected virtual BaseRoutine CreateBehaviorTree() {
        return m_behaviorTree;
    }
}
