using UnityEngine;

public class Selector : BaseComposite
{
    protected new System.Collections.Generic.List<Condition> m_children;

    public override void AddChild(BaseRoutine child)
    {
        if (m_children == null)
            m_children = new System.Collections.Generic.List<Condition>();

        if (child.GetType() != typeof(Condition))
        {
            Debug.LogError("Non-condition node was passed to a Selector type node");
            return;
        }

        m_children.Add((Condition)child);
    }

    public override void Reset()
    {
        foreach (Condition child in m_children)
        {
            child.Reset();
        }
        m_state = RoutineState.Stopped;
    }

    public override RoutineState Tick()
    {
        if (m_children[currentChild].State == RoutineState.Stopped)
            m_children[currentChild].Start();

        var result = m_children[currentChild].Tick();

        if (result == RoutineState.Succeded || result == RoutineState.Running)
        {
            m_state = result;
            return m_state;
        }
        else
        {
            if (currentChild == m_children.Count - 1)
                m_state = result;
            else
                currentChild++;
            return m_state;
        }
    }
}
