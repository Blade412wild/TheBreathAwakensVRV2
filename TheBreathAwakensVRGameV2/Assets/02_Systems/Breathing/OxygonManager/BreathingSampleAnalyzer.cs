using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class BreathingSampleAnalyzer
{
    public event Action<BreathingSample2> SampleAnalyzedEvent;

    private const int phasesAmount = 3;
    private float[] endPhasesTime = new float[phasesAmount];

    public BreathingCycle currentCycle { get; private set; }


    public BreathingCycle[] CurrentBreathingArray { get; private set; }

    private float totalDuration;
    private float avarageInhaleSpeed;

    private BreathingSample2 currentSample;
    private BreathingSampleCreator sampleCreator;

    private BreathingDeviceData BreathingDeviceData;


    public void AnalyzeSample(BreathingSample2 sample)
    {
        List<SensorDataPoint> totalInhalePoints = new List<SensorDataPoint>();
        List<SensorDataPoint> totalexhalePoints = new List<SensorDataPoint>();

        CreateNewDatapointList(sample.breathingCycles, totalInhalePoints, totalexhalePoints);

        float peakInhaling = GetPeak(totalInhalePoints);
        float peakExhaling = GetPeak(totalexhalePoints);

        float avarageSpeedInhaling = GetAvarageSpeed(totalInhalePoints, sample.TotalDuration);
        float avarageSpeedExhaling = GetAvarageSpeed(totalexhalePoints, sample.TotalDuration);

        sample.PeakInhaleSpeed = peakInhaling;
        sample.PeakExhaleSpeed = peakExhaling;

        sample.AvarageInhaleSpeed = avarageSpeedInhaling;
        sample.AvarageExhaleSpeed = avarageSpeedExhaling;

        SampleAnalyzedEvent?.Invoke(sample);
    }

    private void CreateNewDatapointList(List<BreathCycle> breathCycles, List<SensorDataPoint> totalInhalePoints, List<SensorDataPoint> totalexhalePoints)
    {
        foreach (BreathCycle cycle in breathCycles)
        {
            AddDataPointToList(totalInhalePoints, cycle.InhalePoints);
            AddDataPointToList(totalexhalePoints, cycle.ExhalePoints);
        }

    }
    private void AddDataPointToList(List<SensorDataPoint> listA, List<SensorDataPoint> listB)
    {
        foreach (SensorDataPoint datapoint in listB)
        {
            listA.Add(datapoint);
        }
    }

    private float GetPeak(List<SensorDataPoint> list)
    {
        float peak = 0;

        foreach (SensorDataPoint point in list)
        {
            if(point.Value > peak)
                peak = point.Value;
        }

        return peak;
    }

    private float GetAvarageSpeed(List<SensorDataPoint> list, float totalTime)
    {
        float totalSpeed = 0;

        foreach (SensorDataPoint point in list)
        {
            totalSpeed += point.Value;
        }

        return totalSpeed / totalTime;
    }

}





