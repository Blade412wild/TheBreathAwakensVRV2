using System;
using System.Collections.Generic;

public class BreathingSampleAnalyzer
{
    public event Action<BreathingSampleClass> SampleAnalyzedEvent;

    private const int phasesAmount = 3;
    private float[] endPhasesTime = new float[phasesAmount];

    public BreathingCycle currentCycle { get; private set; }


    public BreathingCycle[] CurrentBreathingArray { get; private set; }

    private float totalDuration;
    private float avarageInhaleSpeed;

    private BreathingSample2 currentSample;
    private BreathingSampleCreator sampleCreator;

    private BreathingDeviceData BreathingDeviceData;


    public void AnalyzeSample(BreathingSampleClass sample)
    {
        List<SensorDataPoint> totalInhalePoints = new List<SensorDataPoint>();
        List<SensorDataPoint> totalexhalePoints = new List<SensorDataPoint>();

        CreateNewDatapointList(sample.breathingCycles, totalInhalePoints, totalexhalePoints);

        float peakInhaling = GetPeak(totalInhalePoints);
        float peakExhaling = GetPeak(totalexhalePoints);

        //float avarageSpeedInhaling = GetAvarageSpeed(totalInhalePoints, sample.TotalDuration);
        //float avarageSpeedExhaling = GetAvarageSpeed(totalexhalePoints, sample.TotalDuration);

        float avarageSpeedInhaling = CalculateAverageSpeed(totalInhalePoints);
        float avarageSpeedExhaling = CalculateAverageSpeed(totalexhalePoints);

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

    public  float CalculateAverageSpeed(List<SensorDataPoint> list)
    {
        if (list == null || list.Count < 2)
            throw new ArgumentException("At least two list points are required.");

        float totalDistance = 0f;

        for (int i = 1; i < list.Count; i++)
        {
            float dt = list[i].Time - list[i - 1].Time;

            if (dt < 0)
                throw new ArgumentException("Time values must be in ascending order.");

            // Trapezoidal integration
            totalDistance += (list[i - 1].Value + list[i].Value) * 0.5f * dt;
        }

        float totalTime = list[^1].Time - list[0].Time;

        return totalTime > 0 ? totalDistance / totalTime : 0f;
    }

}





