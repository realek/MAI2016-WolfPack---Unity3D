using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreySpawner : MonoBehaviour {

    private List<GameObject> AllNonWolves = new List<GameObject>();

	// Use this for initialization
	void Start () {
	    AddHerd(1, 4);

	    foreach (GameObject t in AllNonWolves) {
	        t.transform.position = new Vector3(500 + Random.Range(0, 50), 50, 500 + Random.Range(0, 50));
	    }
	}

    private void AddHerd(int type, int size) {
        int count = AllNonWolves.Count;
        NonWolf[] NonWolfArray = new NonWolf[size];
        for (int i = 0; i < size; i++) {
            AllNonWolves.Add(SpawnIndividual(type));
            NonWolfArray[i] = AllNonWolves[i + count].GetComponent<NonWolf>();
        }
        AnimalGroupManager.Instance.RegisterHerd(NonWolfArray);
    }

    private void AddIndividual(int type) {
        AllNonWolves.Add(SpawnIndividual(type));
    }

    private GameObject SpawnIndividual(int type) {
        switch (type) {
            case 0:
                return (GameObject) Instantiate((Resources.Load("Rabbit")));
            case 1:
                return (GameObject) Instantiate((Resources.Load("Elk")));
            case 2:
                return (GameObject) Instantiate((Resources.Load("Coyote")));
            default:
                return (GameObject) Instantiate((Resources.Load("Bear")));
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
