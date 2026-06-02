using JetBrains.Annotations;
using NUnit.Framework;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using UnityEngine;

public class BreathingAnalyzer
{
    private const int phasesAmount = 3;
    private float[] endPhasesTime = new float[phasesAmount];

    public BreathingCycle currentCycle { get; private set; }


    public BreathingCycle[] CurrentBreathingArray { get; private set; }

    private float totalDuration;
    private float avarageInhaleSpeed;

    private BreathingSample2 currentSample;
    private BreathingSampleCreator sampleCreator;

    private BreathingDeviceData BreathingDeviceData;



    public void Setup(MessageFinishedReceived messageFinishedReceived, BreathingDeviceData data, int cyclesPerBreathingSample, BreathingSample2 TestSample)
    {
        sampleCreator = new BreathingSampleCreator(messageFinishedReceived, data, cyclesPerBreathingSample, TestSample);


    }



    public void SetBreathingArray(BreathingSample2 currentSample)
    {
        this.currentSample = currentSample;
    }



    public void AnalyzeBreathingSample()
    {
        float avarageInhaleSpeed = CalculateAvarageSpeed(BreathingState.inhaling);
        float avarageExhaleSpeed = CalculateAvarageSpeed(BreathingState.exhaling);

        currentSample.AvarageInhaleSpeed = avarageInhaleSpeed;
        currentSample.AvarageExhaleSpeed = avarageExhaleSpeed;
    }

    private float CalculateAvarageSpeed(BreathingState state)
    {

        float totalspeed = 0;

        foreach (BreathCycle cycle in currentSample.breathingCycles)
        {
            totalspeed += GetSpeedFromCycle(cycle, state);
        }


        return totalspeed / currentSample.TotalDuration;
    }

    private float GetSpeedFromCycle(BreathCycle cycle, BreathingState state)
    {
        float speed = 0;

        if (state == BreathingState.inhaling)
        {
            foreach (SensorDataPoint sensorDataPoint in cycle.InhalePoints)
            {
                speed += sensorDataPoint.Value;
            }

        }

        if (state == BreathingState.exhaling)
        {
            foreach (SensorDataPoint sensorDataPoint in cycle.ExhalePoints)
            {
                speed += sensorDataPoint.Value;
            }

        }
        return speed;
    }

}


public class OxygonPrediction
{
    private OxygonTank oxygonTank;
    private BreathingAnalyzer breathingAnalyzer;

    public OxygonPrediction(OxygonTank oxygonTank, BreathingAnalyzer breathingAnalyzer)
    {
        this.oxygonTank = oxygonTank;
        this.breathingAnalyzer = breathingAnalyzer;
    }

    public float PredictOxygonTimeLeft(BreathingCycle cycle)
    {



        return 0.0f;
    }

}





