using UnityEngine;

[CreateAssetMenu(fileName = "BreathingCycle", menuName = "Scriptable Objects/BreathingCycle")]
public class BreathingCycle : ScriptableObject
{
    [Header("references")]
    public BreathingSample InhalingSample;
    public float HoldingInBreathTime;
    public BreathingSample ExhalingSample;

    [Header("info")]
    public float TotalDuration;
}
