using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class AIDetectionModule : AIModule {


    public LayerMask m_detectionLayer;
    [Tooltip("Max range for the module's detection functionality.")]
    public float DetectionAreaRadius;
    [Range(0.1f, 0.9f), Tooltip("How much of the detection area is representated by the obstruction area, the area is used in order to find out if the agents movement is obstructed")]
    public float HearingAreaPercentage = 0.2f;
    [Range(0, 1.0f), Tooltip("How far into the detection area can the agent see.")]
    public float SightRange = 0.0f;
    [Tooltip("In degrees.")]
    public float SightAngle = 0.0f;
    [SerializeField]
    private List<Collider> m_detectedEntities;
    private float m_cDARadius;
    private float m_cHARadius;
    private float m_cSRange;
    //module update rate
    public float UpdateRate = 0.2f;
    public List<Collider> DetectedGameObjects
    {
        get
        {
            return m_detectedEntities;
        }
    }
    private WaitForSeconds m_moduleUpdateTick;
    private Coroutine m_moduleExecutor;
    


    protected override void InitializeModule(MonoBehaviour owner)
    {
        m_owner = owner;
        m_moduleUpdateTick = new WaitForSeconds(UpdateRate);
        m_cDARadius = DetectionAreaRadius;
        m_cHARadius = m_cDARadius * HearingAreaPercentage;
        m_cSRange = m_cDARadius * SightRange;
        m_detectedEntities = new List<Collider>();
        m_moduleExecutor = m_owner.StartCoroutine(ModuleLogic());
        
       
    }



    /// <summary>
    /// LOS method returns true if gameobject is in LOS.
    /// </summary>
    /// <param name="gameobject"></param>
    /// <returns></returns>
    private bool Sight(Transform gameobject)
    {
        Vector3 dir = gameobject.position -  m_owner.transform.position;
        float cAngle = Vector3.Angle(dir, m_owner.transform.forward);

        if (cAngle < SightAngle / 2)
        {
            RaycastHit hit;
            if (Physics.Raycast(m_owner.transform.position, dir, out hit, m_cSRange))
            {
                if (hit.collider.gameObject == gameobject.gameObject)
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
    private bool PointInsideSphere(Vector3 p, Vector3 sc, float radius)
    {
        float d = (p - sc).sqrMagnitude;
        if (d < radius * radius)
        {
            return true;
        }
        return false;
    }

    protected override void ShutdownModule()
    {
        if(m_moduleExecutor!=null)
            m_owner.StopCoroutine(m_moduleExecutor);
        m_moduleExecutor = null;
        m_detectedEntities = null;
    }

    protected IEnumerator ModuleLogic()
    {
        while (true)
        {

            if (!IsPaused)
                yield return m_moduleUpdateTick;
            else
            {
                yield return null;
                continue;
            }
            int i;
            Collider[] entities = Physics.OverlapSphere(m_owner.transform.position, m_cDARadius, m_detectionLayer);

            if (entities.Length > 0)
            {
                for (i = 0; i < entities.Length; i++)
                {

                    if (entities[i].gameObject.GetInstanceID() == m_owner.GetInstanceID())
                    {
                        continue;
                    }

                    if (PointInsideSphere(entities[i].transform.position, m_owner.transform.position, m_cHARadius)
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

            for (i = 0; i < m_detectedEntities.Count; i++)
            {
                if (m_detectedEntities[i] == null || m_detectedEntities[i] != null &&
                    !PointInsideSphere(m_detectedEntities[i].transform.position, m_owner.transform.position, m_cDARadius))
                {
                    m_detectedEntities.RemoveAt(i);
                }
            }

        }
    }

    protected override void ModuleDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0.15f, 0.2f, 0.25f);
        Gizmos.DrawSphere(m_owner.transform.position, DetectionAreaRadius);
        Gizmos.color = new Color(0.25f, 1, 0, 0.25f);
        Gizmos.DrawSphere(m_owner.transform.position, (DetectionAreaRadius) * HearingAreaPercentage);
        Gizmos.color = new Color(1, 0, 0, 1f);
        Vector3 sightFalloff = m_owner.transform.position;
        sightFalloff.z += (DetectionAreaRadius) * SightRange;
        sightFalloff = m_owner.transform.position + (m_owner.transform.rotation * (sightFalloff - m_owner.transform.position));
        Gizmos.DrawLine(m_owner.transform.position, sightFalloff);
        Gizmos.DrawLine(m_owner.transform.position, Quaternion.Euler(0, SightAngle / 2, 0) * (sightFalloff - m_owner.transform.position) + m_owner.transform.position);
        Gizmos.DrawLine(m_owner.transform.position, Quaternion.Euler(0, -(SightAngle / 2), 0) * (sightFalloff - m_owner.transform.position) + m_owner.transform.position);

    }
}
