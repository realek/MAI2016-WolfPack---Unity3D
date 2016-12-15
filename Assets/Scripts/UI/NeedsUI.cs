using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class NeedsUI : MonoBehaviour {

    public Animal target;
    public Coroutine scriptUpdate;
    public Slider energy;
    public Slider hunger;
    public Slider thirst;
    public Slider social;
    // Use this for initialization
    void Start () {

        GetComponent<Canvas>().worldCamera = Camera.main;

        if (target == null)
            throw new System.Exception("Missing target");
        if (energy == null || hunger == null || thirst == null || social == null)
            throw new System.Exception("Missing need slider");


        if(scriptUpdate==null)
            scriptUpdate = StartCoroutine(Updater());
		
	}

    private void OnEnable()
    {
        if (scriptUpdate == null)
            scriptUpdate = StartCoroutine(Updater());
    }

    private IEnumerator Updater()
    {
        while (true)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                   Camera.main.transform.rotation * Vector3.up);
            if(target.needs.Initialized)
                UpdateSliders();
            yield return null;
        }
    }

    private void UpdateSliders()
    {
        var needVal = target.needs.GetNeed(NeedType.Energy);
        energy.value = energy.maxValue * (needVal / Needs.MAX_NEED_VALUE);
        needVal = target.needs.GetNeed(NeedType.Hunger);
        hunger.value = hunger.maxValue * (needVal / Needs.MAX_NEED_VALUE);
        needVal = target.needs.GetNeed(NeedType.Thirst);
        thirst.value = thirst.maxValue * (needVal / Needs.MAX_NEED_VALUE);
        needVal = target.needs.GetNeed(NeedType.Social);
        social.value = social.maxValue * (needVal / Needs.MAX_NEED_VALUE);
    }
}
