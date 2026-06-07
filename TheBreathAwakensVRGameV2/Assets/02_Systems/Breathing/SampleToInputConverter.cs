using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SampleToInputConverter : MonoBehaviour
{
    public event Action SampleFinishedEvent;
    public event Action SensorMeasurementFinshedEvent;

    [Header("Control")]
    [SerializeField] private bool update;

    [Header("Refs")]
    [SerializeField] private BreathingSample2 sample;
    [SerializeField] private BreathingDeviceData dataContainer;
    [SerializeField] private MessageFinishedReceived messageFinishedReceived;

    private StopWatch stopWatch = new StopWatch();

    private float startTime;
    private float endTime;
    private float currentTime;

    private Keyframe targetKeyFrame;
    private int counter = 0;
    private bool messageReceived = true;

    private void Start()
    {
        startTime = sample.Curve.keys[0].time;
        endTime = sample.Curve.keys[sample.Curve.keys.Length - 1].time;

        ResetTargetKeyFrame();

        stopWatch.Setup(startTime);
        stopWatch.Run();
    }

    private void Update()
    {
        HandleStopWatch();
        UpdateDataContainer(currentTime, sample.Curve, dataContainer);

        if (messageReceived)
        {
            messageReceived = false;
            messageFinishedReceived.OnDataReceived("");
        }
    }

    private void SetNextTargetKeyFrame()
    {
        counter++;

        if (counter < sample.Curve.length)
        {
            //Debug.Log("counter : " + counter);
            targetKeyFrame = sample.Curve.keys[counter];
        }
        else
        {
            //Debug.Log("counter : " + counter);
            counter = sample.Curve.length - 1;
            targetKeyFrame = sample.Curve.keys[counter];
        }

    }

    private void HandleStopWatch()
    {
        stopWatch.OnUpdate();

        if (stopWatch.currentTime >= targetKeyFrame.time)
        {
            SetNextTargetKeyFrame();
            messageReceived = true;
        }

        if (stopWatch.currentTime >= endTime)
        {
            SampleFinishedEvent?.Invoke();
            stopWatch.ResetWatch();
            ResetTargetKeyFrame();
        }

        currentTime = stopWatch.currentTime;

    }
    private void ResetTargetKeyFrame()
    {
        counter = 0;
        targetKeyFrame = sample.Curve.keys[counter];
    }

    public void UpdateDataContainer(float currentTime, AnimationCurve curve, BreathingDeviceData dataContainer)
    {
        float rawValue = curve.Evaluate(currentTime);
        BreathingState state = GetBreathingState(rawValue);
        float convertedValue = GetTrueValue(state, rawValue);

        dataContainer.inExhaleSpeed = convertedValue;
        dataContainer.BreathingState = state;
    }

    private BreathingState GetBreathingState(float value)
    {
        if (value > 0)
        {
            return BreathingState.inhaling;
        }
        else if (value < 0)
        {
            return BreathingState.exhaling;
        }
        else
        {
            return BreathingState.holdingBreath;
        }
    }
    private float GetTrueValue(BreathingState state, float value)
    {
        if (state == BreathingState.exhaling)
        {
            return value * -1;
        }
        else
        {
            return value;

        }
    }




}
