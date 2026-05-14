using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme.Primitives;
using static BreathFMODDriver;

public class BreathFMODDriver : MonoBehaviour
{
    public enum DataOrigin { BreathCycle, ManualSlider, SensorData }

    [Header("References")]
    [SerializeField] private BreathingSensorSimulator simulator;
    [SerializeField] private EventReference breathEvent;
    [SerializeField] private BreathingDeviceData dataContainer;


    [Header("FMOD Parameter Names")]
    [SerializeField] private string breathGainParam = "audioBreathGain";
    [SerializeField] private string breathQVolumeParam = "audioBreathQVolume";
    [SerializeField] private string breathFrequencyParam = "audioBreathFrequency";

    [Header("Tuning")]
    [SerializeField] private float inhaleFreqStart = 0.70f;
    [SerializeField] private float inhaleFreqEnd = 1.00f;

    [SerializeField] private float exhaleFreqStart = 0.30f;
    [SerializeField] private float exhaleFreqEnd = 0.00f;

    [SerializeField] private float stillFreqValue = 0.50f;

    [SerializeField] private float qVolumeMultiplier = 1.0f;
    [SerializeField] private float stillGainValue = 0.0f;
    [SerializeField] private float stillQValue = 0.0f;

    [Header("Smoothing")]
    [SerializeField] private float gainSmoothSpeed = 12.0f;
    [SerializeField] private float qSmoothSpeed = 12.0f;
    [SerializeField] private float frequencySmoothSpeed = 10.0f;

    [Header("Control")]
    public DataOrigin dataOrigin;


    private EventInstance breathInstance;

    private float currentBreathGain;
    private float currentBreathQVolume;
    private float currentBreathFrequency;

    private void Start()
    {
        if (simulator == null)
        {
            Debug.LogError("BreathFMODDriver: simulator reference is missing.");
            enabled = false;
            return;
        }

        breathInstance = RuntimeManager.CreateInstance(breathEvent);
        breathInstance.start();

        currentBreathGain = 0.0f;
        currentBreathQVolume = 0.0f;
        currentBreathFrequency = stillFreqValue;

        breathInstance.setParameterByName(breathGainParam, currentBreathGain);
        breathInstance.setParameterByName(breathQVolumeParam, currentBreathQVolume);
        breathInstance.setParameterByName(breathFrequencyParam, currentBreathFrequency);
    }

    private void Update()
    {
        var state = dataContainer.BreathingState;
        float sensorValue = GetSensorData();
        float intensity = Mathf.Clamp01(Mathf.Abs(sensorValue));

        float targetBreathGain = 0.0f;
        float targetBreathQVolume = 0.0f;
        float targetBreathFrequency = stillFreqValue;

        if (state == BreathingState.inhaling) // inhale
        {
            targetBreathGain = intensity;
            targetBreathQVolume = Mathf.Clamp01(intensity * qVolumeMultiplier);
            targetBreathFrequency = Mathf.Lerp(inhaleFreqStart, inhaleFreqEnd, intensity);
        }
        else if (state == BreathingState.holdingBreath) // still
        {
            targetBreathGain = stillGainValue;
            targetBreathQVolume = stillQValue;
            targetBreathFrequency = stillFreqValue;
        }
        else if (state == BreathingState.exhaling) // exhale
        {
            targetBreathGain = intensity;
            targetBreathQVolume = Mathf.Clamp01(intensity * qVolumeMultiplier);
            targetBreathFrequency = Mathf.Lerp(exhaleFreqStart, exhaleFreqEnd, intensity);
        }

        currentBreathGain = Mathf.Lerp(currentBreathGain, targetBreathGain, Time.deltaTime * gainSmoothSpeed);
        currentBreathQVolume = Mathf.Lerp(currentBreathQVolume, targetBreathQVolume, Time.deltaTime * qSmoothSpeed);
        currentBreathFrequency = Mathf.Lerp(currentBreathFrequency, targetBreathFrequency, Time.deltaTime * frequencySmoothSpeed);
        breathInstance.setParameterByName(breathGainParam, currentBreathGain);
        breathInstance.setParameterByName(breathQVolumeParam, currentBreathQVolume);
        breathInstance.setParameterByName(breathFrequencyParam, currentBreathFrequency);
    }

    private float GetSensorData()
    {
        float sensorData = dataContainer.inExhaleSpeed;
        float mappedValue = 0;

        if (dataOrigin == DataOrigin.SensorData)
        {
             mappedValue = BreathingSensorSimulatorVisualFeedback.MapValueClamped(sensorData, 0, 15, 0, 1);
        }
        else
        {
            mappedValue = sensorData;
        }

        return mappedValue;
    }



    private void OnDestroy()
    {
        breathInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        breathInstance.release();
    }
}