using System.Collections.Generic;
using UnityEngine;

public class BreathingSampleClass
{
    public string SampleName;
    public string Path;

    public List<BreathCycle> breathingCycles = new List<BreathCycle>();
    public AnimationCurve Curve;

    [Header("info")]
    public int TotalBreathingCycles;
    public float TotalDuration;

    //inhaling
    public float AvarageInhaleSpeed;
    public float PeakInhaleSpeed;
    //public float TotalInhaleDuration;

    // between inhale / exhale
    // public float 

    //exhaling
    public float AvarageExhaleSpeed;
    public float PeakExhaleSpeed;
    //public float TotalExhaleDuration;


    // between cycles
    public float AvarageTimeBetweenCycles;
}



