using UnityEngine;
using System.Collections;
using System;

public class RunnableSequence : BaseRoutine
{
    private BaseRoutine[] m_storedRoutines;
    int id;
    public RunnableSequence(params BaseRoutine[] routines)
    {
        m_storedRoutines = routines;
    }

    public override void Start()
    {
        base.Start();
        id = 0;
    }

    public override void Reset()
    {
        for (int i = 0; i < m_storedRoutines.Length; i++)
        {
            m_storedRoutines[i].Reset();
        }
        Start();
    }

    public override void Tick()
    {
        if (IsRunning())
        {
            m_storedRoutines[id].Start();
            if (m_storedRoutines[id].IsRunning())
                return;

            if (m_storedRoutines[id].State == RoutineState.Succeded)
            {
                id++;
                if (id == m_storedRoutines.Length)
                    Succed();
            }
            else
            {
                Fail();
            }
        }
    }
}
