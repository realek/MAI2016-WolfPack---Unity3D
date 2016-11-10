using UnityEngine;
using System.Collections;
/// <summary>
/// Day/Night cycle enum.
/// </summary>
public enum DNCycleTime
{
    None,
    Morning,
    Noon,
    Afternoon,
    Evening,
    Night,
    Midnight
}

/// <summary>
/// Day night cycle behavior script
/// </summary>
public class DayNightCycler : MonoBehaviour
{

    [SerializeField, Range(0.1f, 5.0f)]
    float m_timeRate = 1.0f;
    private WaitForSeconds m_timeTick;
    private DNCycleTime m_currentTime = DNCycleTime.Morning;
    private const int START_TIME_24H = 9; // Current start time is Morning
    private readonly int[] START_MIN = { 0, 0 };
    private const int START_DAY = 0; // Current start day is 0
    private int m_cHour;
    private int[] m_cMinutes;
    private int m_cDay;

    public DNCycleTime CurrentTime
    {
        get
        {
            return m_currentTime;
        }
    }

    //returns the current time in the format "day*1000 + hour + minutes/60",
    // for example day 2, 15:45 would be presented as 2015.75
    public float GetTimeStamp() {
        return m_cDay * 1000 + m_cHour + (m_cMinutes[0] * 10 + m_cMinutes[1]) / 60;
    }

    private Coroutine m_clock;

    // Use this for initialization
    void Awake()
    {
        m_timeTick = new WaitForSeconds(m_timeRate);
        m_cHour = START_TIME_24H;
        m_cMinutes = START_MIN;
        m_cDay = START_DAY;
    }

    void OnEnable()
    {
        m_cHour = START_TIME_24H;
        m_cMinutes = START_MIN;
        m_cDay = START_DAY;

        if (m_timeTick == null) //if created as enabled false
            m_timeTick = new WaitForSeconds(m_timeRate);

        m_clock = StartCoroutine(Clock());
    }

    //Method is used to simulate the Day/Night cycle
    IEnumerator Clock()
    {
        while (true)
        {
            yield return m_timeTick;
            //increment time
            if (m_cMinutes[1] >= 0 && m_cMinutes[1] < 9)
                m_cMinutes[1]++;
            else if (m_cMinutes[1] == 9)
            {
                m_cMinutes[1] = 0;
                if (m_cMinutes[0] >= 0 && m_cMinutes[0] < 5)
                    m_cMinutes[0]++;
                else if (m_cMinutes[0] == 5)
                {
                    m_cMinutes[0] = 0;
                    if (m_cHour >= 0 && m_cHour < 23)
                        m_cHour++;
                    else if (m_cHour == 23) {
                        m_cHour = 0;
                        m_cDay++;
                    }
                }
            }
            SetCurrentTime();
        }
    }

    //Used to set the current time value based on the hour
    private void SetCurrentTime()
    {
        //Set current time of day
        if (m_cHour == 0)
            m_currentTime = DNCycleTime.Midnight;
        else if (m_cHour > 0 && m_cHour < 12)
            m_currentTime = DNCycleTime.Morning;
        else if (m_cHour == 12)
            m_currentTime = DNCycleTime.Noon;
        else if (m_cHour > 12 && m_cHour <= 18)
            m_currentTime = DNCycleTime.Afternoon;
        else if (m_cHour > 18 && m_cHour <= 23)
            m_currentTime = DNCycleTime.Night;
    }

    void OnDisable()
    {
        StopCoroutine(m_clock);
    }
    ///Debuging LOGIC, On Inspector change
    void OnValidate()
    {
        m_timeTick = new WaitForSeconds(m_timeRate);
    }
}
