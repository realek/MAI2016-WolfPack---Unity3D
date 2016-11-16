using UnityEngine;
using System.Collections;
using System;

public class Sequence : BaseComposite
{

    public void LoadChildren(params BaseRoutine[] children)
    {
        m_children = children;
    }

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
                if (currentChild == m_children.Length - 1)
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
