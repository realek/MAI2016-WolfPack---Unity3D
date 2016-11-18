using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class Action : BaseRoutine
{
    /// <summary>
    /// currently stored action
    /// </summary>
    protected Func<RoutineState> m_executableAction = null;

    public override void Reset()
    {
        m_state = RoutineState.Stopped;
    }

    public void LoadAction(Func<RoutineState> execAction)
    {
        m_executableAction = execAction;
    }

    public override void Start()
    {
        m_state = RoutineState.Running;
    }

    /// <summary>
    /// Tick the leaf node and execute its action
    /// </summary>
    /// <returns></returns>
    public override RoutineState Tick()
    {
        return m_executableAction();
    }

}
