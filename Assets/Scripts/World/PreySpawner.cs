using System.Collections.Generic;
using UnityEngine;

public class PreySpawner : Singleton<PreySpawner> {
    
    public List<GameObject> SpawnPoints;

    private List<GameObject> AllNonWolves = new List<GameObject>();
    private int spCounter = 0;

	// Use this for initialization
	void Start () {
        AddHerd(1, 4);
        AddIndividual(1);
	}

    private void SetMaxEnergy(GameObject animal) {
        
    }

    private void NextPoint() {
        spCounter++;
        Debug.Log(spCounter);
        if (spCounter > SpawnPoints.Count - 1) spCounter = 0;
    }

    public void Repopulate() {
        if (AllNonWolves.Count < 10) {
            float dice = Random.value;
            if (dice < .3) AddIndividual(0);
            else if (dice < .5) AddIndividual(1);
            else if (dice < .7) AddIndividual(2);
            else if (dice < .8) AddHerd(0, Random.Range(3, 6));
            else if (dice < .9) AddHerd(1, Random.Range(3, 6));
            else AddHerd(2, Random.Range(3, 6));
            NextPoint();
        }
    }

    private void AddHerd(int type, int size) {
        int count = AllNonWolves.Count;
        NonWolf[] NonWolfArray = new NonWolf[size];
        for (int i = 0; i < size; i++) {
            AllNonWolves.Add(SpawnIndividual(type));
            SetMaxEnergy(AllNonWolves[AllNonWolves.Count - 1]);
            AllNonWolves[AllNonWolves.Count - 1].transform.position = SpawnPoints[spCounter].transform.position + new Vector3(Random.Range(0, 10), 0f, Random.Range(0, 10));
            NonWolfArray[i] = AllNonWolves[i + count].GetComponent<NonWolf>();
        }
        NextPoint();
        AnimalGroupManager.Instance.RegisterHerd(NonWolfArray);
    }

    private void AddIndividual(int type) {
        AllNonWolves.Add(SpawnIndividual(type));
        SetMaxEnergy(AllNonWolves[AllNonWolves.Count - 1]);
        AllNonWolves[AllNonWolves.Count - 1].transform.position = SpawnPoints[spCounter].transform.position + new Vector3(Random.Range(0, 10), 0f, Random.Range(0, 10));
        NextPoint();
    }

    private GameObject SpawnIndividual(int type) {
        switch (type) {
            case 0:
                return (GameObject) Instantiate((Resources.Load("Rabbit")));
            case 1:
                return (GameObject) Instantiate((Resources.Load("ElkTemp")));
            case 2:
                return (GameObject) Instantiate((Resources.Load("Coyote")));
            default:
                return (GameObject) Instantiate((Resources.Load("Bear")));
        }
    }
    
}
