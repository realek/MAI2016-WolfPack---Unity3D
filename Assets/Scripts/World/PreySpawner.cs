using System.Collections.Generic;
using UnityEngine;

public class PreySpawner : Singleton<PreySpawner> {

    private List<GameObject> AllNonWolves = new List<GameObject>();

	// Use this for initialization
	void Start () {
	    AddHerd(1, 4);
	    AddIndividual(1);
        //###TODO add more animals

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

    public void Repopulate() {
        if (AllNonWolves.Count < 10) {
            bool bearExistsFlag = false;
            for (int i = 0; i < AllNonWolves.Count; i++) {
                if (AllNonWolves[i].GetComponent<Bear>()) {
                    bearExistsFlag = true;
                }
            }
            if (!bearExistsFlag) {
                //AddIndividual(3); //TODO add bear model
            }

            float dice = Random.value;
            if (dice < .3) AddIndividual(0);
            else if (dice < .5) AddIndividual(1);
            else if (dice < .7) AddIndividual(2);
            else if (dice < .8) AddHerd(0, Random.Range(3, 6));
            else if (dice < .9) AddHerd(1, Random.Range(3, 6));
            else AddHerd(2, Random.Range(3, 6));
        }
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
    
}
