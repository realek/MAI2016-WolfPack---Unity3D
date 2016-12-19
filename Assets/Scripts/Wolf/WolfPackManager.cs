using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfPackManager : Singleton<WolfPackManager> {

    public GameObject initMWolf;
    public GameObject initFWolf;
    public GameObject restArea;

    public GameObject[] patrolPoints;

    [HideInInspector]
    public List<GameObject> packList;
    [HideInInspector]
    public Dictionary<Wolf, GameObject> targetDict = new Dictionary<Wolf, GameObject>(); //wolf->target
    [HideInInspector]
    public int mateStage = 0;

    private bool registered = false;

    // Use this for initialization
    void Awake () {
        targetDict.Add(initMWolf.GetComponent<Wolf>(), initFWolf);
        targetDict.Add(initFWolf.GetComponent<Wolf>(), null);
    }

    public void RegisterPack() {
        if (!registered) {
            AnimalGroupManager.Instance.RegisterWolfPack(initMWolf.GetComponent<Wolf>(), initFWolf.GetComponent<Wolf>());
            targetDict[initMWolf.GetComponent<Wolf>()] = restArea;
            targetDict[initFWolf.GetComponent<Wolf>()] = restArea;
            registered = true;
        }
    }

    public void SpawnWolves(int numb) {
        for (int i = 0; i < numb; i++) {
            GameObject pup = (GameObject) Instantiate((Resources.Load("Wolf")));
            pup.transform.position = restArea.transform.position +
                                     new Vector3(Random.Range(0, 10), 0f, Random.Range(0, 10));
            pup.GetComponent<Wolf>().SetGroup(initMWolf.GetComponent<Wolf>().currentGroup);
        }
    }
}
