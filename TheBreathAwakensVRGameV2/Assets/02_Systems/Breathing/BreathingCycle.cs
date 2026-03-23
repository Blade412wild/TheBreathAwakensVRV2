using UnityEngine;

[CreateAssetMenu(fileName = "BreathingCycle", menuName = "Scriptable Objects/BreathingCycle")]
public class BreathingCycle : ScriptableObject
{
    public BreathingSample InhalingSample;
    public float HoldingInBreathTime;
    public BreathingSample ExhalingSample;
}
