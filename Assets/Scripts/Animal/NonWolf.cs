using UnityEngine;

public class NonWolf : Animal {

    public int Size = 1; //animal size: 1 rabbit, 2 coyote, 3 elk, 4 bear
    public float NormalSpeed = 1f; //running speed
    public int Bravery = 0; //sets how many wolves (and taunts by wolves) are needed to make it run away instead of standing its ground
    public float SpeedDrop = 0.5f; //how much speed it loses when running on the RoughTerrain area
    public int NearbyFriendly;

    private float fatigue;
    private bool wolvesAround;
    
	void Start () {
	    fatigue = 0f;
	}

    //set a smaller behavior tree for it
}
