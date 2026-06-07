using System;
using System.Drawing.Text;
using UnityEngine;

public class BreathingSystem : MonoBehaviour
{
    public event Action PredictBreathingTime;
    public event Action<TimeLeftStruct> OxygonPredictionDoneEvent;

    [SerializeField] private OxygonTank oxygonTank;
    [SerializeField] private BreathingDeviceData dataContainer;
    [SerializeField] private MessageFinishedReceived messageFinishedReceived;
    [SerializeField]  private BreathingSample2 sample;
    [SerializeField] private SampleToInputConverter sampleToInputConverter;

    private OxygonPrediction oxygonPrediction;
    private DateTime previousOxgyonUsedDateTime;
    private float previousSpeed = 0;

    public void OnStart()
    {
        oxygonTank.Setup();
        oxygonPrediction = new OxygonPrediction(oxygonTank, this, sample);

        messageFinishedReceived.OnDataReceivedEvent += HandleSensorDataReceived;
        sampleToInputConverter.SampleFinishedEvent += () => PredictBreathingTime?.Invoke();
        oxygonPrediction.OxygonPredictionMadeEvent += (x) => OxygonPredictionDoneEvent?.Invoke(x);
    }

    public void OnUpdate()
    {
        //oxygonTank.UseOxygonTank(5, Time.deltaTime);
        //oxygonTank.UpdatePercentage();
    }

    public void OnDeactivate()
    {
        messageFinishedReceived.OnDataReceivedEvent -= HandleSensorDataReceived;
        sampleToInputConverter.SampleFinishedEvent -= () => PredictBreathingTime?.Invoke();
        oxygonPrediction.OxygonPredictionMadeEvent -= (x) => OxygonPredictionDoneEvent?.Invoke(x);

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





