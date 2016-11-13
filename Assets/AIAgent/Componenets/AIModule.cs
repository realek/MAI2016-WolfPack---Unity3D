using System.Collections;
using UnityEngine;

[System.Serializable]
public abstract class AIModule {

    private bool m_paused;
    private bool m_init;
    //module owner
    [SerializeField]
    protected MonoBehaviour m_owner;

    //module update rate
    public float UpdateRate = 0.2f;

    /// <summary>
    /// Module running state
    /// </summary>
    public bool IsPaused
    {
        get
        {
            return m_paused;
        }
    }

    /// <summary>
    /// Returns true if the module was initialized, shutding down the module will cause this to return false.
    /// </summary>
    public bool IsInitialized
    {
        get
        {
            return m_init;
        }
    }

    /// <summary>
    ///  Use this for initialization, takes in a monobehavoir as owner
    /// </summary>
    public void Initialize(MonoBehaviour owner)
    {
        m_paused = false;

        if(!m_init)
            InitializeModule(owner);
        m_init = true;
    }

    /// <summary>
    /// Use this to pause/unpause the module, enabled by default.
    /// </summary>
    public void Pause()
    {
        m_paused = !m_paused;
        OnModulePause();
    }

    /// <summary>
    /// Use this to shutdown the module , also performs clean up.
    /// </summary>
    public void Shutdown()
    {
        m_init = false;
        ShutdownModule();
    }

    /// <summary>
    /// Use this to draw gizmos for the module, must be called in OnDrawGizmos or OnDrawGizmosSelected
    /// </summary>
    public void DrawGizmos()
    {
        ModuleDrawGizmos();
    }

    /// <summary>
    /// Implement initialization code here.
    /// </summary>
    protected abstract void InitializeModule(MonoBehaviour owner);

    /// <summary>
    /// Implement pause state code here.
    /// </summary>
    protected abstract void OnModulePause();

    /// <summary>
    /// Implement shutdown code here.
    /// </summary>
    protected abstract void ShutdownModule();

    /// <summary>
    /// Implement module logic here
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerator ModuleLogic();

    /// <summary>
    /// Implement gizmo code for the module here
    /// </summary>
    protected abstract void ModuleDrawGizmos();

}
