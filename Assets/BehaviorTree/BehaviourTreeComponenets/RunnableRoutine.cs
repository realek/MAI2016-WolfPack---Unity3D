using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public enum ActionState
{
    Stopped,
    Running,
    Completed,
    Failed
}

[System.Serializable]
public class RunnableRoutine : BaseRoutine
{
    [SerializeField]
    private UnityEvent m_routineAction;
    [SerializeField]
    private ActionState m_actionState;

    public RunnableRoutine()
    {

        m_routineAction = new UnityEvent();
    }

    /// <summary>
    /// Used to load the state driven action into the routine (behavior block)
    /// </summary>
    /// <param name="action">delegate that returns a ActionState emum value.</param>
    public void LoadStateDrivenAction(Func<ActionState> action)
    {
        m_routineAction.AddListener(() => m_actionState = action.Invoke());
    }

    public override void Start()
    {
        base.Start();
        m_actionState = ActionState.Stopped;
    }

    public override void Reset()
    {
        Start();
    }

    public override void Tick()
    {
        if (m_actionState==ActionState.Running)
        {
            return;
        }

        if (m_actionState != ActionState.Stopped)
        {
            switch (m_actionState)
            {
                case ActionState.Completed:
                    {
                        Succed();
                        break;
                    }

                case ActionState.Failed:
                    {
                        Fail();
                        break;
                    }
            }
        }

        

    }
}

