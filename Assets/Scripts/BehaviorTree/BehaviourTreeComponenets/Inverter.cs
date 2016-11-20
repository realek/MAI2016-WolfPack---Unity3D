public class Inverter : BaseDecorator
{
    public override RoutineState Tick()
    {
        if (m_child.State == RoutineState.Stopped)
            m_child.Start();
        else if (m_child.IsRunning)
            return m_state;

        var result = m_child.Tick();

        switch (result)
        {
            case RoutineState.Failed:
                m_state = RoutineState.Succeded;
                break;

            case RoutineState.Succeded:
                m_state = RoutineState.Failed;
                break;
        }

        return m_state;
    }
}
