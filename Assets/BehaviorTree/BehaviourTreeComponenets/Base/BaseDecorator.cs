public abstract class BaseDecorator : BaseRoutine
{
    protected BaseRoutine m_child;

    public void LoadChild(BaseRoutine child)
    {
        m_child = child;
    }

    public override void Start()
    {
        m_state = RoutineState.Running;
        m_child.Start();
    }

    public override void Reset()
    {
        m_child.Reset();
        m_state = RoutineState.Stopped;
    }

}
