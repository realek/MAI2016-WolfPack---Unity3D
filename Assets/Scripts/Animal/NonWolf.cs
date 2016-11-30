using UnityEngine;
using System.Collections;

public class NonWolf : Animal {

    public int size = 1; //animal size: 1 rabbit, 2 coyote, 3 elk
    public float normalSpeed = 1f; //running speed
    public int bravery = 0; //sets how many wolves (and taunts by wolves) are needed to make it run away instead of standing its ground
    public float speedDrop = 0.5f; //how much speed it loses when running on the RoughTerrain area

    private float fatigue;
    private bool wolvesAround;
    
	void Start () {
	    fatigue = 0f;
	}
	
	void Update () {
	    //set a smaller behavior tree for it
	}
}
