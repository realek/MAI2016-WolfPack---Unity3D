public class RunnableSelector : BaseRoutine
{
    private BaseRoutine[] m_storedRoutines;
    int id;
    public RunnableSelector(params BaseRoutine[] routines)
    {
        m_storedRoutines = routines;
    }

    /// <summary>
    /// Starts the selector and resets the incrementor to 0.
    /// </summary>
    public override void Start()
    {
        base.Start();
        id = 0;
    }

    /// <summary>
    /// Resets the selector and its stored routines also calls start.
    /// </summary>
    public override void Reset()
    {
        for (int i = 0; i < m_storedRoutines.Length; i++)
        {
            m_storedRoutines[i].Reset();
        }
        Start();
    }

    /// <summary>
    /// Ticks the selector and its current routine, if the routine succedes, the selector succedes.
    /// </summary>
    public override void Tick()
    {
        if (IsRunning())
        {
            if (m_storedRoutines[id].IsRunning())
                return;

            if (id == 0 && m_storedRoutines[id].State == RoutineState.None)
                m_storedRoutines[id].Start();

            m_storedRoutines[id].Tick();

            if (m_storedRoutines[id].State == RoutineState.Succeded)
            {
                Succed();
                return;
            }
            else //routine didnt succed continue to the next, or retrieve state if its the last one
            {
                id++;
                if (id == m_storedRoutines.Length)
                {
                    Fail();
                    return;
                }
                m_storedRoutines[id].Start();
            }
        }
    }
}
