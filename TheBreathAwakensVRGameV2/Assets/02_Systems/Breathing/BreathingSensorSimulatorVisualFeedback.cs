using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class BreathingSensorSimulatorVisualFeedback : MonoBehaviour
{
    public event Action ReadSensorDataEvent;

    [Header("References")]
    [SerializeField] private BreathingSensorSimulator simulator;
    [SerializeField] private BreathingDeviceData deviceData;

    [Header("Visuals")]
    [SerializeField] private float highestVisualPeak;
    [SerializeField] private float lowestVisualPeak;
    [SerializeField] private float visualZeroLine;
    [SerializeField] private Transform visualPointTrans;
    [SerializeField] private float speed;

    [Header("Sensor")]
    [SerializeField] private float highestSensorDataPeak;
    [SerializeField] private float lowestSensorDataPeak;

    private Vector3 targetPos;
    private Vector3 currentPos;

    private float startTimeInterpolation;
    



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //simulator.UpdateVisualsEvent += HandleUpdateVisualsEvent;
        SetupGraph();

    }

    // Update is called once per frame
    void Update()
    {
        currentPos = visualPointTrans.position;
        HandleUpdateVisualsEvent();
    }

    private void OnDisable()
    {
        simulator.UpdateVisualsEvent -= HandleUpdateVisualsEvent;

    }


    private void HandleUpdateVisualsEvent()
    {
        float graphData = ConvertToVisualData(deviceData.AirVelocity, (int)deviceData.BreathingState);
        targetPos = new Vector3(0, graphData, 0); 
        visualPointTrans.position = targetPos;

    }



    private void UpdatePos()
    {
        currentPos = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * speed);
        visualPointTrans.position = currentPos;
    }

    public float ConvertToVisualData(float sensorValue, int phase)
    {
        if (phase == 0)
        {
            float mappedData = MapValueClamped(sensorValue, 0, highestSensorDataPeak, 0, highestVisualPeak);
            return mappedData;
        }
        else if (phase == 1)
        {
            return visualZeroLine;
        }
        else if (phase == 2)
        {
            float mappedData = MapValueClamped(sensorValue, 0, lowestSensorDataPeak, 0, lowestVisualPeak);
            return mappedData;
        }
        else
        {
            return visualZeroLine;
        }
    }
    

    private void SetupGraph()
    {
        BreathingCycle cycle = simulator.GetCycle();

        float highestPeakInhaling;
        float lowestPeakInhaling;
        GetCurvePeaks(cycle.InhalingSample.Curve, out highestPeakInhaling, out lowestPeakInhaling);

        float highestPeakExhaling;
        float lowestPeakExhaling;
        GetCurvePeaks(cycle.InhalingSample.Curve, out highestPeakExhaling, out lowestPeakExhaling);

        highestPeakExhaling *= -1;
        lowestPeakExhaling *= -1;


        highestSensorDataPeak = highestPeakInhaling;
        lowestSensorDataPeak  = highestPeakExhaling;

    }

    private void GetCurvePeaks(AnimationCurve curve, out float highestPeak, out float lowestPeak)
    {
        highestPeak = float.MinValue;
        lowestPeak = float.MaxValue;

        if (curve == null || curve.length == 0)
        {
            Debug.LogWarning("Curve is null or empty.");
            highestPeak = 0f;
            lowestPeak = 0f;
            return;
        }

        // Check all keyframes first
        foreach (var key in curve.keys)
        {
            if (key.value > highestPeak)
                highestPeak = key.value;

            if (key.value < lowestPeak)
                lowestPeak = key.value;
        }

        // Optional: sample between keys for more accurate peaks (curves can overshoot!)
        int samplesPerSegment = 20;

        for (int i = 0; i < curve.length - 1; i++)
        {
            float startTime = curve.keys[i].time;
            float endTime = curve.keys[i + 1].time;

            for (int s = 0; s <= samplesPerSegment; s++)
            {
                float t = Mathf.Lerp(startTime, endTime, s / (float)samplesPerSegment);
                float value = curve.Evaluate(t);

                if (value > highestPeak)
                    highestPeak = value;

                if (value < lowestPeak)
                    lowestPeak = value;
            }
        }
    }

    public static float MapValueClamped(float currentValue, float oldMin, float oldMax, float newMin, float newMax)
    {
        if (oldMax == oldMin)
            throw new ArgumentException("oldMax and oldMin cannot be the same.");

        float t = (currentValue - oldMin) / (oldMax - oldMin);

        t = Math.Clamp(t, 0f, 1f);

        return newMin + t * (newMax - newMin);
    }
}
