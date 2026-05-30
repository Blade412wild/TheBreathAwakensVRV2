using System;

[Serializable]
public struct SensorDataPoint
{
    public float Time;
    public float Value;
    public BreathingState State;
}


