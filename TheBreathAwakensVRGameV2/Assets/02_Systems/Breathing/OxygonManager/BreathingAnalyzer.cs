using JetBrains.Annotations;
using System.Drawing.Text;
using UnityEngine;

public class BreathingAnalyzer
{
    private const int phasesAmount = 3;
    private float[] endPhasesTime = new float[phasesAmount];

    public BreathingCycle currentCycle { get; private set; }

    public BreathingCycle[] CurrentBreathingArray { get; private set; }

    private float totalDuration;
    private float avarageInhaleSpeed;




    public void Setup()
    {

    }



    public void SetBreathingArray(BreathingCycle[] breathingArray)
    {
        this.CurrentBreathingArray = breathingArray;
    }



    public void AnalyzeBreathingSample()
    {

        float totalTime = CalculateDurationOfBreathingSample(currentCycle);
        float avarageSpeed = CalculateAvarageSpeed(currentCycle.InhalingSample, totalTime);

    }

    public void AnalyzeBreathingSampleArray()
    {

    }




    private float CalculateDurationOfBreathingSample(BreathingCycle cycle)
    {
        float FullCycleDuration = 0;
        for (int i = 0; i < phasesAmount; i++)
        {
            if (i == 0)
            {
                AnimationCurve curve = cycle.InhalingSample.Curve;
                float time = curve.keys[curve.length - 1].time;

                endPhasesTime[i] = time;
                FullCycleDuration += time;
            }
            else if (i == 1)
            {
                float time = cycle.HoldingInBreathTime;

                endPhasesTime[i] = time;
                FullCycleDuration += time;
            }
            else if (i == 2)
            {
                AnimationCurve curve = cycle.ExhalingSample.Curve;
                float time = curve.keys[curve.length - 1].time;

                endPhasesTime[i] = time;
                FullCycleDuration += time;
            }
        }
        return FullCycleDuration;
    }

    private float CalculateAvarageSpeed(BreathingSample inhaling, float totalTime)
    {
        float totalspeed = 0;

        foreach (var key in inhaling.Curve.keys)
        {
            totalspeed += key.value;
        }

        return totalspeed / totalTime;
    }

}

public class OxygonPrediction
{
    private OxygonTank oxygonTank;
    private BreathingAnalyzer breathingAnalyzer;

    public OxygonPrediction(OxygonTank oxygonTank, BreathingAnalyzer breathingAnalyzer)
    {
        this.oxygonTank = oxygonTank;
        this.breathingAnalyzer = breathingAnalyzer;
    }

    public float PredictOxygonTimeLeft(BreathingCycle cycle)
    {



        return 0.0f;
    }

}





