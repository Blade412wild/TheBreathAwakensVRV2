using System;
using System.Drawing.Text;
using UnityEngine;
using static UnityEngine.PlayerLoop.PreUpdate;

public class BreathingSystem : MonoBehaviour
{
    public event Action CreateFirstSampleEvent;
    public event Action StopCreateFirstSampleEvent;

    public event Action StartSampling;
    public event Action StopSampling;

    public event Action<BreathingSampleClass> FirstSampleAnalysed;
    public event Action<BreathingSampleClass> SampleAnalysed;

    public event Action PredictBreathingTime;
    public event Action<TimeLeftStruct> OxygonPredictionDoneEvent;

    [Header("Controls")]
    [SerializeField] private bool mayUpdate;
    [SerializeField] private int maxCyclesPerSample;
    [SerializeField] private bool useSampleCreator;
    [SerializeField] private bool createFirstSample;
    [SerializeField] private bool stopCreatinngFirstSample;

    [Header("References")]
    [SerializeField] private OxygonTank oxygonTank;
    [SerializeField] private BreathingDeviceData dataContainer;
    [SerializeField] private MessageFinishedReceived messageFinishedReceived;
    [SerializeField] private BreathingSample2 sample;
    [SerializeField] private SampleToInputConverter sampleToInputConverter;
    [SerializeField] private BreathingDeviceData deviceData;


    private OxygonPrediction oxygonPrediction;
    private DateTime previousOxgyonUsedDateTime;
    private float previousSpeed = 0;
    private BreathingSampleManager breathingSystemManager;

    private DateTime startFirstSampleDateTime = DateTime.MinValue;

    public void OnStart()
    {
        oxygonTank.Setup();
        oxygonPrediction = new OxygonPrediction(this, oxygonTank);

        if (useSampleCreator)
        {
            breathingSystemManager = new BreathingSampleManager(messageFinishedReceived, deviceData, maxCyclesPerSample, this);
            breathingSystemManager.Activate();

            breathingSystemManager.SampleAnalysed += (x) => SampleAnalysed?.Invoke(x);
            breathingSystemManager.FirstSampleAnalysed += (x) => FirstSampleAnalysed.Invoke(x);

            oxygonPrediction.FirstOxygonPredictionMadeEvent += (x) => StartSampling?.Invoke();
            StartSampling += HandleStartSamplingEvent;
        }


        CreateFirstSampleEvent += HandleCreateFirstSampleEvent;
        StopCreateFirstSampleEvent += HandleStopCreateFirstSampleEvent;

        oxygonPrediction.OxygonPredictionMadeEvent += (x) => OxygonPredictionDoneEvent?.Invoke(x);
        oxygonPrediction.OxygonPredictionMadeEvent += Test;
        OxygonPredictionDoneEvent += Test;
    }

    private void Test(TimeLeftStruct _struct)
    {
        Debug.Log("event struct : " + _struct);
    }

    public void OnUpdate()
    {
        if (!mayUpdate) return;

        if (createFirstSample)
        {
            createFirstSample = false;
            CreateFirstSampleEvent?.Invoke();
        }

        if (stopCreatinngFirstSample)
        {
            stopCreatinngFirstSample = false;
            StopCreateFirstSampleEvent?.Invoke();
        }

        if (breathingSystemManager != null)
        {
            breathingSystemManager.OnUpdate();
        }
    }

    public void OnDeactivate()
    {
        if (useSampleCreator)
        {
            breathingSystemManager.SampleAnalysed -= (x) => SampleAnalysed?.Invoke(x);
            breathingSystemManager.FirstSampleAnalysed -= (x) => FirstSampleAnalysed.Invoke(x);
            OxygonPredictionDoneEvent -= (x) => StartSampling?.Invoke();


            breathingSystemManager.OnDisable();
        }

        messageFinishedReceived.OnDataReceivedEvent -= HandleSensorDataReceived;
        sampleToInputConverter.SampleFinishedEvent -= () => PredictBreathingTime?.Invoke();
        oxygonPrediction.OxygonPredictionMadeEvent -= (x) => OxygonPredictionDoneEvent?.Invoke(x);


    }

    private void HandleCreateFirstSampleEvent()
    {
        messageFinishedReceived.OnDataReceivedEvent += HandleSensorDataReceived;


    }

    private void HandleStopCreateFirstSampleEvent()
    {
        //TimeSpan timeSpan = DateTime.Now - startFirstSampleDateTime;
        //Debug.Log("total duration TimeSpan: " + timeSpan.TotalSeconds);
        messageFinishedReceived.OnDataReceivedEvent -= HandleSensorDataReceived;

    }

    private void HandleStartSamplingEvent()
    {
        Debug.Log("start Sampling");
        messageFinishedReceived.OnDataReceivedEvent += HandleSensorDataReceived;

    }

    private void HandleStopSamplingEvent()
    {
        messageFinishedReceived.OnDataReceivedEvent -= HandleSensorDataReceived;

    }
    private void HandleSensorDataReceived()
    {
        if (dataContainer.BreathingState != BreathingState.inhaling) return;
        //UseOxygon();
        UseOxygon2();
    }

    private void HandleOxygonPredictionMade(TimeLeftStruct timeLeft)
    {

    }

    private void UseOxygon()
    {
        float secondsBetweenMeasurement = GetTotalMillisecondsBetweenLastSensorMeasurement() / 1000.0f;
        float currentSpeed = dataContainer.inExhaleSpeed;
        float averageSpeed = GetAverageSpeed(currentSpeed, previousSpeed, secondsBetweenMeasurement);
        //Debug.Log("avarageSpeed : " + averageSpeed / 1000 + " ms | time passed : " + secondsBetweenMeasurement);

        oxygonTank.UseOxygonTank(averageSpeed, secondsBetweenMeasurement);
        previousSpeed = currentSpeed;
    }

    private void UseOxygon2()
    {
        float currentSpeed = dataContainer.inExhaleSpeed;
        oxygonTank.UseOxygonTank(currentSpeed, 0.125f);
    }

    private int GetTotalMillisecondsBetweenLastSensorMeasurement()
    {
        DateTime currentMoment = DateTime.Now;

        if (previousOxgyonUsedDateTime == null)
        {
            Debug.Log("previous is null");
            previousOxgyonUsedDateTime = currentMoment;
            return 0;
        }

        TimeSpan timeSpan = currentMoment - previousOxgyonUsedDateTime;

        if (timeSpan.TotalMilliseconds > 200) // als er geen tweede inhalingmeasurement is geweest, dat betekent deze meting de eerste is de breathinginhalingPhase
        {
            Debug.Log("previous time is above 200ms");
            previousOxgyonUsedDateTime = currentMoment;

            return 0;
        }

        previousOxgyonUsedDateTime = currentMoment;

        return (int)timeSpan.TotalMilliseconds;


    }


    private float GetAverageSpeed(float currentSpeed, float previousSpeed, float time)
    {
        float totalSpeed = currentSpeed + previousSpeed;
        return totalSpeed / time;
    }



}





