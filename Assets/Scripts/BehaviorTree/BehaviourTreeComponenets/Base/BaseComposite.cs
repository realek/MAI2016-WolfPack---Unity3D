using System.Collections.Generic;

public abstract class BaseComposite : BaseParentRoutine
{

    protected List<BaseRoutine> m_children;
    protected int currentChild;

    public override void Start()
    {
        currentChild = 0;
        m_state = RoutineState.Running;
    }

    public override void Reset()
    {
        foreach (BaseRoutine child in m_children)
        {
            child.Reset();
        }
        m_state = RoutineState.Stopped;
    }
}
