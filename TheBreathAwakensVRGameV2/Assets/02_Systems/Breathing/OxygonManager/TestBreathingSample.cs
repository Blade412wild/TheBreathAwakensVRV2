using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class TestBreathingSample : MonoBehaviour
{
    [Header("Control")]
    [SerializeField] private bool useThis;

    [SerializeField]
    [Range(-15, 15)]
    private float Speed;

    [SerializeField] private int cyclesPerSample;

    [Header("Info")]
    [Space]
    [SerializeField]
    private float threshold;

    [SerializeField]
    private List<BreathingState> breathingStateHistory = new List<BreathingState>();

    [SerializeField]
    private BreathCycle currentBreathCycle = new BreathCycle { };


    [SerializeField]
    private float mappedSpeed;

    [Header("Refs")]
    [SerializeField] private BreathingSample2 testSample;
    [SerializeField] private BreathingSensorSimulator simulator;
    [SerializeField] private BreathingDeviceData deviceData;
    [SerializeField] private MessageFinishedReceived messageFinishedReceived;

    private BreathingSampleCreator creator;
    private BreathingSampleSaver saver;
    private BreathingSampleManager breathingSystemManager;

    private Timer timer = new Timer();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!useThis) return;

        breathingSystemManager = new BreathingSampleManager(messageFinishedReceived, deviceData, cyclesPerSample);
        breathingSystemManager.Activate();

        timer.SetTimer(0.125f, true);
        timer.OnTimerIsDone += SensorEvent;
        timer.StartTimer();
        Speed = 0.0f;
    }


    // Update is called once per frame
    void Update()
    {
        if (!useThis) return;

        timer.OnUpdate();
        breathingSystemManager.OnUpdate();
    }

    private void OnDisable()
    {
        breathingSystemManager.OnDisable();
    }

    private void RemapSpeed()
    {
        if (Speed >= threshold * -1 && Speed <= threshold)
        {
            mappedSpeed = 0;
        }
        else
        {
            mappedSpeed = Speed;
        }

        if (mappedSpeed < 0)
        {
            deviceData.BreathingState = BreathingState.exhaling;
            deviceData.inExhaleSpeed = mappedSpeed * -1;
        }
        else if (mappedSpeed == 0)
        {
            deviceData.BreathingState = BreathingState.holdingBreath;
            deviceData.inExhaleSpeed = mappedSpeed;

        }
        else
        {
            deviceData.BreathingState = BreathingState.inhaling;
            deviceData.inExhaleSpeed = mappedSpeed;
        }
    }

    private void SensorEvent()
    {
        RemapSpeed();
        messageFinishedReceived.OnDataReceived("true");
    }

    
}
