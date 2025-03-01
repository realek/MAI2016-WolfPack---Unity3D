﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WolfPackManager : Singleton<WolfPackManager> {

    public Wolf initMWolf;
    public Wolf initFWolf;
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
        targetDict.Add(initMWolf, initFWolf.gameObject);
        targetDict.Add(initFWolf, null);
    }

    public void RegisterPack() {
        if (!registered) {
            AnimalGroupManager.Instance.RegisterWolfPack(initMWolf, initFWolf);
            targetDict[initMWolf] = restArea;
            targetDict[initFWolf] = restArea;

            registered = true;
        }
    }

    public void SpawnWolves(int numb) {
        GameObject wolf = (GameObject)Resources.Load("Wolf");
        for (int i = 0; i < numb; i++) {
            GameObject pup = Instantiate(wolf);
            NavMeshHit hit;
            NavMesh.SamplePosition(restArea.transform.position +
                                     new Vector3(Random.Range(1, 3), 0f, Random.Range(0, 10)), out hit, 3, -1);

            // pup.transform.position = hit.position;
            pup.GetComponent<NavMeshAgent>().Warp(hit.position);
            pup.GetComponent<Wolf>().SetGroup(initMWolf.GetComponent<Wolf>().currentGroup);
        }
    }
}
