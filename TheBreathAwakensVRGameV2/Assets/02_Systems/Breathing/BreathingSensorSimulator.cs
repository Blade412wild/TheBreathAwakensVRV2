using System;
using UnityEngine;

public class BreathingSensorSimulator : MonoBehaviour
{
    public event Action StartSimEvent;
    [Header("Controls")]
    [SerializeField] private bool start;
    //[SerializeField] private bool pauze;
    [SerializeField] private bool reset;
    [SerializeField] private bool loop;

    [Header("Setup")]
    [SerializeField] private BreathingCycle cycle;

    [Header("Info")]
    public float FullCycleDuration;

    [Space]
    public float OverallTime;
    public float PhaseTime;
    public int CurrentPhase;

    private const int phasesAmount = 3;
    private float[] endPhasesTime = new float[phasesAmount];

    private float currentPhaseTime;
    private float overallTime;

    private int currentPhase = 0;

    private bool mayUpdate = false;
    private bool isPaused;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupSimulator();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();

        if (mayUpdate)
        {
            UpdateInfo();
            UpdateTimer();

        }

    }

    public BreathingCycle GetCycle()
    {
        return cycle;
    }

    public int GetCurrentPhase()
    {
        return currentPhase;
    }

    public float GetData(int currentPhase)
    {
        if (currentPhase == 0)
        {
            AnimationCurve curve = cycle.InhalingSample.Curve;
            float valueAtTime = curve.Evaluate(currentPhaseTime);
            return valueAtTime;
        }
        else if (currentPhase == 1)
        {
            return 0;
        }
        else if (currentPhase == 2)
        {
            AnimationCurve curve = cycle.ExhalingSample.Curve;
            float valueAtTime = curve.Evaluate(currentPhaseTime);
            return valueAtTime*-1;
        }
        return 0;

    }


    private void SetupSimulator()
    {
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
    }

    private void CheckInput()
    {
        if (start)
        {
            start = false;
            mayUpdate = true;
            StartSimEvent?.Invoke();

        }


        if (reset)
        {
            reset = false;
            currentPhaseTime = 0.0f;
            overallTime = 0.0f;
            currentPhase = 0;
        }

        //if (pauze)
        //{
        //    mayUpdate = false;
        //}

    }


    private void UpdateTimer()
    {
        // Debug.Log("overallTime : " + overallTime);
        if (currentPhaseTime >= endPhasesTime[currentPhase])
        {
            if (currentPhase == phasesAmount - 1)
            {
                if (loop)
                {
                    currentPhaseTime = 0.0f;
                    overallTime = 0.0f;
                    currentPhase = 0;
                    StartSimEvent?.Invoke();
                    return;
                }
                else
                {
                    mayUpdate = false;
                    currentPhaseTime = 0.0f;
                    overallTime = 0.0f;
                    currentPhase = 0;
                    return;
                }
            }
            else
            {
                currentPhaseTime = 0.0f;
                currentPhase++;
            }


        }
        else
        {
            currentPhaseTime += Time.deltaTime;
            overallTime += Time.deltaTime;
        }




    }
    private void UpdateInfo()
    {
        OverallTime = overallTime;
        PhaseTime = currentPhaseTime;
        CurrentPhase = currentPhase;
    }
}
