﻿using UnityEngine;
using System.Collections;
using System;

public class Repeater : BaseDecorator
{
    /// <summary>
    /// number of times the child is reprocessed, 0 means infinite.
    /// </summary>
    public int numberOfRepeats = 0;
    private int currentRepeats;

    public override void Start()
    {
        currentRepeats = numberOfRepeats;
        base.Start();

    }

    public override void Reset()
    {
        currentRepeats = numberOfRepeats;
        base.Reset();

    }

    public override RoutineState Tick()
    {
        switch (numberOfRepeats)
        {
            case 0:
                m_child.Tick();
                break;

            default:
                var result = m_child.Tick();
                currentRepeats--;
                if (currentRepeats == 0)
                    m_state = result;
                break;
        }

        return m_state;
    }
}