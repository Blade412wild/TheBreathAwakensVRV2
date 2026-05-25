using System;
using UnityEngine;

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
    [SerializeField] private float sensorValueForFullVisual = 3f;
    [SerializeField] private bool updateFromDeviceDataEveryFrame = true;

    private Vector3 targetPos;
    private Vector3 currentPos;

    private float startTimeInterpolation;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupGraph();

        if (simulator != null)
        {
            simulator.UpdateVisualsEvent += HandleUpdateVisualsEvent;
        }
        else
        {
            Debug.LogWarning("BreathingSensorSimulatorVisualFeedback: simulator reference is missing.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (visualPointTrans == null) return;

        if (updateFromDeviceDataEveryFrame)
        {
            UpdateTargetFromDeviceData();
        }

        currentPos = visualPointTrans.position;
        UpdatePos();
    }

    private void OnDisable()
    {
        if (simulator != null)
        {
            simulator.UpdateVisualsEvent -= HandleUpdateVisualsEvent;
        }
    }


    private void HandleUpdateVisualsEvent()
    {
        UpdateTargetFromDeviceData();
    }

    private void UpdateTargetFromDeviceData()
    {
        if (deviceData == null || visualPointTrans == null)
            return;

        float sensorValue = GetSensorValue();
        float graphData = ConvertToVisualData(sensorValue, (int)deviceData.BreathingState);
        targetPos = new Vector3(0, graphData, 0);
    }


    private void UpdatePos()
    {
        if (visualPointTrans == null) return;

        currentPos = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * speed);
        visualPointTrans.position = currentPos;
    }

    public float ConvertToVisualData(float sensorValue, int phase)
    {
        //Debug.Log("pahse : " + phase + " | breathingState : " + deviceData.BreathingState);
        if (phase == 0)
        {
            float sensorPeak = GetPositiveSensorPeak(highestSensorDataPeak);
            float mappedData = MapValueClamped(Mathf.Abs(sensorValue), 0, sensorPeak, 0, highestVisualPeak);
            //Debug.Log("inhaling GraphData : " + mappedData);
            return mappedData;
        }
        else if (phase == 1)
        {
            //Debug.Log("hjolding : " + 0);
            return visualZeroLine;

        }
        else if (phase == 2)
        {
            float sensorPeak = GetPositiveSensorPeak(lowestSensorDataPeak);
            float mappedData = MapValueClamped(Mathf.Abs(sensorValue), 0, sensorPeak, 0, lowestVisualPeak);
            //Debug.Log("exhaling GraphData : " + mappedData);

            return mappedData;
        }
        else
        {
            //Debug.Log("else : " + 0);

            return visualZeroLine;
        }
    }


    private void SetupGraph()
    {
        if (simulator == null)
        {
            Debug.LogWarning("BreathingSensorSimulatorVisualFeedback: simulator reference is missing, cannot setup graph.");
            return;
        }

        BreathingCycle cycle = simulator.GetCycle();
        if (cycle == null)
        {
            Debug.LogWarning("BreathingSensorSimulatorVisualFeedback: breathing cycle is missing.");
            return;
        }

        float highestPeakInhaling;
        float lowestPeakInhaling;
        GetCurvePeaks(cycle.InhalingSample.Curve, out highestPeakInhaling, out lowestPeakInhaling);

        float highestPeakExhaling;
        float lowestPeakExhaling;
        GetCurvePeaks(cycle.ExhalingSample.Curve, out highestPeakExhaling, out lowestPeakExhaling);

        highestPeakExhaling *= -1;
        lowestPeakExhaling *= -1;

        highestSensorDataPeak = highestPeakInhaling;
        lowestSensorDataPeak = highestPeakExhaling;
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
            return newMin;

        float t = (currentValue - oldMin) / (oldMax - oldMin);

        t = Mathf.Clamp01(t);

        return newMin + t * (newMax - newMin);
    }

    private float GetPositiveSensorPeak(float configuredPeak)
    {
        float peak = Mathf.Abs(configuredPeak);
        if (peak > Mathf.Epsilon)
            return peak;

        return Mathf.Max(Mathf.Epsilon, sensorValueForFullVisual);
    }

    private float GetSensorValue()
    {
        if (deviceData == null)
            return 0f;

        if (!Mathf.Approximately(deviceData.AirVelocity, 0f))
            return deviceData.AirVelocity;

        return deviceData.inExhaleSpeed;
    }
}
