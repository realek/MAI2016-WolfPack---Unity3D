using UnityEngine;
using System.Collections.Generic;

public class Elk : Animal {

    public int Size = 1; //animal size: 1 rabbit, 2 coyote, 3 elk, 4 bear
    public float NormalSpeed = 1f; //running speed
    public int Bravery = 0; //sets how many wolves (and taunts by wolves) are needed to make it run away instead of standing its ground
    public float SpeedDrop = 0.5f; //how much speed it loses when running on the RoughTerrain area
    private List<Elk> m_herd;
    public bool inHerd
    {
        get
        {
            if (m_herd != null)
                return m_herd.Count > 0;
            else
                return false;
        }

    }
    private bool wolvesAround;
    
	void Start () {

        for (int i = 0; i < (int)NeedType.NEED_COUNT; i++)
        {
            m_needs.SetNeed((NeedType)i, Random.Range(50, 101));
        }
	}

    //set a smaller behavior tree for it
}
