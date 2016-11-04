using UnityEngine;
using System;
using UnityEngine.Events;

public enum ActionState
{
    Stopped,
    Running,
    Completed,
    Failed
}

/// <summary>
/// Routine class, can load up an action and query for its result. Recomend using coroutines
/// </summary>
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

    /// <summary>
    /// Start function for the routine called to start it.
    /// </summary>
    public override void Start()
    {
        base.Start();
        m_actionState = ActionState.Stopped;
    }

    /// <summary>
    /// Reset function for the routine called to reset the routine after it completes
    /// </summary>
    public override void Reset()
    {
        Start();
    }

    /// <summary>
    /// Used to tick the routine in order to get its action result.
    /// </summary>
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

