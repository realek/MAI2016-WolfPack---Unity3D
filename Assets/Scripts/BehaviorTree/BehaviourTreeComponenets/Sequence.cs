using UnityEngine;

public class Sequence : BaseComposite
{

    public override void AddChild(BaseRoutine child)
    {

        if (child != null)
        {
            if (m_children == null)
                m_children = new System.Collections.Generic.List<BaseRoutine>();
            m_children.Add(child);
        }
        else
            Debug.LogError("Child object routine is null!");
    }

    public override RoutineState Tick()
    {
        if (m_children[currentChild].State == RoutineState.Stopped)
            m_children[currentChild].Start();
        else if (m_children[currentChild].IsRunning)
            return m_state;

        var result = m_children[currentChild].Tick();

        switch (result)
        {
            case RoutineState.Succeded:
                if (currentChild == m_children.Count - 1)
                    m_state = result;
                else
                    currentChild++;
                break;

            case RoutineState.Failed:
                m_state = result;
                break;
        }

        return m_state;
    }
}
