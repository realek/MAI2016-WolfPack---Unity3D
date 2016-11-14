using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Used by selector class
/// </summary>
public class SelectorCondition
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

    /// <summary>
    /// Evaluate loaded condition as routine state.
    /// </summary>
    /// <returns></returns>
    public RoutineState Evaluate()
    {
        return m_condition.Invoke() ? RoutineState.Succeded : RoutineState.Failed;
    }
}
