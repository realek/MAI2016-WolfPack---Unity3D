using UnityEngine;

public class Elk : Animal {

    public int Size = 1; //animal size: 1 rabbit, 2 coyote, 3 elk, 4 bear
    public float NormalSpeed = 1f; //running speed
    public int Bravery = 0; //sets how many wolves (and taunts by wolves) are needed to make it run away instead of standing its ground
    public float SpeedDrop = 0.5f; //how much speed it loses when running on the RoughTerrain area
    private bool wolvesAround;
    private BaseRoutine m_behaviorTree;

    void Start () {
        m_strength = AnimalStrength.Strong;

        m_gender = (Random.value >= .5) ? AnimalGender.Male : AnimalGender.Female;

        switch ((int) (Random.value * 100) % 4)
	    {
            case 0: m_age = AnimalAge.Infant;
	            break;
            case 1: m_age = AnimalAge.YoungAdult;
	            break;
            case 2: m_age = AnimalAge.Adult;
	            break;
            case 3: m_age = AnimalAge.Elder;
	            break;
            default: Debug.Log("Impossible Elk age");
	            break;
        }

        for (int i = 0; i < (int)NeedType.NEED_COUNT; i++)
        {
            m_needs.SetNeed((NeedType)i, Random.Range(50, 101));
        }

	    m_currentHealth = Random.Range(80, 101);
	}

    BaseRoutine CreateBehaviorTree() {
        BehaviorTreeBuilder treeBuilder = new BehaviorTreeBuilder();


        return treeBuilder;
    }
}
