using NUnit.Framework;
using System;
using System.Collections.Generic;

public class BreathingSystemManager
{
    public event Action<BreathingSampleClass> SampleAnalyzedEvent;
    public event Action BreathingStateChanged;

    private BreathingSampleCreator creator;
    private BreathingSampleAnalyzer analyzer;
    private BreathingSampleSaver saver;

    private BreathingSample2 TestSample;

    private BreathingState previousBreathingState;
    private BreathingState currentBreathingState;

    private BreathingDeviceData deviceData;
    private bool saveFlag;

    private bool isActive = false;

    public BreathingSystemManager(MessageFinishedReceived messageFinishedReceived, BreathingDeviceData data, int cyclesPerBreathingSample)
    {
        deviceData = data;

        creator = new BreathingSampleCreator(messageFinishedReceived, data, cyclesPerBreathingSample);
        analyzer = new BreathingSampleAnalyzer();
        saver = new BreathingSampleSaver();

        previousBreathingState = BreathingState.holdingBreath;
        currentBreathingState = BreathingState.holdingBreath;

    }


    public void OnUpdate()
    {
        if (!isActive) return;

        creator.OnUpdate();
        saver.OnUpdate();

        AnalyzeBreathingState();
    }

    public void OnDisable()
    {
        saver.OnDisable();
    }
    public void Activate()
    {
        isActive = true;
        creator.Activate();
        creator.FinishedCreatingSampleEvent += HandleFinishedCreatingSampleEvent;
        analyzer.SampleAnalyzedEvent += HandleSampleAnalyzed;
    }

    public void DeActivate()
    {
        isActive = false;
        creator.Deactivate();
        creator.FinishedCreatingSampleEvent -= HandleFinishedCreatingSampleEvent;
        analyzer.SampleAnalyzedEvent -= HandleSampleAnalyzed;
    }

    private void HandleFinishedCreatingSampleEvent(BreathingSampleClass sample)
    {
        analyzer.AnalyzeSample(sample);
    }

    private void HandleSampleAnalyzed(BreathingSampleClass sample)
    {
        SampleAnalyzedEvent?.Invoke(sample);
        //oxygonPredictionSystem.
        saver.SaveNewBreathSample(sample);
    }

    private void AnalyzeBreathingState()
    {
        currentBreathingState = deviceData.BreathingState;
        if (currentBreathingState != previousBreathingState)
        {
            previousBreathingState = currentBreathingState;
            BreathingStateChanged?.Invoke();
        }
    }

}





