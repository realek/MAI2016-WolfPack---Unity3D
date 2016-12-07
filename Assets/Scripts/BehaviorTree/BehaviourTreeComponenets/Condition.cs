using System;
using UnityEngine;
/// <summary>
/// Used by selector class, loads up a boolean function as the execution condition and a baseroutine as the executable
/// </summary>
public class Condition : BaseDecorator
{
    private Func<bool> m_condition;

    /// <summary>
    /// Load a function as a condition
    /// </summary>
    /// <param name="condition"></param>
    public void LoadCondition(Func<bool> condition)
    {
        m_condition = condition;
    }

    public override void Start()
    {
        m_state = RoutineState.Running;
    }

    public override void Reset()
    {
        m_child.Reset();
        m_state = RoutineState.Stopped;
    }

    /// <summary>
    /// Condition
    /// </summary>
    /// <returns></returns>
    public override RoutineState Tick()
    {
        var result = m_condition();

        if (!result)
        {
            m_state = RoutineState.Failed;
            return m_state;
        }
        else
        {
            if (m_child.State == RoutineState.Stopped)
                m_child.Start();

            m_state = m_child.Tick();
        }
        return m_state;


    }

    public override bool HasChildren()
    {
        return m_child != null;
    }
}
