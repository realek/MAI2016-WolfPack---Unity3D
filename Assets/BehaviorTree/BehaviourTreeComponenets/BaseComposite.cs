﻿using UnityEngine;
using System.Collections;
using System;

public abstract class BaseComposite : BaseRoutine
{
    public BaseRoutine[] m_children;
    int currentChild;

    public override void Start()
    {
        currentChild = 0;
        m_state = RoutineState.Running;
    }

    public void LoadChildren(params BaseRoutine[] children)
    {
        m_children = children;
    }

    public override void Reset()
    {
        foreach (BaseRoutine child in m_children)
        {
            child.Reset();
        }
        m_state = RoutineState.Stopped;
    }
}