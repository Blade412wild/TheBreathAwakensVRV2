using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class BreathingSampleCreator
{
    public event Action<BreathingSampleClass> FinishedCreatingSampleEvent;


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

    private List<BreathingSampleClass> breathingSampleStruct = new List<BreathingSampleClass>();

    private BreathingSample2 currentSample;
    private BreathingSampleClass currentBreathingSampleClass;


    //private string BreathingSampleFolderPath = "Assets";

    public BreathingSampleCreator(MessageFinishedReceived messageFinishedReceived, BreathingDeviceData data, int cyclesPerBreathingSample)
    {

        if (cyclesPerBreathingSample > 0)
        {
            this.messageFinishedReceived = messageFinishedReceived;
            this.data = data;
            maxCycles = cyclesPerBreathingSample;

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

    private void BeginCreatingNewSample()
    {
        currentSample = ScriptableObject.CreateInstance<BreathingSample2>();
        currentBreathingSampleClass = new BreathingSampleClass { breathingCycles = new List<BreathCycle>() };

        BeginCreatingNewCycle();
        CreatingSample = true;
    }

    private void BeginCreatingNewCycle()
    {
        Debug.Log("started Breathing cycle");
        stopWatch.ResetWatch();
        stopWatch.Run();

        CreateNewBreathCycleInstance();

        //breathingStateHistory.Clear();

        CreateSensorDataPoint();
    }
    private void CreateNewBreathCycleInstance()
    {
        currentBreathCycle = new BreathCycle
        {
            Points = new List<SensorDataPoint>(),
            InhalePoints = new List<SensorDataPoint>(),
            ExhalePoints = new List<SensorDataPoint>(),
            HoldingBreathPoints = new List<SensorDataPoint>(),
        };
    }


    private void OnSensorMeasurementEvent()
    {
        if (!CreatingSample) // waiting for first point to be inhaling
        {
            if (data.BreathingState == BreathingState.inhaling)
            {
                BeginCreatingNewSample();
            }
            else
            {
                return;
            }

        }

        AddNewBreathingState(data.BreathingState);

        if (breathingStateHistory.Count > 1 && IsNewCycle(data.BreathingState))
        {
            BreathCyleFinished();
        }
        else
        {
            CreateSensorDataPoint();
            AddSensorDataPointToCurrentBreathCycle();

        }

    }

    private void AddSensorDataPointToCurrentBreathCycle()
    {
        currentBreathCycle.Points.Add(currentSensorDataPoint);

        switch (currentSensorDataPoint.State)
        {
            case BreathingState.inhaling: currentBreathCycle.InhalePoints.Add(currentSensorDataPoint); break;
            case BreathingState.exhaling: currentBreathCycle.ExhalePoints.Add(currentSensorDataPoint); break;
            case BreathingState.holdingBreath: currentBreathCycle.HoldingBreathPoints.Add(currentSensorDataPoint); break;

        }

    }

    private void BreathCyleFinished()
    {
        //Debug.Log("-Cycle finished");

        currentBreathCycle.duration = stopWatch.currentTime;

        currentSample.breathingCycles.Add(currentBreathCycle);
        currentBreathingSampleClass.breathingCycles.Add(currentBreathCycle);


        Debug.Log("currentSample : " + currentSample.breathingCycles.Count + " | currentBreathingSampleClass : " + currentBreathingSampleClass.breathingCycles.Count);

        if (currentSample.breathingCycles.Count > maxCycles - 1 || currentBreathingSampleClass.breathingCycles.Count > maxCycles - 1) // targetCycles per sample reached
        {
            // SampleFinished
            Debug.Log("--Sample Finished");
            currentSample.Curve = BreathSampleArrayToAnimationCurveConverter.ConvertSampleToAnimationCurve(currentSample.breathingCycles);
            currentSample.TotalBreathingCycles = currentSample.breathingCycles.Count;
            currentSample.TotalDuration = currentBreathCycle.Points[currentBreathCycle.Points.Count - 1].Time; // get the last time value of the last 

            currentBreathingSampleClass.Curve = BreathSampleArrayToAnimationCurveConverter.ConvertSampleToAnimationCurve(currentBreathingSampleClass.breathingCycles);
            currentBreathingSampleClass.TotalBreathingCycles = currentBreathingSampleClass.breathingCycles.Count;
            currentBreathingSampleClass.TotalDuration = currentBreathCycle.Points[currentBreathCycle.Points.Count - 1].Time;

            breathingStateHistory.Clear();
            CreatingSample = false;

            FinishedCreatingSampleEvent?.Invoke(currentBreathingSampleClass);
        }
        else
        {
            StartNextCycle();
        }
    }

    private void StartNextCycle()
    {
        Debug.Log("next Cycle");
        breathingStateHistory.Clear();
        CreateNewBreathCycleInstance();


        //breathingStateHistory.Clear();

        CreateSensorDataPoint();
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
            //Debug.Log("same as previous");
            return;
        }
        else
        {
            //Debug.Log("new State so Add");
            breathingStateHistory.Add(newState);
        }

        if (breathingStateHistory.Count > maxBreathingStateHistory)
        {
            //Debug.Log("remove first");
            breathingStateHistory.RemoveAt(0);
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



