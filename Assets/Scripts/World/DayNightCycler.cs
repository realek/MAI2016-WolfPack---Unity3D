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
    public bool dynamicLighting = true;
    public Light dayLighting;
    public Light nightLighting;
    public Vector3 lightingDefaultRotation = new Vector3(0f, -30f, 0f);

    private WaitForSeconds m_timeTick;
    private DNCycleTime m_currentTime = DNCycleTime.Morning;
    private const int START_TIME_24H = 16; // Current start time is Morning
    private readonly int[] START_MIN = { 3, 0 };
    private const int START_DAY = 0; // Current start day is 0
    private int m_cHour;
    private int[] m_cMinutes;
    private int m_cDay;

    private int totalTime; // the current time in increments of 6 hours, used for lighting

    private const float sixHours = 360f; // totalTime/360 is 0f to 1f
    private const int minLightAngle = 20;
    private const int spanLightAngle = 70;
    private const int additionalLightAngle = 90;
    private const float minIntensityD = 0.5f;
    private const float maxIntensityD = 1.7f;
    private const float minIntensityN = 1.3f;
    private const float maxIntensityN = 2.5f;

    private float currX;

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
    }

    void OnEnable()
    {
        Instance = this;
        m_cHour = START_TIME_24H;
        m_cMinutes = START_MIN;
        m_cDay = START_DAY;
        
        if (dynamicLighting) {
            dayLighting.enabled = true;
            if (START_TIME_24H < 5 || START_TIME_24H >= 19) {
                dayLighting.enabled = false;
                nightLighting.enabled = true;
            } else if (START_TIME_24H >= 7 && START_TIME_24H < 19) {
                dayLighting.enabled = true;
                nightLighting.enabled = false;
            } else {
                dayLighting.enabled = true;
                nightLighting.enabled = true;
            }
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
            if (dynamicLighting) {
                totalTime = (m_cHour % 6) * 60 + m_cMinutes[0]*10 + m_cMinutes[1] + 1;
                if (m_cHour < 5) {
                    if (dayLighting.enabled) {
                        dayLighting.enabled = false;
                        nightLighting.enabled = true;
                        //PreySpawner.Instance.Repopulate(); //check if the world needs repopulation once per day
                    }
                    nightLighting.intensity = minIntensityN + (maxIntensityN - minIntensityN) * ((sixHours - totalTime) / sixHours);
                    nightLighting.transform.localRotation = Quaternion.Euler(new Vector3((totalTime / sixHours) * spanLightAngle
                        + additionalLightAngle, lightingDefaultRotation.y, lightingDefaultRotation.z));
                } else if (m_cHour < 6) {
                    if (!dayLighting.enabled) {
                        dayLighting.enabled = true;
                    }
                    nightLighting.intensity = minIntensityN + (maxIntensityN - minIntensityN) * ((sixHours - totalTime) / sixHours);
                    nightLighting.transform.localRotation = Quaternion.Euler(new Vector3((totalTime / sixHours) * spanLightAngle
                        + additionalLightAngle, lightingDefaultRotation.y, lightingDefaultRotation.z));

                    dayLighting.intensity = minIntensityD * ((totalTime - 300) / 60f);
                    dayLighting.transform.localRotation = Quaternion.Euler(new Vector3(minLightAngle * ((totalTime - 300)/60f),
                        lightingDefaultRotation.y, lightingDefaultRotation.z));
                } else if (m_cHour < 7) {
                    nightLighting.intensity = minIntensityN* ((60 - totalTime) / 60f);
                    nightLighting.transform.localRotation = Quaternion.Euler(new Vector3(minLightAngle * (totalTime / 60f) + spanLightAngle
                        + additionalLightAngle, lightingDefaultRotation.y, lightingDefaultRotation.z));

                    dayLighting.intensity = minIntensityD + (maxIntensityD - minIntensityD) * (totalTime / sixHours);
                    dayLighting.transform.localRotation = Quaternion.Euler(new Vector3((totalTime / sixHours) * spanLightAngle + minLightAngle,
                        lightingDefaultRotation.y, lightingDefaultRotation.z));
                } else if (m_cHour < 12) {
                    if (nightLighting.enabled) {
                        dayLighting.enabled = true;
                        nightLighting.enabled = false;
                    }
                    dayLighting.intensity = minIntensityD + (maxIntensityD - minIntensityD) * (totalTime / sixHours);
                    dayLighting.transform.localRotation = Quaternion.Euler(new Vector3((totalTime / sixHours) * spanLightAngle + minLightAngle,
                            lightingDefaultRotation.y, lightingDefaultRotation.z));
                } else if (m_cHour < 17) {
                    if (nightLighting.enabled) {
                        dayLighting.enabled = true;
                        nightLighting.enabled = false;
                    }
                    dayLighting.intensity = minIntensityD + (maxIntensityD - minIntensityD) * ((sixHours - totalTime) / sixHours);
                    dayLighting.transform.localRotation = Quaternion.Euler(new Vector3((totalTime / sixHours) * spanLightAngle
                            + additionalLightAngle, lightingDefaultRotation.y, lightingDefaultRotation.z));
                } else if (m_cHour < 18) {
                    if (!nightLighting.enabled) {
                        nightLighting.enabled = true;
                    }
                    dayLighting.intensity = minIntensityD + (maxIntensityD - minIntensityD) * ((sixHours - totalTime) / sixHours);
                    dayLighting.transform.localRotation = Quaternion.Euler(new Vector3((totalTime / sixHours) * spanLightAngle
                        + additionalLightAngle, lightingDefaultRotation.y, lightingDefaultRotation.z));

                    nightLighting.intensity = minIntensityN * ((totalTime - 300) / 60f);
                    nightLighting.transform.localRotation = Quaternion.Euler(new Vector3(minLightAngle * ((totalTime - 300) / 60f),
                            lightingDefaultRotation.y, lightingDefaultRotation.z));
                } else if (m_cHour < 19) {
                    dayLighting.intensity = minIntensityD * ((60 - totalTime) / 60f);
                    dayLighting.transform.localRotation = Quaternion.Euler(new Vector3(minLightAngle * (totalTime / 60f) + spanLightAngle
                        + additionalLightAngle, lightingDefaultRotation.y, lightingDefaultRotation.z));

                    nightLighting.intensity = minIntensityN + (maxIntensityN - minIntensityN) * (totalTime / sixHours);
                    nightLighting.transform.localRotation = Quaternion.Euler(new Vector3((totalTime / sixHours) * spanLightAngle + minLightAngle,
                        lightingDefaultRotation.y, lightingDefaultRotation.z));
                } else {
                    if (dayLighting.enabled) {
                        dayLighting.enabled = false;
                        nightLighting.enabled = true;
                    }
                    nightLighting.intensity = minIntensityN + (maxIntensityN - minIntensityN) * (totalTime / sixHours);
                    nightLighting.transform.localRotation = Quaternion.Euler(new Vector3((totalTime / sixHours) * spanLightAngle + minLightAngle,
                            lightingDefaultRotation.y, lightingDefaultRotation.z));
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
