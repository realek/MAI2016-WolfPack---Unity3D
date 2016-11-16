using UnityEngine;
using System.Collections;
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
        private GameObject targetObject;
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
}
