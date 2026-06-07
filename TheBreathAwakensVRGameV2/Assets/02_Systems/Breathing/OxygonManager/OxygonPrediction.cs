using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class OxygonPrediction
{
    public event Action<TimeLeftStruct> OxygonPredictionMadeEvent;
    public TimeLeftStruct OxygonTimeLeft { get; private set; }

    private OxygonTank oxygonTank;
    private BreathingSample2 sample;

    private float previousOxygonVolume;
    private bool haveSetVolume;


    public OxygonPrediction(OxygonTank oxygonTank, BreathingSystem system, BreathingSample2 sample)
    {
        this.oxygonTank = oxygonTank;
        this.sample = sample;
        previousOxygonVolume = oxygonTank.MaxVolume;
        system.PredictBreathingTime += PredictOxygonTimeLeft;
        OxygonTimeLeft = new TimeLeftStruct { };

    }

    public void PredictOxygonTimeLeft()
    {
        float duration = sample.TotalDuration; // s

        float currentVolume = oxygonTank.AvailableOxygon; // L
        float oxygonUsed = previousOxygonVolume - currentVolume; // L

        float flowRateFromCycle = oxygonUsed / duration; // L/s

        if (!haveSetVolume)
        {
            oxygonTank.UpdateOxygonTankVolume(flowRateFromCycle);
            previousOxygonVolume = oxygonTank.MaxVolume;
            haveSetVolume = true;
        }
        else
        {
            previousOxygonVolume = currentVolume;
        }


        float timeLeft = oxygonTank.AvailableOxygon / flowRateFromCycle;

        SetOxygonTimeLeft(timeLeft);

        OxygonPredictionMadeEvent?.Invoke(OxygonTimeLeft);

    }

    private void SetOxygonTimeLeft(float timeLeft)
    {

        int totalSeconds = Mathf.FloorToInt(timeLeft);
        int seconds = totalSeconds % 60;

        int totalMinutes = totalSeconds / 60;
        int minutes = (totalSeconds / 60) % 60;

        int hours = totalSeconds / 3600;

        //Debug.Log("totalSeconds = " + totalSeconds + " s | seconds : " + seconds + " s" + " | totalMinutes : " + totalMinutes + " | minutes : " + minutes);


        OxygonTimeLeft = new TimeLeftStruct
        {
            TotalSeconds = totalSeconds,
            Seconds = seconds,

            TotalMinutes = totalMinutes,
            Minutes = minutes,
        };
    }

}





