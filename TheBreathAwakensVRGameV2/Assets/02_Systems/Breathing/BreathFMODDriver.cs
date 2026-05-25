using FMOD.Studio;
using FMODUnity;
using UnityEngine;

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
    [SerializeField] private string breathStateParam = "BreathState";
    [SerializeField] private bool sendBreathStateParam;

    [Header("Frequency Mapping (Hz)")]
    [SerializeField] private float frequencyMin = 660f;      // Exhale: lower frequency
    [SerializeField] private float frequencyMax = 4070f;     // Inhale: higher frequency
    [SerializeField] private float frequencyStill = 1800f;   // Neutral/holding breath
    [SerializeField] private float exhaleFrequencyRest = 950f;

    [Header("Gain Mapping (dB)")]
    [SerializeField] private float gainMinDb = -32f;
    [SerializeField] private float gainMaxDb = -10f;
    [SerializeField] private float gainStillDb = -32f;
    [SerializeField] private float volumeIntensityPower = 1.6f;

    [Header("Q Volume Mapping (dB, Q=2.0)")]
    [SerializeField] private float qVolumeMinDb = 0f;
    [SerializeField] private float qVolumeMaxDb = 12.5f;
    [SerializeField] private float qVolumeStillDb = 0f;

    [Header("FMOD Parameter 0-10 Scaling")]
    // FMOD parameters are set to 0-10 range in Studio
    [SerializeField] private float fmodParamMin = 0f;
    [SerializeField] private float fmodParamMax = 10f;

    [Header("Smoothing")]
    [SerializeField] private float gainSmoothSpeed = 12.0f;
    [SerializeField] private float qSmoothSpeed = 12.0f;
    [SerializeField] private float frequencySmoothSpeed = 10.0f;
    [SerializeField] private float exhaleGainSmoothSpeed = 18.0f;
    [SerializeField] private float exhaleQSmoothSpeed = 18.0f;
    [SerializeField] private float exhaleFrequencySmoothSpeed = 22.0f;

    [Header("Control")]
    public DataOrigin dataOrigin = DataOrigin.SensorData;
    [SerializeField] private float sensorSpeedForFullIntensity = 6f;
    [SerializeField] private float breathStartThreshold = 0.001f;
    [SerializeField] private float breathStopThreshold = 0.0005f;
    [SerializeField] private float attackSpeed = 3.5f;
    [SerializeField] private float releaseSpeed = 2.0f;
    [SerializeField] private float exhaleReleaseSpeed = 4.5f;

    private EventInstance breathInstance;

    private float currentBreathGain;
    private float currentBreathQVolume;
    private float currentBreathFrequency;
    private float currentBreathState;
    private float outputEnvelope;
    private bool eventIsPlaying;
    private BreathingState lastActiveBreathingState = BreathingState.holdingBreath;

    private void Start()
    {
        if (dataContainer == null)
        {
            Debug.LogError("BreathFMODDriver: dataContainer reference is missing.");
            enabled = false;
            return;
        }

        if (dataOrigin != DataOrigin.SensorData && simulator == null)
        {
            Debug.LogError("BreathFMODDriver: simulator reference is missing for the selected data origin.");
            enabled = false;
            return;
        }

        // Initialize to neutral/holding breath state
        currentBreathGain = fmodParamMin;
        currentBreathQVolume = DbToFmodParam(qVolumeStillDb, qVolumeMinDb, qVolumeMaxDb);
        currentBreathFrequency = HzToFmodParam(frequencyStill);
        currentBreathState = (float)BreathingState.holdingBreath;
        outputEnvelope = 0f;

        breathInstance = CreateBreathEventInstance();
        breathInstance.setParameterByName(breathGainParam, currentBreathGain);
        breathInstance.setParameterByName(breathQVolumeParam, currentBreathQVolume);
        breathInstance.setParameterByName(breathFrequencyParam, currentBreathFrequency);
        SetBreathStateParameter(currentBreathState);
        breathInstance.start();
        breathInstance.setPaused(true);
        eventIsPlaying = false;
    }

    private void Update()
    {
        BreathingState sensorState = dataContainer.BreathingState;
        float intensity = GetBreathIntensity();
        bool hasBreathInput = intensity >= breathStartThreshold
            || (eventIsPlaying && intensity > breathStopThreshold);

        if (hasBreathInput && sensorState != BreathingState.holdingBreath)
        {
            lastActiveBreathingState = sensorState;
        }

        BreathingState effectiveState = sensorState;
        if (!hasBreathInput && eventIsPlaying && lastActiveBreathingState != BreathingState.holdingBreath)
        {
            effectiveState = lastActiveBreathingState;
        }

        outputEnvelope = Mathf.MoveTowards(
            outputEnvelope,
            hasBreathInput ? 1f : 0f,
            Time.deltaTime * GetEnvelopeSpeed(hasBreathInput, effectiveState));

        if (hasBreathInput && !eventIsPlaying)
        {
            PrepareFrequencyForBreathStart(effectiveState);
            breathInstance.setPaused(false);
            eventIsPlaying = true;
        }

        if (!hasBreathInput && outputEnvelope <= 0f)
        {
            SendSilentParameters(effectiveState);
            breathInstance.setPaused(true);
            eventIsPlaying = false;
            return;
        }

        if (!hasBreathInput && effectiveState == BreathingState.exhaling)
        {
            FadeOutExhaleWithoutFrequencySweep();
            return;
        }

        // Calculate target parameter values based on state and intensity
        float volumeIntensity = GetVolumeIntensity(intensity);
        float targetFreqHz = frequencyStill;
        float targetGainDb = gainStillDb;
        float targetQVolumeDb = qVolumeStillDb;

        if (effectiveState == BreathingState.inhaling)
        {
            // GREEN (INHALE): Frequency UP, Gain UP, Q UP
            targetFreqHz = Mathf.Lerp(frequencyMin, frequencyMax, intensity);
            targetGainDb = Mathf.Lerp(gainStillDb, gainMaxDb, volumeIntensity);
            targetQVolumeDb = Mathf.Lerp(qVolumeStillDb, qVolumeMaxDb, intensity);
        }
        else if (effectiveState == BreathingState.holdingBreath)
        {
            // HOLDING: Neutral state
            targetFreqHz = frequencyStill;
            targetGainDb = gainStillDb;
            targetQVolumeDb = qVolumeStillDb;
        }
        else if (effectiveState == BreathingState.exhaling)
        {
            // YELLOW (EXHALE): keep the breath-out band lower so the soft start and tail do not chirp upward.
            targetFreqHz = Mathf.Lerp(exhaleFrequencyRest, frequencyMin, intensity);
            targetGainDb = Mathf.Lerp(gainMinDb, gainMaxDb, volumeIntensity);
            targetQVolumeDb = Mathf.Lerp(qVolumeStillDb, qVolumeMaxDb, intensity);
        }

        // Convert Hz and dB to FMOD 0-10 parameter values
        float targetBreathFrequency = HzToFmodParam(targetFreqHz);
        float targetBreathGain = DbToFmodParam(targetGainDb, gainMinDb, gainMaxDb);
        float targetBreathQVolume = DbToFmodParam(targetQVolumeDb, qVolumeMinDb, qVolumeMaxDb);

        targetBreathGain = Mathf.Lerp(fmodParamMin, targetBreathGain, outputEnvelope);

        // Smooth transitions
        float gainSpeed = effectiveState == BreathingState.exhaling ? exhaleGainSmoothSpeed : gainSmoothSpeed;
        float qSpeed = effectiveState == BreathingState.exhaling ? exhaleQSmoothSpeed : qSmoothSpeed;
        float frequencySpeed = effectiveState == BreathingState.exhaling ? exhaleFrequencySmoothSpeed : frequencySmoothSpeed;

        currentBreathGain = Mathf.Lerp(currentBreathGain, targetBreathGain, Time.deltaTime * gainSpeed);
        currentBreathQVolume = Mathf.Lerp(currentBreathQVolume, targetBreathQVolume, Time.deltaTime * qSpeed);
        currentBreathFrequency = Mathf.Lerp(currentBreathFrequency, targetBreathFrequency, Time.deltaTime * frequencySpeed);
        currentBreathState = (float)effectiveState;

        // Send to FMOD
        breathInstance.setParameterByName(breathGainParam, currentBreathGain);
        breathInstance.setParameterByName(breathQVolumeParam, currentBreathQVolume);
        breathInstance.setParameterByName(breathFrequencyParam, currentBreathFrequency);
        SetBreathStateParameter(currentBreathState);
    }

    private float GetBreathIntensity()
    {
        if (dataOrigin == DataOrigin.SensorData)
        {
            return BreathingSensorSimulatorVisualFeedback.MapValueClamped(
                Mathf.Abs(dataContainer.inExhaleSpeed),
                0f,
                sensorSpeedForFullIntensity,
                0f,
                1f);
        }

        if (dataOrigin == DataOrigin.ManualSlider)
        {
            return Mathf.Clamp01(Mathf.Abs(simulator.GetManualSliderData()));
        }

        int phase = simulator.GetCurrentPhase();
        return Mathf.Clamp01(Mathf.Abs(simulator.GetCycleData(phase)));
    }

    private EventInstance CreateBreathEventInstance()
    {
        if (!string.IsNullOrEmpty(breathEvent.Path))
            return RuntimeManager.CreateInstance(breathEvent.Path);

        return RuntimeManager.CreateInstance(breathEvent);
    }

    private float GetVolumeIntensity(float intensity)
    {
        float power = Mathf.Max(0.01f, volumeIntensityPower);
        return Mathf.Pow(Mathf.Clamp01(intensity), power);
    }

    private float GetEnvelopeSpeed(bool hasBreathInput, BreathingState state)
    {
        if (hasBreathInput)
            return attackSpeed;

        return state == BreathingState.exhaling ? exhaleReleaseSpeed : releaseSpeed;
    }

    private void FadeOutExhaleWithoutFrequencySweep()
    {
        currentBreathGain = Mathf.Lerp(currentBreathGain, fmodParamMin, Time.deltaTime * exhaleGainSmoothSpeed);
        currentBreathQVolume = Mathf.Lerp(currentBreathQVolume, fmodParamMin, Time.deltaTime * exhaleQSmoothSpeed);

        breathInstance.setParameterByName(breathGainParam, currentBreathGain);
        breathInstance.setParameterByName(breathQVolumeParam, currentBreathQVolume);
    }

    private void PrepareFrequencyForBreathStart(BreathingState state)
    {
        if (state != BreathingState.exhaling)
            return;

        currentBreathFrequency = HzToFmodParam(exhaleFrequencyRest);
        breathInstance.setParameterByName(breathFrequencyParam, currentBreathFrequency);
    }

    private void SendSilentParameters(BreathingState state)
    {
        currentBreathGain = fmodParamMin;
        currentBreathQVolume = DbToFmodParam(qVolumeStillDb, qVolumeMinDb, qVolumeMaxDb);
        currentBreathFrequency = HzToFmodParam(frequencyStill);
        currentBreathState = (float)state;

        if (breathInstance.isValid())
        {
            breathInstance.setParameterByName(breathGainParam, currentBreathGain);
            breathInstance.setParameterByName(breathQVolumeParam, currentBreathQVolume);
            breathInstance.setParameterByName(breathFrequencyParam, currentBreathFrequency);
            SetBreathStateParameter(currentBreathState);
        }
    }

    private void SetBreathStateParameter(float value)
    {
        if (!sendBreathStateParam || string.IsNullOrEmpty(breathStateParam))
            return;

        breathInstance.setParameterByName(breathStateParam, value);
    }

    /// <summary>
    /// Convert Hz value to FMOD 0-10 parameter range
    /// Maps: frequencyMin (660Hz) → 0, frequencyMax (4070Hz) → 10
    /// </summary>
    private float HzToFmodParam(float frequencyHz)
    {
        float normalized = Mathf.Clamp01((frequencyHz - frequencyMin) / (frequencyMax - frequencyMin));
        return Mathf.Lerp(fmodParamMin, fmodParamMax, normalized);
    }

    /// <summary>
    /// Convert dB value to FMOD 0-10 parameter range
    /// Maps dB values to 0-10 range based on context
    /// </summary>
    private float DbToFmodParam(float decibels, float minDb, float maxDb)
    {
        float normalized = Mathf.Clamp01((decibels - minDb) / (maxDb - minDb));
        return Mathf.Lerp(fmodParamMin, fmodParamMax, normalized);
    }

    private void OnDestroy()
    {
        if (!breathInstance.isValid())
            return;

        breathInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        breathInstance.release();
    }
}
