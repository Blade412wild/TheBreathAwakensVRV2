using System;
using System.Collections.Generic;

[Serializable]
public struct BreathCycle
{
    public List<SensorDataPoint> Points;
    public List<SensorDataPoint> InhalePoints;
    public List<SensorDataPoint> ExhalePoints;
    public List<SensorDataPoint> HoldingBreathPoints;
    public float duration;
}


