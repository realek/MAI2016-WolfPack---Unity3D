﻿using UnityEngine;
using System.Collections;
using System;

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
        var result = m_condition() ? RoutineState.Succeded : RoutineState.Failed;

        if (result == RoutineState.Failed)
        {
            m_state = result;
            return m_state;
        }

        if (m_child.State == RoutineState.Stopped)
            m_child.Start();

        m_state = m_child.Tick();

        return m_state;


    }
}