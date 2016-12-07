using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AnimalGroup {

    public string name;
    protected List<Animal> m_members;

    public AnimalGroup()
    {
        m_members = new List<Animal>();
    }

    public void AddMember(Animal animal)
    {
        if (!m_members.Contains(animal))
            m_members.Add(animal);
        else
            Debug.Log("Animal already part of group");
    }

    public void RemoveMember(Animal animal)
    {
        if (m_members.Contains(animal))
            m_members.Remove(animal);
        else
            Debug.Log("Animal not part of the group " + name);
    }

    public bool IsMember(Animal animal)
    {
        return m_members.Contains(animal);
    }
}
