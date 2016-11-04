
/// <summary>
/// Enum containing all routine runtime states
/// </summary>
public enum RoutineState
{
    None,
    Running,
    Succeded,
    Failed
}

/// <summary>
/// Base routine class, used to create routines
/// </summary>
public abstract class BaseRoutine {

    protected RoutineState m_state = RoutineState.None;
    public RoutineState State
    {
        get
        {
            return m_state;
        }
    }

    public abstract void Tick();
    public virtual void Start()
    {
        m_state = RoutineState.Running;
    }
    public abstract void Reset();
    protected void Succed()
    {
        m_state = RoutineState.Succeded;
    }
    protected void Fail()
    {
        m_state = RoutineState.Failed;
    }
    public bool IsRunning()
    {
        return m_state == RoutineState.Running ? true : false;
    }

}
