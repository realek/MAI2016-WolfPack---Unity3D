public class Selector : BaseComposite
{
    public override RoutineState Tick()
    {
        if (m_children[currentChild].State == RoutineState.Stopped)
            m_children[currentChild].Start();
        else if (m_children[currentChild].IsRunning())
            return m_state;

        var result = m_children[currentChild].Tick();

        switch (result)
        {
            case RoutineState.Succeded:
                m_state = result;
                break;

            case RoutineState.Failed:
                if (currentChild == m_children.Length - 1)
                    m_state = result;
                else
                    currentChild++;
                break;
        }

        return m_state;
    }
}
