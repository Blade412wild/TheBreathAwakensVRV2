using System;
using System.Security.Cryptography;
using UnityEngine;
using static BreathFMODDriver;
using static UnityEngine.InputSystem.HID.HID;

public class BreathingSensorSimulator : MonoBehaviour
{

    public event Action SensorMeasurementEvent;
    public event Action UpdateVisualsEvent;
    public event Action StartSimEvent;
    [Header("Controls")]
    [SerializeField] private bool usingSimulator;
    [SerializeField] private bool start;
    //[SerializeField] private bool pauze;
    [SerializeField] private bool reset;
    [SerializeField] private bool loop;

    [Space]
    [Range(-1, 1)]
    [SerializeField] private float manualSlider;

    [Header("Setup")]
    [SerializeField] private BreathingCycle cycle;
    [SerializeField] private BreathFMODDriver FMODdriver;
    [SerializeField] private int sensorReactionTime;
    [SerializeField] private BreathingDeviceData deviceData;

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

    private DateTime previousTime;
    private DateTime currentTime;
    private TimeSpan timeSpan;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupSimulator();
    }

    // Update is called once per frame
    void Update()
    {

        if (!usingSimulator) return;

        CheckInput();
        UpdateSimulatorTimer();

        if (mayUpdate)
        {
            UpdateInfo();
            UpdateBreathCycleTimer();
        }

    }

    private void OnDisable()
    {
        SensorMeasurementEvent -= HandleSensorMeasurementEvent;

    }

    public BreathingCycle GetCycle()
    {
        return cycle;
    }

    public int GetCurrentPhase()
    {
        return currentPhase;
    }

    public float GetCycleData(int currentPhase)
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
            return valueAtTime * -1;
        }
        return 0;

    }

    public float GetManualSliderData()
    {
        return manualSlider;
    }


    private void SetupSimulator()
    {
        currentTime = DateTime.Now;
        previousTime = currentTime;
        SensorMeasurementEvent += HandleSensorMeasurementEvent;

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
    private void HandleSensorMeasurementEvent()
    {
        if (usingSimulator) return;
        if (FMODdriver.dataOrigin == DataOrigin.ManualSlider)
        {
            if (manualSlider > 0)
            {
                UpdateTemporaryDataContainer(0, manualSlider);
            }
            else if (manualSlider < 0)
            {
                UpdateTemporaryDataContainer(2, manualSlider);

            }
            else
            {
                UpdateTemporaryDataContainer(1, manualSlider);

            }
        }
        else if (FMODdriver.dataOrigin == DataOrigin.BreathCycle)
        {
            int phase = GetCurrentPhase();
            float sensorData = GetCycleData(phase);
            UpdateTemporaryDataContainer(phase, sensorData);

        }

        UpdateVisualsEvent?.Invoke();

    }

    private void UpdateTemporaryDataContainer(int state, float data)
    {
        deviceData.BreathingState = (BreathingState)state;
        deviceData.AirVelocity = data;

    }

    private void UpdateSimulatorTimer()
    {
        currentTime = DateTime.Now;

        timeSpan = currentTime - previousTime;

        if (timeSpan.TotalMilliseconds >= sensorReactionTime)
        {
            SensorMeasurementEvent?.Invoke();
            previousTime = currentTime;
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


    private void UpdateBreathCycleTimer()
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
