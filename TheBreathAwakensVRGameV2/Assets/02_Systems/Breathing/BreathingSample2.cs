using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BreathingCycleArray2", menuName = "Scriptable Objects/BreathingCycleArray2")]
public class BreathingSample2 : ScriptableObject
{
    public List<BreathCycle> breathingCycles;// = new List<BreathCycle>();

    [Header("info")]
    public int TotalBreathingCycles;
    public float TotalDuration;

    //inhaling
    public float AvarageInhaleSpeed;
    public float PeakInhaleSpeed;

    // between inhale / exhale
    // public float 

    //exhaling
    public float AvarageExhaleSpeed;
    public float PeakExhaleSpeed;

    // between cycles
    public float AvarageTimeBetweenCycles;

}
