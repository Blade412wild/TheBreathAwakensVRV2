using System;
using UnityEngine;

public class BreathingSampleManager
{
    public event Action<BreathingSampleClass> FirstSampleAnalysed;
    public event Action<BreathingSampleClass> SampleAnalysed;


    public event Action<BreathingSampleClass> SampleAnalyzedEvent;
    public event Action BreathingStateChanged;
    public BreathingSampleClass FirstSample { get; private set; }

    private BreathingSampleCreator creator;
    private BreathingSampleAnalyzer analyzer;
    private BreathingSampleSaver saver;


    private BreathingState previousBreathingState;
    private BreathingState currentBreathingState;

    private BreathingDeviceData deviceData;
    private bool saveFlag;
    private BreathingSystem breathingSystem;

    private bool isActive = false;

    public BreathingSampleManager(MessageFinishedReceived messageFinishedReceived, BreathingDeviceData data, int cyclesPerBreathingSample, BreathingSystem breathingSystem)
    {

        this.breathingSystem = breathingSystem;
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
        creator.FinishedCreatingSampleEvent += HandleFinishedCreatingSampleEvent;

        analyzer.SampleAnalyzedEvent += HandleSampleAnalyzed;

        breathingSystem.CreateFirstSampleEvent += HandleCreatingFirstSampleEvent;
        breathingSystem.StopCreateFirstSampleEvent += HandleStopCreatingSampleEvent;

        breathingSystem.StartSampling += creator.StartCreatingSamples;
        breathingSystem.StopSampling += creator.StopSampling;

    }

    public void DeActivate()
    {
        isActive = false;
        creator.FinishedCreatingSampleEvent -= HandleFinishedCreatingSampleEvent;

        analyzer.SampleAnalyzedEvent -= HandleSampleAnalyzed;

        breathingSystem.CreateFirstSampleEvent -= HandleCreatingFirstSampleEvent;
        breathingSystem.StopCreateFirstSampleEvent -= HandleStopCreatingSampleEvent;

        breathingSystem.StartSampling -= creator.StartCreatingSamples;
        breathingSystem.StartSampling -= creator.StopSampling;
    }

    private void HandleCreatingFirstSampleEvent()
    {
        creator.StartCreatingExampleSample();
    }

    private void HandleStopCreatingSampleEvent()
    {
        creator.StopCreatingExampleSample();
        //creator.StartCreatingSamples();
    }

    private void HandleFinishedCreatingSampleEvent(BreathingSampleClass sample)
    {
        analyzer.AnalyzeSample(sample);
    }

    private void HandleSampleAnalyzed(BreathingSampleClass sample)
    {
        if (FirstSample == null)
        {
            FirstSample = sample;
            FirstSampleAnalysed?.Invoke(sample);

            Debug.Log("fnished First Sample");
            return;
        }


        SampleAnalyzedEvent?.Invoke(sample);
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





