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

    /// <summary>
    /// Get centerpoint position to group if separated
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public Vector3 GetGroupPosition(Animal self) {

        float x = 0;
        float y = 0;
        float z = 0;
        for(int i = 0; i < m_members.Count; i ++)
        {
            if(m_members[i] != self)
            {
                x += m_members[i].transform.position.x;
                y += m_members[i].transform.position.y;
                z += m_members[i].transform.position.z;
            }

        }
        //works for planes only // will add Y axis support later on.
        return new Vector3(x / m_members.Count - 1, y / m_members.Count - 1, z / m_members.Count - 1);

    }

    public int GetSize() {
        return m_members.Count;
    }
}
