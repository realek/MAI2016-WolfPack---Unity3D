using UnityEngine;

public class Selector : BaseComposite
{
    /// <summary>
    /// Structure containing a condition and a routine, used by selectors
    /// </summary>
    public struct SelectorPair
    {
        public SelectorCondition condition;
        public BaseRoutine routine;
    }

    private new SelectorPair[] m_children;

    public void LoadChildren(params SelectorPair[] children)
    {
        m_children = children;
    }

    public override void Reset()
    {
        foreach (SelectorPair child in m_children)
        {
            child.routine.Reset();
        }
        m_state = RoutineState.Stopped;
    }

    public override RoutineState Tick()
    {
        var result = m_children[currentChild].condition.Evaluate();
        switch (result)
        {
            case RoutineState.Succeded:
                if (m_children[currentChild].routine.State == RoutineState.Stopped)
                    m_children[currentChild].routine.Start();

                m_state = m_children[currentChild].routine.Tick();
                    break;

            case RoutineState.Failed:
                if (currentChild == base.m_children.Length - 1)
                    m_state = result;
                else
                    currentChild++;
                break;
        }

        return m_state;
    }
}
