using UnityEngine;
using System.Collections;

public class NonWolf : Animal {

    protected float NormalSpeed = 1f; //running speed
    protected int Bravery = 0;        //sets how many wolves (and taunts by wolves) are needed to make it run away instead of standing its ground
    protected float SpeedDrop = 0.5f; //how much speed it loses when running on the RoughTerrain area

    protected BaseRoutine m_behaviorTree;

    protected void InitValues() {
        //m_gender = (Random.value >= .5) ? AnimalGender.Male : AnimalGender.Female;

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
        
        for (int i = 0; i < (int)NeedType.NEED_COUNT; i++) {
            m_needs.SetNeed((NeedType)i, Random.Range(50, 101));
        }

        m_currentHealth = Random.Range(80, 101);
    }

    protected virtual BaseRoutine CreateBehaviorTree() {
        return m_behaviorTree;
    }
}
