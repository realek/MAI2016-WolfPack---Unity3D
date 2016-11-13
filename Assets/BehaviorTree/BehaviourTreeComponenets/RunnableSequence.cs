//public class RunnableSequence : BaseRoutine
//{
//    private BaseRoutine[] m_storedRoutines;
//    int id;
//    public RunnableSequence(params BaseRoutine[] routines)
//    {
//        m_storedRoutines = routines;
//    }

//    /// <summary>
//    /// Used to start the sequence, sets the increment to 0
//    /// </summary>
//    public override void Start()
//    {
//        base.Start();
//        id = 0;
//    }

//    /// <summary>
//    /// Used to reset the sequence and all its stored routines, calls start
//    /// </summary>
//    public override void Reset()
//    {
//        for (int i = 0; i < m_storedRoutines.Length; i++)
//        {
//            m_storedRoutines[i].Reset();
//        }
//        Start();
//    }

//    /// <summary>
//    /// Used to tick the sequence, and its current routine. Returns if the routine is still running
//    /// </summary>
//    public override void Tick()
//    {
//        if (IsRunning())
//        {
//            if (m_storedRoutines[id].IsRunning())
//                return;

//            if (id==0 && m_storedRoutines[id].State == RoutineState.None)
//                m_storedRoutines[id].Start();

//            m_storedRoutines[id].Tick();

//            //continue if success
//            if (m_storedRoutines[id].State == RoutineState.Succeded)
//            {
//                id++;
//                if (id == m_storedRoutines.Length)
//                {
//                    Succed();
//                    return;
//                }
//                m_storedRoutines[id].Start();
//            }
//            else //fail if failed
//            {
//                Fail();
//                return;
//            }
//        }
//    }
//}
