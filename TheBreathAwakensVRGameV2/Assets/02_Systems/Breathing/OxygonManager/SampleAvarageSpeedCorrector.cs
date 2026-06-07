using System;
using System.Collections.Generic;
using UnityEngine;

public class SampleAvarageSpeedCorrector : MonoBehaviour
{
    public bool Correct;

    public BreathingSample2 sample;

    private void Update()
    {
        if (Correct)
        {
            Correct = false;
            CorrectSample();
        }
    }

    public void CorrectSample()
    {
        List<SensorDataPoint> totalInhalePoints = new List<SensorDataPoint>();
        List<SensorDataPoint> totalexhalePoints = new List<SensorDataPoint>();

        CreateNewDatapointList(sample.breathingCycles, totalInhalePoints, totalexhalePoints);
        // float inhaleSpeed = CalculateAverageSpeed()

        float avarageSpeedInhaling = CalculateAverageSpeed(totalInhalePoints, sample.TotalDuration);
        float avarageSpeedExhaling = CalculateAverageSpeed(totalexhalePoints, sample.TotalDuration);

        sample.AvarageInhaleSpeed = avarageSpeedInhaling;
        sample.AvarageExhaleSpeed = avarageSpeedExhaling;
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

    private float GetAvarageSpeed(List<SensorDataPoint> list, float totalTime)
    {
        float totalSpeed = 0;

        foreach (SensorDataPoint point in list)
        {
            totalSpeed += point.Value;
        }

        return totalSpeed / totalTime;
    }


    public float CalculateAverageSpeed(List<SensorDataPoint> list, float totalTime)
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

        //float totalTime = list[^1].Time - list[0].Time;

        return totalTime > 0 ? totalDistance / totalTime : 0f;
    }

}




