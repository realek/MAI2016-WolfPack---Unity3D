﻿using UnityEngine;
using System.Collections;

public class NonWolf : Animal {

    protected int Bravery = 0;        //sets how many wolves (and taunts by wolves) are needed to make it run away instead of standing its ground
    
    [SerializeField]
    protected AIDetectionModule m_detectionModule;
    [SerializeField]
    protected AIMovementModule m_movementModule;
    [SerializeField]
    protected bool m_wandering = false;

    protected GameObject m_wanderPoint;
    protected GameObject m_currentTarget;
    protected AnimalGroup m_group;

    protected BaseRoutine m_behaviorTree;
    protected Coroutine treeRunner;
    protected const float BEHAVIOR_TREE_UPDATE_RATE = 0.2f;
    protected WaitForSeconds m_behaviorTreeTick;

    protected void InitValues() {

        InitAge(Random.Range(0,4));
        m_currentHealth = Random.Range(70, m_maxHealth + 1);
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
