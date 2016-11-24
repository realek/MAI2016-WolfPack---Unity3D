using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

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
    [SerializeField]
    private AIStoppingDistance m_stoppingDistance;
    public AIStoppingDistance StoppingDistance
    {
        set
        {
            m_stoppingDistance = value;
            SetStoppingDistance();

        }
        get { return m_stoppingDistance; }
    }
    private NavMeshAgent m_navAgent;
    [SerializeField]
    private float m_lowStopDistance = 0.5f;
    [SerializeField]
    private float m_medStopDistance = 1.5f;
    [SerializeField]
    private float m_highStopDistance = 3.0f;
    [SerializeField,Tooltip("The distance threshold between the last known position of the target and the current position, if it is exceeded the path will update.")]
    private float m_targetPositionDiff = 0.5f;
    private WaitForSeconds m_moduleUpdateTick;
    private Coroutine m_moduleExecutor;
    [Tooltip("How much of the detection area is representated by the obstruction area, the area is used in order to find out if the agents movement is obstructed")]
    public float ObstructionAreaRange = 0.5f;
    [SerializeField,Tooltip("The module suspends operation once it is inactive for a certain period of time, in seconds.")]
    private float m_inactivityTime = 10.0f;
    //module update rate
    public float UpdateRate = 0.2f;
    private bool m_isInactive;
    private GameObject m_target;
    private bool m_targetReached;
    private bool m_failed;
    public GameObject target
    {
        get
        {
            return m_target;
        }
    }

    public bool reachedTarget
    {
        get
        {
            return m_targetReached;
        }
    }

    public bool unreachableTarget
    {
        get
        {
            return m_failed;
        }
    }

    protected override void InitializeModule(MonoBehaviour owner)
    {
        m_owner = owner;
        m_navAgent = owner.GetComponent<NavMeshAgent>();
        m_moduleUpdateTick = new WaitForSeconds(UpdateRate);
        if (m_navAgent == null)
        {
            Debug.LogError("Missing NavMeshAgent", owner);
            return;
        }

        SetStoppingDistance();

    }

    private void SetStoppingDistance()
    {
        switch (m_stoppingDistance)
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
    }

    protected override void ModuleDrawGizmos()
    {
        Gizmos.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);
        Gizmos.DrawSphere(m_owner.transform.position, ObstructionAreaRange);
        if (m_navAgent != null && m_navAgent.path != null)
            NavMeshPathDisplay.DisplayPath(m_navAgent.path, new Color(1, 1, 0, 1));
    }

    protected IEnumerator ModuleLogic()
    {
        bool colCheck = false;
        NavMeshPath currentPath = new NavMeshPath();
        Collider gameObjCol = null;
        Vector3 oldTargetPosition = m_owner.transform.position;
        m_targetReached = false;
        float inactivityCounter = m_inactivityTime; 


        while (inactivityCounter > 0)
        {

            if (!IsPaused)
                yield return m_moduleUpdateTick;
            else
            {
                yield return null;
                continue;
            }


            if (!m_targetReached && !m_failed)
            {
                //check if target has a collider
                if (gameObjCol == null && !colCheck || gameObjCol != null &&
                    gameObjCol.gameObject.GetInstanceID() != m_target.GetInstanceID())
                {
                    colCheck = true;
                    gameObjCol = m_target.GetComponent<Collider>();
                }

                //if distance threshold is exceeded recompute path
                if ((m_target.transform.position - oldTargetPosition).sqrMagnitude > m_targetPositionDiff * m_targetPositionDiff)
                {
                    if (gameObjCol != null)
                        oldTargetPosition = gameObjCol.ClosestPointOnBounds(m_owner.transform.position);
                    else
                        oldTargetPosition = m_target.transform.position;

                    if (m_navAgent.CalculatePath(oldTargetPosition, currentPath))
                        m_navAgent.SetPath(currentPath);
                    else
                        m_failed = true;
                }

                if (m_navAgent.remainingDistance < m_navAgent.stoppingDistance)
                {
                    m_navAgent.ResetPath();
                    currentPath.ClearCorners();
                    m_targetReached = true;
                }

            }
            else
                inactivityCounter -= UpdateRate;

        }
        m_isInactive = true;

    }

    public void Move(GameObject target)
    {
        m_targetReached = false;
        m_failed = false;
        if (m_isInactive || m_moduleExecutor==null)
        {
            m_target = target;
            m_moduleExecutor = m_owner.StartCoroutine(ModuleLogic());
        }
        else
            m_target = target;
    }

    protected override void ShutdownModule()
    {
        if (m_moduleExecutor != null)
        {
            m_owner.StopCoroutine(m_moduleExecutor);
        }

    }
}
