
/// <summary>
/// Enum containing all routine runtime states
/// </summary>
public enum RoutineState
{
    Stopped,
    Running,
    Succeded,
    Failed
}

/// <summary>
/// Base routine class, used to create routines
/// </summary>
public abstract class BaseRoutine {

    protected RoutineState m_state = RoutineState.Stopped;


    public string name;
    public RoutineState State
    {
        get
        {
            return m_state;
        }
    }


    public bool IsRunning
    {
        get
        {
            return m_state == RoutineState.Running ? true : false;
        }

    }

    public abstract RoutineState Tick();
    public abstract void Start();
    public abstract void Reset();


}
