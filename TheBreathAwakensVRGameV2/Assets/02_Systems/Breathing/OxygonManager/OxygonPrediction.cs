using System;

public class OxygonPrediction
{
    public event Action<TimeLeftStruct> OxygonPredictionMadeEvent;
    public TimeLeftStruct OxygonTimeLeft { get; private set; }

    private OxygonTank oxygonTank;
    private BreathingSystem breathingSystem;

    private float previousOxygonVolume;
    private bool haveSetVolume;


    public OxygonPrediction(BreathingSystem breathingSystem, OxygonTank oxygonTank)
    {
        this.breathingSystem = breathingSystem;
        this.oxygonTank = oxygonTank;
        previousOxygonVolume = oxygonTank.MaxVolume;
        OxygonTimeLeft = new TimeLeftStruct { };

        breathingSystem.FirstSampleAnalysed += HandleFirstSampleAnalysedEvent;
        breathingSystem.SampleAnalysed += PredictOxygonTimeLeft;

    }

    public void OnDisable()
    {
        breathingSystem.FirstSampleAnalysed -= HandleFirstSampleAnalysedEvent;
        breathingSystem.SampleAnalysed -= PredictOxygonTimeLeft;
    }

    public void PredictOxygonTimeLeft(BreathingSampleClass sample)
    {
        float flowRateFromCycle = GetFlowRateFromCycle(sample); // L/s
        float timeLeft = oxygonTank.AvailableOxygon / flowRateFromCycle;

        previousOxygonVolume = oxygonTank.AvailableOxygon;

        SetOxygonTimeLeft(timeLeft);
        OxygonPredictionMadeEvent?.Invoke(OxygonTimeLeft);

    }

    private float GetFlowRateFromCycle(BreathingSampleClass sample )
    {
        float duration = sample.TotalDuration; // s

        float oxygonUsed = previousOxygonVolume - oxygonTank.AvailableOxygon; // L

        float flowRateFromCycle = oxygonUsed / duration; // L/s
        return flowRateFromCycle;
    }

    private void HandleFirstSampleAnalysedEvent(BreathingSampleClass sample)
    {
        float flowRateFromCycle = GetFlowRateFromCycle(sample); // L/s
        float timeLeft = oxygonTank.AvailableOxygon / flowRateFromCycle;

        oxygonTank.UpdateOxygonTankVolume(flowRateFromCycle);
        previousOxygonVolume = oxygonTank.MaxVolume;

        SetOxygonTimeLeft(oxygonTank.TargetTime);
        OxygonPredictionMadeEvent?.Invoke(OxygonTimeLeft);

    }

    private void SetOxygonTimeLeft(float timeLeft)
    {
        OxygonTimeLeft = TimeLeftConversions.CreateLeftStruct(timeLeft);
    }

}





