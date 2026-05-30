using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class TestBreathingSample : MonoBehaviour
{
    [Header("Control")]
    [SerializeField] private bool Activate;
    [SerializeField] private bool Deactivate;

    [SerializeField]
    [Range(-15, 15)]
    private float Speed;

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
    private Timer timer = new Timer();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        creator = new BreathingSampleCreator(messageFinishedReceived, deviceData, 1, testSample);
        timer.SetTimer(0.125f, true);
        timer.OnTimerIsDone += SensorEvent;
        timer.StartTimer();
        Speed = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        creator.OnUpdate();
        timer.OnUpdate();

        if (Activate)
        {
            Activate = false;
            creator.Activate();
        }

        if (Deactivate)
        {
            Deactivate = false;
            creator.Deactivate();
        }
    }

    private void RemapSpeed()
    {
        if(Speed >= threshold * -1 && Speed <= threshold)
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
        breathingStateHistory = creator.breathingStateHistory;
        currentBreathCycle = creator.currentBreathCycle;
        messageFinishedReceived.OnDataReceived("true");
    }
}
