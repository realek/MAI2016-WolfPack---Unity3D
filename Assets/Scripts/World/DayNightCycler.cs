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
public class DayNightCycler : MonoBehaviour {
    public static DayNightCycler Instance;

    [SerializeField, Range(0.1f, 5.0f)]
    float m_timeRate = 1.0f;

    [Space(10)]
    public bool affectLighting = true;
    public Light dayLighting;
    public Light nightLighting;
    public Vector3 lightingDefaultRotation = new Vector3(0f, -30f, 0f);

    private WaitForSeconds m_timeTick;
    private DNCycleTime m_currentTime = DNCycleTime.Morning;
    private const int START_TIME_24H = 9; // Current start time is Morning
    private readonly int[] START_MIN = { 0, 0 };
    private const int START_DAY = 0; // Current start day is 0
    private int m_cHour;
    private int[] m_cMinutes;
    private int m_cDay;

    private int totalTime; // the current time in increments of 6 hours, used for lighting

    private const float timeIntensityDivisor = 240f; // 360/240 is 1.5f
    private const float sixHours = 360f; // totalTime/360 is 0f to 1f
    private const int minLightAngle = 5;
    private const int spanLightAngle = 85;
    private const int additionalLightAngle = 90;

    public DNCycleTime CurrentTime
    {
        get
        {
            return m_currentTime;
        }
    }

    private Coroutine m_clock;

    //returns the current time in the format "day*1000 + hour + minutes/60",
    // for example day 2, 15:45 would be presented as 2015.75
    public float GetTimeStamp() {
        return m_cDay * 1000 + m_cHour + (m_cMinutes[0] * 10 + m_cMinutes[1]) / 60;
    }

    // Use this for initialization
    void Awake()
    {
        Instance = this;
        m_timeTick = new WaitForSeconds(m_timeRate);
        m_cHour = START_TIME_24H;
        m_cMinutes = START_MIN;
        m_cDay = START_DAY;
        if (affectLighting) {
            //9am settings
            dayLighting.enabled = true;
            nightLighting.enabled = false;
            dayLighting.intensity = 0f;
            nightLighting.intensity = 1.5f;
        }
    }

    void OnEnable()
    {
        Instance = this;
        m_cHour = START_TIME_24H;
        m_cMinutes = START_MIN;
        m_cDay = START_DAY;
        
        if (affectLighting) {
            //9am settings
            dayLighting.enabled = true;
            nightLighting.enabled = false;
            dayLighting.intensity = 0f;
            nightLighting.intensity = 1.5f;
        }

        if (m_timeTick == null) //if created as enabled false
            m_timeTick = new WaitForSeconds(m_timeRate);

        m_clock = StartCoroutine(Clock());
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.T)) {
            Debug.Log(TimeString());
        }
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

            //change lighting based on time
            if (affectLighting) {
                totalTime = (m_cHour % 6) * 60 + m_cMinutes[0]*10 + m_cMinutes[1] + 1;
                if (m_cHour < 6) {
                    if (dayLighting.enabled) {
                        dayLighting.enabled = false;
                        nightLighting.enabled = true;
                        dayLighting.intensity = 0f;
                        nightLighting.intensity = 0f;
                    } else {
                        nightLighting.intensity = totalTime / timeIntensityDivisor;
                        nightLighting.transform.localRotation = Quaternion.Euler(new Vector3((totalTime / sixHours) * spanLightAngle + minLightAngle,
                            lightingDefaultRotation.y, lightingDefaultRotation.z));
                    }
                } else if (m_cHour < 12) {
                    if (dayLighting.enabled) {
                        dayLighting.enabled = false;
                        nightLighting.enabled = true;
                        dayLighting.intensity = 0f;
                        nightLighting.intensity = 1.5f;
                    } else {
                        nightLighting.intensity = 1.5f - totalTime / timeIntensityDivisor; 
                        nightLighting.transform.localRotation = Quaternion.Euler(new Vector3((totalTime / sixHours) * spanLightAngle + minLightAngle
                            + additionalLightAngle, lightingDefaultRotation.y, lightingDefaultRotation.z));
                    }
                } else if (m_cHour < 18) {
                    if (!dayLighting.enabled) {
                        dayLighting.enabled = true;
                        nightLighting.enabled = false;
                        dayLighting.intensity = 0f;
                        nightLighting.intensity = 1.5f;
                    } else {
                        dayLighting.intensity = totalTime / timeIntensityDivisor;
                        dayLighting.transform.localRotation = Quaternion.Euler(new Vector3((totalTime / sixHours) * spanLightAngle + minLightAngle,
                            lightingDefaultRotation.y, lightingDefaultRotation.z));
                    }
                } else {
                    if (!dayLighting.enabled) {
                        dayLighting.enabled = true;
                        nightLighting.enabled = false;
                        dayLighting.intensity = 1.5f;
                        nightLighting.intensity = 1.5f;
                    } else {
                        dayLighting.intensity = 1.5f - totalTime / timeIntensityDivisor;
                        dayLighting.transform.localRotation = Quaternion.Euler(new Vector3((totalTime / sixHours) * spanLightAngle + minLightAngle
                            + additionalLightAngle, lightingDefaultRotation.y, lightingDefaultRotation.z));
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

    public string TimeString() {
        return ("Day" + m_cDay + " " + m_cHour + ":" + m_cMinutes[0] + m_cMinutes[1]);
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
