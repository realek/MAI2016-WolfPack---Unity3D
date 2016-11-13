using UnityEngine;
using System.Collections;
using System;

public enum AIStoppingDistance
{
    Low,
    Medium,
    High
}
/// <summary>
/// Requires a NavMeshAgent to be present on the object onto which it is attached.
/// </summary>
[Serializable]
public class AIMovementModule : AIModule
{
    public AIStoppingDistance StoppingDistance;
    private NavMeshAgent m_navAgent;
    [SerializeField]
    private float m_lowStopDistance = 0.5f;
    [SerializeField]
    private float m_medStopDistance = 1.5f;
    [SerializeField]
    private float m_highStopDistance = 3.0f;
    private WaitForSeconds m_moduleUpdateTick;
    private Coroutine m_moduleExecutor;
    [Tooltip("How much of the detection area is representated by the obstruction area, the area is used in order to find out if the agents movement is obstructed")]
    public float ObstructionAreaRange = 0.5f;
    [SerializeField,Tooltip("The module suspends operation once it is inactive for a certain period of time, in seconds.")]
    private float m_inactivityTime;
    private bool m_isInactive;
    protected override void InitializeModule(MonoBehaviour owner)
    {
        m_navAgent = owner.GetComponent<NavMeshAgent>();
        m_moduleUpdateTick = new WaitForSeconds(UpdateRate);
        if (m_navAgent == null)
        {
            Debug.LogError("Missing NavMeshAgent", owner);
            return;
        }

        switch (StoppingDistance)
        {
            case AIStoppingDistance.Low:
                    m_navAgent.stoppingDistance = m_lowStopDistance;
                    break;

            case AIStoppingDistance.Medium:
                    m_navAgent.stoppingDistance = m_medStopDistance;
                    break;

            case AIStoppingDistance.High:
                    m_navAgent.stoppingDistance = m_highStopDistance;
                    break;
        }

       m_moduleExecutor = m_owner.StartCoroutine(ModuleLogic());

    }

    protected override void ModuleDrawGizmos()
    {
        if (m_owner != null)
        {
            Gizmos.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);
            Gizmos.DrawSphere(m_owner.transform.position, ObstructionAreaRange);
            if (m_navAgent != null && m_navAgent.path != null)
                NavMeshPathDisplay.DisplayPath(m_navAgent.path, new Color(1, 0, 0, 1));
        }
    }

    protected override IEnumerator ModuleLogic()
    {
        bool colCheck = false;
        NavMeshPath currentPath = new NavMeshPath();
        Collider gameObjCol = null;
        Vector3 oldTargetPosition = m_owner.transform.position;
        Vector3 cTargetPosition = m_owner.transform.position;
        Vector3 cFollowTargetPos = m_owner.transform.position;
        Vector3 cPos = m_owner.transform.position;
        float inactivityCounter = m_inactivityTime; 
        while (true && inactivityCounter > 0)
        {

            if (!IsPaused)
                yield return m_moduleUpdateTick;
            else
            {
                yield return null;
                continue;
            }





        }
        m_isInactive = true;

    }

    public void Move(GameObject target)
    {
        if (m_isInactive)
        {

        }
        else
        {

        }
    }

    protected override void OnModulePause()
    {
        //Pause code
    }

    protected override void ShutdownModule()
    {
        m_owner.StopCoroutine(m_moduleExecutor);
    }
}
