using System;

public class BreathingSystemManager
{
    public event Action<BreathingSample2> SampleAnalyzedEvent;
    public event Action BreathingStateChanged;

    private BreathingSampleCreator creator;
    private BreathingSampleAnalyzer analyzer;
    private BreathingSampleSaver saver;

    private BreathingSample2 TestSample;

    private BreathingState previousBreathingState;
    private BreathingState currentBreathingState;

    private BreathingDeviceData deviceData;

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
        AnalyzeBreathingState();
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

    private void HandleFinishedCreatingSampleEvent(BreathingSample2 sample)
    {
        analyzer.AnalyzeSample(sample);
    }

    private void HandleSampleAnalyzed(BreathingSample2 sample)
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





