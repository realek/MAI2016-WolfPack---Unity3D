using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

[System.Serializable]
public class RunnableRoutine : BaseRoutine
{
    [SerializeField]
    private UnityEvent m_routineAction;
    [SerializeField]
    private bool m_actionState;

    public RunnableRoutine()
    {

        m_routineAction = new UnityEvent();
    }


    public void LoadAction(Func<bool> action)
    {
        m_routineAction.AddListener(() => m_actionState = action.Invoke());
    }

    public override void Start()
    {
        base.Start();

    }

    public override void Reset()
    {
        throw new NotImplementedException();
    }

    public override void Tick()
    {
        if (IsRunning())
        {
            m_routineAction.Invoke();
            if (m_actionState)
            {
                Succed();
            }
            else
            {
                Fail();
            }
        }

    }
}

