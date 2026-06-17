using System;
using UnityEngine;
public class OxygonPrediction
{
    public event Action<TimeLeftStruct> OxygonPredictionMadeEvent;
    public event Action<TimeLeftStruct> FirstOxygonPredictionMadeEvent;
    public TimeLeftStruct OxygonTimeLeft { get; private set; }

    private OxygonTank oxygonTank;
    private BreathingSystem breathingSystem;

    private float previousOxygonVolume;
    private bool haveSetVolume;


    public OxygonPrediction(BreathingSystem breathingSystem, OxygonTank oxygonTank)
    {
        this.breathingSystem = breathingSystem;
        this.oxygonTank = oxygonTank;
        previousOxygonVolume = oxygonTank.MaxVolume;
        OxygonTimeLeft = new TimeLeftStruct { };

        breathingSystem.FirstSampleAnalysed += HandleFirstSampleAnalysedEvent;
        breathingSystem.SampleAnalysed += PredictOxygonTimeLeft;

        //breathingSystem.FirstSampleAnalysed += PredictOxygonTimeLeftOld;
        //breathingSystem.SampleAnalysed += PredictOxygonTimeLeftOld;

    }

    public void OnDisable()
    {
        breathingSystem.FirstSampleAnalysed -= HandleFirstSampleAnalysedEvent;
        breathingSystem.SampleAnalysed -= PredictOxygonTimeLeft;

        breathingSystem.FirstSampleAnalysed -= PredictOxygonTimeLeftOld;
        breathingSystem.SampleAnalysed -= PredictOxygonTimeLeftOld;
    }

    public void PredictOxygonTimeLeft(BreathingSampleClass sample)
    {
        float flowRateFromCycle = GetFlowRateFromCycle(sample); // L/s
        float timeLeft = oxygonTank.AvailableOxygon / flowRateFromCycle;

        previousOxygonVolume = oxygonTank.AvailableOxygon;

        Debug.Log("TimeLeft : " + timeLeft);
        SetOxygonTimeLeft(timeLeft);
        Debug.Log("OxygenTimeLeft : " + OxygonTimeLeft);

        OxygonPredictionMadeEvent?.Invoke(OxygonTimeLeft);

    }

    private void HandleFirstSampleAnalysedEvent(BreathingSampleClass sample)
    {
        float flowRateFromCycle = GetFlowRateFromCycle(sample); // L/s
        float timeLeft = oxygonTank.AvailableOxygon / flowRateFromCycle;

        oxygonTank.UpdateOxygonTankVolume(flowRateFromCycle);
        previousOxygonVolume = oxygonTank.MaxVolume;

        SetOxygonTimeLeft(oxygonTank.TargetTime);
        FirstOxygonPredictionMadeEvent?.Invoke(OxygonTimeLeft);

    }
    private float GetFlowRateFromCycle(BreathingSampleClass sample)
    {
        float duration = sample.TotalDuration; // s
        //Debug.Log("total duration prediction: " + duration);

        float oxygonUsed = previousOxygonVolume - oxygonTank.AvailableOxygon; // L

        float flowRateFromCycle = oxygonUsed / duration; // L/s
        return flowRateFromCycle;
    }

    public void PredictOxygonTimeLeftOld(BreathingSampleClass sample)
    {
        float duration = sample.TotalDuration; // s

        float currentVolume = oxygonTank.AvailableOxygon; // L
        float oxygonUsed = previousOxygonVolume - currentVolume; // L

        float flowRateFromCycle = oxygonUsed / duration; // L/s

        if (!haveSetVolume)
        {
            //Debug.Log("total duration prediction: " + duration);
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
        OxygonTimeLeft = TimeLeftConversions.CreateLeftStruct(timeLeft);
    }

}





