using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public static class TimeLeftConversions
{
    public static string ConvertTimeToDigitalClock(TimeLeftStruct time)
    {

        string UITime = string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);
        return UITime;
    }

    public static TimeLeftStruct CreateLeftStruct(float timeSeconds)
    {

        int totalSeconds = Mathf.FloorToInt(timeSeconds);
        int seconds = totalSeconds % 60;

        int totalMinutes = totalSeconds / 60;
        int minutes = (totalSeconds / 60) % 60;

        int hours = totalSeconds / 3600;


        TimeLeftStruct timeLeftStruct = new TimeLeftStruct
        {
            TotalSeconds = totalSeconds,
            Seconds = seconds,

            TotalMinutes = totalMinutes,
            Minutes = minutes,
        };

        return timeLeftStruct;


    }
}
