using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class AIMemoryModule : AIModule
{
    [Serializable]
    private struct Memory
    {
        [SerializeField]
        private MemoryTarget target;
        [SerializeField]
        public GameObject targetObject;
    }

    internal enum MemoryTarget
    {
        Location,
        Actor
    }

    [SerializeField]
    private List<Memory> m_staticObjects; //static always known objects
    private Dictionary<Memory,Vector3> m_dynamicObjects; //last known position of dynamic object

    protected override void InitializeModule(MonoBehaviour owner)
    {
        m_owner = owner;
    }

    protected override void ModuleDrawGizmos()
    {

    }

    protected override void OnModulePause()
    {
      
    }

    protected override void ShutdownModule()
    {
        
    }

    //get a list of all memorized obejects
    public List<GameObject> GetRememberedObjects() {
        List<GameObject> myMemory = new List<GameObject>();

        if (m_staticObjects != null && m_staticObjects.Count > 0)
            foreach (Memory mem in m_staticObjects) {
                myMemory.Add(mem.targetObject);
            }
        if (m_dynamicObjects != null && m_dynamicObjects.Count > 0)
            foreach (KeyValuePair<Memory, Vector3> mem in m_dynamicObjects) {
                myMemory.Add(mem.Key.targetObject);
            }

        return myMemory;
    }
}
