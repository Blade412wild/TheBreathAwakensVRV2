using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public static class TimeToDIgitalClockConverter
{
    public static string ConvertTime(float time)
    {
        //float time = levelManager.PlayerGameStopWatch.currentTime;
        int seconds = ((int)time % 60);
        int minutes = ((int)time / 60);
        string UITime = string.Format("{0:00}:{1:00}", minutes, seconds);
        return UITime;
    }
}
