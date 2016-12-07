public abstract class BaseDecorator : BaseParentRoutine
{
    protected BaseRoutine m_child;

    public override void AddChild(BaseRoutine child)
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
