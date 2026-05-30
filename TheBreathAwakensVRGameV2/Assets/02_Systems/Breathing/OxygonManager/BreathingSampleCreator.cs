using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BreathingSampleCreator
{
    public event Action BreathingCycleSampleFinished;
    public event Action BreathingCycleArrayFinished;


    private BreathingSensorSimulator simulator;
    private BreathingDeviceData data;

    private In_ExhaleSpeedDataReceived speedDataReceived;
    private BreathingStateDataReceived breathingStateDataReceived;
    private MessageFinishedReceived messageFinishedReceived;

    public BreathCycle currentBreathCycle = new BreathCycle { };

    private SensorDataPoint currentSensorDataPoint = new SensorDataPoint { };
    private SensorDataPoint[] breathingCycleArray;

    private StopWatch stopWatch = new StopWatch();
    private bool CreatingSample = false;

    private int currentCycleSampleIndex;
    private int maxCycles;

    private bool active;

    public List<BreathingState> breathingStateHistory = new List<BreathingState>();
    private const int maxBreathingStateHistory = 3;
    private BreathingState currentBreathingState;
    private BreathingState previousBreathingState;


    private bool newCycleFlag = false;

    private BreathingSample2 TestSample;


    //private string BreathingSampleFolderPath = "Assets";

    public BreathingSampleCreator(MessageFinishedReceived messageFinishedReceived, BreathingDeviceData data, int cyclesPerBreathingSample, BreathingSample2 TestSample)
    {

        if (cyclesPerBreathingSample > 0)
        {
            this.messageFinishedReceived = messageFinishedReceived;
            this.data = data;
            maxCycles = cyclesPerBreathingSample;
            this.TestSample = TestSample;

        }
        else
        {
            Debug.LogWarning("maxCycles needs to be > 0");
        }


    }

    ~BreathingSampleCreator()
    {
        if (messageFinishedReceived != null)
            messageFinishedReceived.OnDataReceivedEvent -= OnSensorMeasurementEvent;
    }

    public void Activate()
    {
        active = true;

        if (messageFinishedReceived != null)
            messageFinishedReceived.OnDataReceivedEvent += OnSensorMeasurementEvent;
    }

    public void Deactivate()
    {
        active = false;
        if (messageFinishedReceived != null)
            messageFinishedReceived.OnDataReceivedEvent -= OnSensorMeasurementEvent;
    }

    public void OnUpdate()
    {
        if (!active) return;
        stopWatch.OnUpdate();
    }

    private void BeginCreatingNewCycle()
    {
        Debug.Log("started Breathing cycle");
        stopWatch.ResetWatch();
        stopWatch.Run();
        CreatingSample = true;

        currentBreathCycle = new BreathCycle {Points = new List<SensorDataPoint>() };
        CreateSensorDataPoint();
    }


    private void OnSensorMeasurementEvent()
    {
        Debug.Log("sensor data received");
        if (!CreatingSample) // waiting for first point to be inhaling
        {
            if (data.BreathingState == BreathingState.inhaling)
            {
                BeginCreatingNewCycle();
            }
            else
            {
                Debug.Log("-waiting");
                return;
            }

        }

        AddNewBreathingState(data.BreathingState);

        if (breathingStateHistory.Count > 1 && IsNewCycle(data.BreathingState))
        {
            Debug.Log("-new Cycle");
            BreathCyleFinished();
        }
        else
        {
            CreateSensorDataPoint();
            currentBreathCycle.Points.Add(currentSensorDataPoint);

        }

    }

    private void BreathCyleFinished()
    {
        currentBreathCycle.duration = stopWatch.currentTime;
        TestSample.breathingCycles.Add(currentBreathCycle);

        if (TestSample.breathingCycles.Count > maxCycles - 1) // targetCycles per sample reached
        {
            // SampleFinished
            Debug.Log("--Sample Finished");
            Deactivate();
        }
        else
        {
            BeginCreatingNewCycle();
        }
    }
    private void CreateSensorDataPoint()
    {
        //Debug.Log("--Create DataPoint");
        currentSensorDataPoint = new SensorDataPoint { };
        currentSensorDataPoint.State = data.BreathingState;
        currentSensorDataPoint.Value = data.inExhaleSpeed;
        currentSensorDataPoint.Time = stopWatch.currentTime;
        //Debug.Log("-- currentDataPoint " + currentSensorDataPoint);

    }


    private void AddNewBreathingState(BreathingState newState)
    {
        if (breathingStateHistory.Count == 0)
        {
            breathingStateHistory.Add(newState);
            return;
        }
         //TODO something GOEs wrong with historyt
        //BreathingState previousState = breathingStateHistory[breathingStateHistory.Count - 1];
        // if last breathingState in history is same as new state return
        if (breathingStateHistory[breathingStateHistory.Count - 1] == newState)
        {
            Debug.Log("same as previous");
            return;
        }
        else
        {
            Debug.Log("new State so Add");
            breathingStateHistory.Add(newState);
        }

        if (breathingStateHistory.Count >= maxBreathingStateHistory)
        {
            Debug.Log("remove first");
            breathingStateHistory.Remove(0);
        }

    }


    private bool IsNewCycle(BreathingState newState)
    {
        if (newState != BreathingState.inhaling) return false;

        // one the previous states needs to be exhaling, else there isn't a new cycle
        if (breathingStateHistory.Count == 2)
        {
            if (breathingStateHistory[0] == BreathingState.exhaling) return true;
            return false;
        }

        if (breathingStateHistory.Count == 3)
        {
            if (breathingStateHistory[0] == BreathingState.exhaling || breathingStateHistory[1] == BreathingState.exhaling) return true;
            return false;
        }

        return false;

    }


}


