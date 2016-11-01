using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;

internal enum MovementType
{
    None,
    Destination,
    Follow
}


[RequireComponent(typeof(Rigidbody),typeof(NavMeshAgent))]
public class AIAgent : MonoBehaviour {

    //Sensor Variables
    [SerializeField]
    private LayerMask m_detectionLayer;
    [SerializeField]
    private float m_detectionAreaRadius;
    [SerializeField,Range(0.1f,0.25f), Tooltip("How much of the detection area is representated by the avoidance area, the area is used in order to avoid other agents while moving.")]
    private float m_avoidanceAreaPercentage = 0.1f;
    [SerializeField, Range(0.2f, 0.9f), Tooltip("How much of the detection area is represented by the hearing range.")]
    private float m_hearingAreaPercentage = 0.2f;
    [SerializeField, Range(0, 1.0f), Tooltip("How far into the detection area can the agent see.")]
    private float m_sightRange = 0.0f;
    [SerializeField, Tooltip("In degrees.")]
    private float m_sightAngle = 0.0f;
    [SerializeField]
    private List<Collider> m_detectedEntities;
    private float m_cDARadius;
    private float m_cAARadius;
    private float m_cHARadius;
    private float m_cSRange;
    private WaitForSeconds m_sensorTick;
    private float m_agentSurf;

    //Movement Variables
    private MovementType m_moveType;
    private NavMeshAgent m_navAgent;
    private WaitForSeconds m_movementUpdateTick;
    private GameObject m_followTarget;
    private Vector3 m_worldDestination;

    // Accesors
    public GameObject CurrentFollowTarget
    {
        get
        {
            return m_followTarget;
        }
    }
    public Vector3 CurrentWorldDestination
    {
        get
        {
            return m_worldDestination;
        }
    }

    //Constant Variables
    private const float PATH_UPDATE_RATE = 0.5f;
    private const float PATH_DEST_DIFF = 0.5f;
    private const float SENSOR_UPDATE_TICK = 0.2f;
    private bool m_avoiding;
    //TESTING
    public GameObject TESTTARGET;
    //ENDOFTESTING

    void Awake()
    {
        m_navAgent = GetComponent<NavMeshAgent>();
        if (m_navAgent == null)
        {
            Debug.LogError("Navmesh agent is missing,", gameObject);
            enabled = false;
            return;
        }
        Rigidbody rb = GetComponent<Rigidbody>();
        if (!rb.isKinematic)
        {
            rb.isKinematic = true;
        }

        Collider agentCol = GetComponent<Collider>();
        m_agentSurf = (agentCol.bounds.extents.z + agentCol.bounds.extents.x) / 2;
        m_cDARadius = m_detectionAreaRadius + m_agentSurf;
        m_cAARadius = m_cDARadius * m_avoidanceAreaPercentage;
        m_cHARadius = m_cDARadius * m_hearingAreaPercentage;
        m_cSRange = m_cDARadius * m_sightRange;
        m_detectedEntities = new List<Collider>();
        m_sensorTick = new WaitForSeconds(SENSOR_UPDATE_TICK);
        m_movementUpdateTick = new WaitForSeconds(PATH_UPDATE_RATE);
    }

    void OnEnable()
    {
        StartUpAgent();
    }

    void OnDisable()
    {
        ShutdownAgent();
    }

    /// <summary>
    /// Calling this function will stop all coroutines within the AIAgent componenet.
    /// </summary>
    public void ShutdownAgent()
    {
        StopAllCoroutines();
        m_detectedEntities = null;
        m_followTarget = null;
    }

    public void StartUpAgent()
    {
        m_detectedEntities = new List<Collider>();
        m_moveType = MovementType.None;
        StartCoroutine(Detection());
        StartCoroutine(Movement());
    }

    public void SetStoppingDistance(float distance)
    {
        m_navAgent.stoppingDistance = distance;
    }

    private bool EntityAtDestination(ref List<Collider> entities)
    {
        int i;
        
        if(entities.Count > 0)
        {
            for (i = 0; i < entities.Count; i++)
            {
                if(m_moveType == MovementType.Follow && m_followTarget!=null &&
                    entities[i].gameObject.GetInstanceID() == m_followTarget.GetInstanceID())
                {
                    continue;
                }
                Vector3 closestPoint = entities[i].ClosestPointOnBounds(transform.position);
                if (PointInsideSphere(closestPoint, transform.position, m_cAARadius))
                {
                   
                    float entitySurf = (entities[i].bounds.extents.z + entities[i].bounds.extents.x)/2;
                    if(m_navAgent.path.corners.Length > 1 && m_navAgent.remainingDistance < m_navAgent.stoppingDistance+entitySurf)
                    {
                        return true;
                    }
                    
                }
            }
            return false;
        }
        return false;



    }

    private bool Sight(Transform entity)
    {
        Vector3 dir = entity.position - transform.position;
        float cAngle = Vector3.Angle(dir, transform.forward);

        if (cAngle < m_sightAngle / 2)
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position,dir,out hit, m_cSRange))
            {
                if(hit.collider.gameObject == entity.gameObject)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        return false;
    }
    /// <summary>
    /// Helper function for checking if a point is inside a sphere.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="sc"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    private bool PointInsideSphere(Vector3 p,Vector3 sc,float radius)
    {
        float d = (p - sc).sqrMagnitude;
        if(d < radius*radius)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Detection coroutine, handles sight & hearing for the agent.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Detection()
    {
        int i;

        DetectionStart:

        Collider[] entities = Physics.OverlapSphere(transform.position, m_cDARadius, m_detectionLayer);

       if(entities.Length > 0)
        {
            for (i = 0; i < entities.Length; i++)
            {

                if (entities[i].gameObject.GetInstanceID() == gameObject.GetInstanceID())
                {
                    continue;
                }

                if (PointInsideSphere(entities[i].transform.position, transform.position, m_cHARadius)
                     || Sight(entities[i].transform))
                {
                    if (!m_detectedEntities.Contains(entities[i]))
                    {
                        m_detectedEntities.Add(entities[i]);
                    }
                }
            }
        }
        entities = null;

        yield return m_sensorTick;
        for (i = 0; i < m_detectedEntities.Count; i++)
        {
            if(m_detectedEntities[i]==null || m_detectedEntities[i]!=null &&
                !PointInsideSphere(m_detectedEntities[i].transform.position, transform.position, m_cDARadius))
            {
                m_detectedEntities.RemoveAt(i);
            }
        }

        goto DetectionStart;
    }

    private IEnumerator Movement()
    {
        bool colCheck = false;
        NavMeshPath currentPath = new NavMeshPath();
        Collider gameObjCol = null;
        Vector3 oldTargetPosition = transform.position;
        Vector3 cTargetPosition = transform.position;
        Vector3 cFollowTargetPos = transform.position;
        Vector3 cPos = transform.position;

        MovementLoop:

        switch (m_moveType)
        {
            case MovementType.Destination:
                {
                    if(cTargetPosition != m_worldDestination)
                    {
                        cTargetPosition = m_worldDestination;
                        m_navAgent.CalculatePath(cTargetPosition,currentPath);
                    }
                    break;
                }
            case MovementType.Follow:
                {
                    if (gameObjCol == null && !colCheck || gameObjCol!=null &&
                        gameObjCol.gameObject.GetInstanceID() != m_followTarget.GetInstanceID())
                    {
                        colCheck = true;
                        gameObjCol = m_followTarget.GetComponent<Collider>();
                    }

                    if (!cFollowTargetPos.Equals(m_followTarget.transform.position))
                    {
                        if(gameObjCol != null)
                        {
                            cTargetPosition = gameObjCol.ClosestPointOnBounds(transform.position);
                        }
                        else
                        {
                            cTargetPosition = m_followTarget.transform.position;
                        }
                       Debug.Log("Compute Pos");

                    }

                    if (Vector3.Distance(oldTargetPosition,cTargetPosition) > PATH_DEST_DIFF)
                    {
                        m_navAgent.CalculatePath(cTargetPosition, currentPath);
                    }

                    oldTargetPosition = cTargetPosition;
                    cFollowTargetPos = m_followTarget.transform.position;
                    break;
                }
        }

        if(currentPath.status == NavMeshPathStatus.PathComplete)
        {
            if(currentPath != m_navAgent.path)
            {
                m_navAgent.SetPath(currentPath);
            }
        }

        if(m_navAgent.path !=null)
        {
            switch (m_moveType)
            {
                case MovementType.Destination:
                    {
                        if (m_navAgent.remainingDistance < m_navAgent.stoppingDistance && m_navAgent.path.corners.Length > 1 || EntityAtDestination(ref m_detectedEntities))
                        {
                            m_navAgent.ResetPath();
                        }
                            break;
                    }
                case MovementType.Follow:
                    {
                        if (m_navAgent.remainingDistance < m_navAgent.stoppingDistance && m_navAgent.path.corners.Length > 1 || EntityAtDestination(ref m_detectedEntities))
                        {
                            m_navAgent.ResetPath();
                            currentPath = m_navAgent.path;
                            m_navAgent.Stop();
                        }
                        else if (m_navAgent.remainingDistance > m_navAgent.stoppingDistance && cPos == transform.position)
                        {
                            m_navAgent.Resume();
                        }
                        cPos = transform.position;
                        break;
                    }
            }

        }

       
        yield return m_movementUpdateTick;
        goto MovementLoop;
    }
    
    public void Move(Vector3 destination)
    {
        if(m_worldDestination != destination)
        {
            m_worldDestination = destination;
        }

        if (m_moveType != MovementType.Destination)
        {
            m_moveType = MovementType.Destination;
        }

    }

    public void Follow(GameObject target)
    {
        if(m_followTarget==null || m_followTarget.GetInstanceID()!= target.GetInstanceID())
        {
            m_followTarget = target;
        }

        if(m_moveType != MovementType.Follow)
        {
            m_moveType = MovementType.Follow;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f,0.15f,0.2f,0.25f);
        Gizmos.DrawSphere(transform.position, m_detectionAreaRadius + m_agentSurf);
        Gizmos.color = new Color(0.25f, 1, 0, 0.25f);
        Gizmos.DrawSphere(transform.position, (m_detectionAreaRadius + m_agentSurf) * m_hearingAreaPercentage);
        Gizmos.color = new Color(0, 0.5f, 0.75f, 0.25f);
        Gizmos.DrawSphere(transform.position, (m_detectionAreaRadius + m_agentSurf) * m_avoidanceAreaPercentage);
        Gizmos.color = new Color(1,1,1,0.75f);
        Vector3 sightFalloff = transform.position;
        sightFalloff.z += (m_detectionAreaRadius + m_agentSurf) * m_sightRange;
        sightFalloff = transform.position + (transform.rotation * (sightFalloff - transform.position));
        Gizmos.DrawLine(transform.position,sightFalloff);

    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
          //  Debug.Log("FIRED");

   
            Follow(TESTTARGET);
            
        }


    }
}
