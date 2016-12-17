using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalGroupManager : Singleton<AnimalGroupManager> {

    private Dictionary<int, AnimalGroup> m_groups;
    // Use this for initialization

    protected AnimalGroupManager() { } // disallow construction
    
    void Awake () {

        m_groups = new Dictionary<int, AnimalGroup>();
	}
	
    /// <summary>
    /// Registers a new wolf pack with provided initial members.
    /// </summary>
    /// <param name="initialMembers"></param>
    public void RegisterWolfPack(params Wolf[] initialMembers)
    {
        var pack = new WolfPack();
        for (int i = 0; i < initialMembers.Length; i++)
            initialMembers[i].SetGroup(pack);

        m_groups.Add(pack.GUIDGroupID, pack);
    }


    /// <summary>
    /// Registers a new herd with provided initial members.
    /// </summary>
    /// <param name="initialMembers"></param>
    public void RegisterHerd(params NonWolf[] initialMembers)
    {
        var herd = new Herd();
        for (int i = 0; i < initialMembers.Length; i++)
            initialMembers[i].SetGroup(herd);

        m_groups.Add(herd.GUIDGroupID, herd);
    }

    public List<T> GetAnimalGroupsOfType<T>() where T : AnimalGroup
    {
        var groups = m_groups.Values;
        List<T> targetGroups = new List<T>();
        foreach(AnimalGroup g in groups)
        {
            if(g.GetType() == typeof(T))
            {
                targetGroups.Add((T)g);
            }
        }
        return targetGroups;
    }

    public void UnRegisterGroup(AnimalGroup group)
    {
        m_groups.Remove(group.GUIDGroupID);
    }
}
