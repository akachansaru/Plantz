using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public enum Seasons { Spring = 0, Summer = 1, Fall = 2, Winter = 3 }
public enum TimeUnits { Minute, Hour, Day, Season, Year, None }

public class GameTime : MonoBehaviour
{
    //[SerializeField] public const float SECOND = 0.1f;
    public const float MINUTE = 1f;
    public const float HOUR = 60 * MINUTE;
    public const float DAY = 24 * HOUR;
    public const float SEASON = 30 * DAY;
    public const float YEAR = 4 * SEASON;

    [SerializeField] private DeveloperOptions devOptions = new DeveloperOptions();
    [SerializeField] private bool twelveHourClock = true;

    [Header("Starting Date/Time")]
    [SerializeField] private int startingYear = 0;
    [Range(0, 3)] [SerializeField] private int startingSeason = 0;
    [Range(0, 29)] [SerializeField] private int startingDay = 0;
    [Range(0, 23)] [SerializeField] private int startingHour = 12;

    [Space]
    [SerializeField] private Text dateText = null;
    [SerializeField] private Text timeText = null;

    public DeveloperOptions DevOptions { get { return devOptions; } }
    public static bool GamePaused { get; private set; } = false;

    public static GameTime instance;

    public void Awake()
    {
        Assert.IsNotNull(dateText);
        Assert.IsNotNull(timeText);
        instance = this;
    }

    private void Start()
    {
        if (GlobalControl.Instance.savedValues.GameTime == 0f)
        {
            GlobalControl.Instance.savedValues.GameTime = (startingHour * HOUR) + 
                                                          (startingDay * DAY) + 
                                                          (startingSeason * SEASON) + 
                                                          (startingYear * YEAR);
            TicTime();
        }
    }

    void Update()
    {
        if (!GamePaused)
        {
            Time.timeScale = devOptions.devTimeScaleAdjuster;
            TicTime();
            FormatClock();
        }
    }

    public static void PauseGame()
    {
        Time.timeScale = 0;
        GamePaused = true;
    }

    public static void UnpauseGame()
    {
        Time.timeScale = 1;
        GamePaused = false;
    }

    //public static float YearToMinute(float years)
    //{
    //    // This will not work properly if the dev options are changed while running!!!
    //    //DeveloperOptions developerOptions = new DeveloperOptions();
    //    //return years * developerOptions.yearLength * developerOptions.seasonLength * developerOptions.dayLength * developerOptions.hourLength *
    //    //    developerOptions.minuteLength / developerOptions.minuteIncrement;
    //    return years * DeveloperOptions.YEAR * DeveloperOptions.SEASON * DeveloperOptions.DAY * DeveloperOptions.HOUR *
    //        DeveloperOptions.MINUTE / DeveloperOptions.INCREMENTS_PER_HOUR;
    //}

    //public static float SeasonToMinute(float seasons)
    //{
    //    // This will not work properly if the dev options are changed while running!!!
    //    //DeveloperOptions developerOptions = new DeveloperOptions();
    //    //return seasons * developerOptions.seasonLength * developerOptions.dayLength * developerOptions.hourLength * 
    //    //    developerOptions.minuteLength / developerOptions.minuteIncrement;
    //    return seasons * DeveloperOptions.SEASON * DeveloperOptions.DAY * DeveloperOptions.HOUR *
    //        DeveloperOptions.MINUTE / DeveloperOptions.INCREMENTS_PER_HOUR;
    //}

    //public static float DayToMinute(float days)
    //{
    //    // This will not work properly if the dev options are changed while running!!!
    //    //DeveloperOptions developerOptions = new DeveloperOptions();
    //    //return days * developerOptions.dayLength * developerOptions.hourLength *
    //    //    developerOptions.minuteLength / developerOptions.minuteIncrement;
    //    return days * DeveloperOptions.DAY * DeveloperOptions.HOUR *
    //        DeveloperOptions.MINUTE / DeveloperOptions.INCREMENTS_PER_HOUR;
    //}

    //public static float HourToMinute(float hours)
    //{
    //    // This will not work properly if the dev options are changed while running!!!
    //    //DeveloperOptions developerOptions = new DeveloperOptions();
    //    //return hours * developerOptions.hourLength * developerOptions.minuteLength / developerOptions.minuteIncrement;
    //    return hours * DeveloperOptions.HOUR * DeveloperOptions.MINUTE / DeveloperOptions.INCREMENTS_PER_HOUR;
    //}

    private void TicTime()
    {
        //if (Time.deltaTime > devOptions.minuteLength)
        //{
        //    Debug.LogWarning("Time.timeScale too high. Skipping time.");
        //}

        // Every minuteLength time will increase by minuteIncrement (currently: every real second (Time.DeltaTime) time increases by 15 game min)
        //if (GlobalControl.Instance.savedValues.GameTime >= devOptions.minuteLength)
        //{
        //    GlobalControl.Instance.savedValues.Minute += devOptions.minuteIncrement;
        //    GlobalControl.Instance.savedValues.GameTime = 0f;
        //}
        //else
        //{
        //    GlobalControl.Instance.savedValues.GameTime += Time.deltaTime;
        //}

        //if (GlobalControl.Instance.savedValues.Minute >= devOptions.hourLength)
        //{
        //    GlobalControl.Instance.savedValues.Hour++;
        //    GlobalControl.Instance.savedValues.Minute = 0;
        //}

        //if (GlobalControl.Instance.savedValues.Hour >= devOptions.dayLength)
        //{
        //    GlobalControl.Instance.savedValues.Day++;
        //    GlobalControl.Instance.savedValues.Hour = 0;
        //}

        //if (GlobalControl.Instance.savedValues.Day > devOptions.seasonLength)
        //{
        //    GlobalControl.Instance.savedValues.Season++;
        //    GlobalControl.Instance.savedValues.Day = 1;
        //}

        //if (GlobalControl.Instance.savedValues.Season >= devOptions.yearLength)
        //{
        //    GlobalControl.Instance.savedValues.Year++;
        //    GlobalControl.Instance.savedValues.Season = 0;
        //}

        GlobalControl.Instance.savedValues.GameTime += Time.deltaTime;

        //GlobalControl.Instance.savedValues.Second = (int)(GlobalControl.Instance.savedValues.GameTime % DeveloperOptions.MINUTE);
        GlobalControl.Instance.savedValues.Minute = (int)(GlobalControl.Instance.savedValues.GameTime % HOUR);
        GlobalControl.Instance.savedValues.Hour = (int)((GlobalControl.Instance.savedValues.GameTime / HOUR) % (DAY / HOUR));
        GlobalControl.Instance.savedValues.Day = (int)(1 + (GlobalControl.Instance.savedValues.GameTime / DAY) % (SEASON / DAY));
        GlobalControl.Instance.savedValues.Season = (int)((GlobalControl.Instance.savedValues.GameTime / SEASON) % (YEAR / SEASON));
        GlobalControl.Instance.savedValues.Year = (int)(1 + GlobalControl.Instance.savedValues.GameTime / YEAR);
    }

    public static float GetMinutesElapsed(float startGameTime, float endGameTime)
    {
        float deltaTime = endGameTime - startGameTime;
        return deltaTime / HOUR;
    }

    public static float ToMinutes(float time, TimeUnits unit)
    {
        float value;
        switch (unit)
        {
            case TimeUnits.Year:
                value = time * YEAR;
                break;
            case TimeUnits.Season:
                value = time * SEASON;
                break;
            case TimeUnits.Day:
                value = time * DAY;
                break;
            case TimeUnits.Hour:
                value = time * HOUR;
                break;
            default:
                value = 0;
                Debug.LogError("Invalid TimeUnit");
                break;
        }
        return value;
    }

    private void FormatClock()
    {
        if (twelveHourClock)
        {
            if (GlobalControl.Instance.savedValues.Hour == 0)
            {
                timeText.text = string.Format("{0:0}:{1:00} AM", 12, GlobalControl.Instance.savedValues.Minute);
            }
            else if (GlobalControl.Instance.savedValues.Hour > 0 && GlobalControl.Instance.savedValues.Hour < 12)
            {
                timeText.text = string.Format("{0:0}:{1:00} AM", GlobalControl.Instance.savedValues.Hour, GlobalControl.Instance.savedValues.Minute);
            }
            else if (GlobalControl.Instance.savedValues.Hour == 12)
            {
                timeText.text = string.Format("{0:0}:{1:00} PM", GlobalControl.Instance.savedValues.Hour, GlobalControl.Instance.savedValues.Minute);
            }
            else
            {
                timeText.text = string.Format("{0:0}:{1:00} PM", GlobalControl.Instance.savedValues.Hour - 12, GlobalControl.Instance.savedValues.Minute);
            }
        }
        else
        {
            timeText.text = string.Format("{0:0}:{1:00}", GlobalControl.Instance.savedValues.Hour, GlobalControl.Instance.savedValues.Minute);
        }
        dateText.text = string.Format("Year {0:0}, {1}, Day {2:0}", GlobalControl.Instance.savedValues.Year, 
            (Seasons)GlobalControl.Instance.savedValues.Season, GlobalControl.Instance.savedValues.Day);
    }

    // Adapted from Game Timer Tutorial for Unity3D displaying Hours,Minutes and Seconds by Nejo FTW on YouTube

    //private void Update()
    //{
    //    Time.timeScale = devOptions.devTimeScaleAdjuster;
    //    GlobalControl.Instance.savedValues.gameTime += Time.deltaTime * timeScaleAdjuster;

    //    GlobalControl.Instance.savedValues.minute = (int)(GlobalControl.Instance.savedValues.gameTime) % devOptions.hourLength;
    //    GlobalControl.Instance.savedValues.hour = (int)(GlobalControl.Instance.savedValues.gameTime / devOptions.hourLength) % devOptions.dayLength;
    //    GlobalControl.Instance.savedValues.day = (int)(1 + (GlobalControl.Instance.savedValues.gameTime / (devOptions.hourLength * devOptions.dayLength))) % devOptions.seasonLength;
    //    GlobalControl.Instance.savedValues.season = (int)(GlobalControl.Instance.savedValues.gameTime / (devOptions.hourLength * devOptions.dayLength * devOptions.seasonLength)) % devOptions.yearLength;
    //    GlobalControl.Instance.savedValues.year = (int)(1 + GlobalControl.Instance.savedValues.gameTime / (devOptions.hourLength * devOptions.dayLength * devOptions.seasonLength * devOptions.yearLength));

    //    if (twelveHourClock)
    //    {
    //        if (GlobalControl.Instance.savedValues.hour == 0)
    //        {
    //            timeText.text = string.Format("{0:0}:{1:00} AM", 12, GlobalControl.Instance.savedValues.minute);
    //        }
    //        else if (GlobalControl.Instance.savedValues.hour > 0 && GlobalControl.Instance.savedValues.hour < 12)
    //        {
    //            timeText.text = string.Format("{0:0}:{1:00} AM", GlobalControl.Instance.savedValues.hour, GlobalControl.Instance.savedValues.minute);
    //        }
    //        else if (GlobalControl.Instance.savedValues.hour == 12)
    //        {
    //            timeText.text = string.Format("{0:0}:{1:00} PM", GlobalControl.Instance.savedValues.hour, GlobalControl.Instance.savedValues.minute);
    //        }
    //        else
    //        {
    //            timeText.text = string.Format("{0:0}:{1:00} PM", GlobalControl.Instance.savedValues.hour - 12, GlobalControl.Instance.savedValues.minute);
    //        }
    //    }
    //    else
    //    {
    //        timeText.text = string.Format("{0:0}:{1:00}", GlobalControl.Instance.savedValues.hour, GlobalControl.Instance.savedValues.minute);
    //    }
    //    dateText.text = string.Format("Year {0:0}, {1}, Day {2:0}", GlobalControl.Instance.savedValues.year, (Seasons)GlobalControl.Instance.savedValues.season, GlobalControl.Instance.savedValues.day);
    //}

    [Serializable]
    public class DeveloperOptions
    {
        [Range(0, 100)] public float devTimeScaleAdjuster = 1; // For fast-forwarding the game. Only for developing purposes

        //[SerializeField] public int minuteIncrement = 15; // Needs to be a factor of hourLength
        //[SerializeField] public float minuteLength = 1f; // More like length between minuteIncrements (in seconds?)
        //[SerializeField] public int hourLength = 60;
        //[SerializeField] public int dayLength = 24;
        //[SerializeField] public int seasonLength = 30;
        //[SerializeField] public int yearLength = 4;

        public const int INCREMENTS_PER_HOUR = 15; // 4 makes time move in 15 minute increments
    }
}
