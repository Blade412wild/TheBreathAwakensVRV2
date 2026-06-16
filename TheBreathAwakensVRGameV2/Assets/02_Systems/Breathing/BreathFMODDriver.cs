using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class BreathFMODDriver : MonoBehaviour
{
    public enum DataOrigin { BreathCycle, ManualSlider, SensorData }

    [Header("Control")]
    [SerializeField] private bool activate;
    [SerializeField] private bool deActivate;

    [Header("References")]
    [SerializeField] private BreathingSensorSimulator simulator;
    [SerializeField] private EventReference breathEvent;
    [SerializeField] private BreathingDeviceData dataContainer;
    [SerializeField] private SCBA scba;
    [SerializeField] private OxygonMask mask;


    [Header("FMOD Parameter Names")]
    [SerializeField] private string breathGainParam = "audioBreathGain";
    [SerializeField] private string breathQVolumeParam = "audioBreathQVolume";
    [SerializeField] private string breathFrequencyParam = "audioBreathFrequency";
    [SerializeField] private string breathStateParam = "BreathState";
    [SerializeField] private bool sendBreathStateParam;

    [Header("Mask Resonance Parameter Names")]
    [SerializeField] private bool sendMaskResonanceParams;
    [SerializeField] private string maskLowFreqParam = "maskLowFreq";
    [SerializeField] private string maskLowGainParam = "maskLowGain";
    [SerializeField] private string maskLowQParam = "maskLowQ";
    [SerializeField] private string maskMidFreqParam = "maskMidFreq";
    [SerializeField] private string maskMidGainParam = "maskMidGain";
    [SerializeField] private string maskMidQParam = "maskMidQ";
    [SerializeField] private string maskHighFreqParam = "maskHighFreq";
    [SerializeField] private string maskHighGainParam = "maskHighGain";
    [SerializeField] private string maskHighQParam = "maskHighQ";

    [Header("Reverb")]
    [SerializeField] private bool controlEventReverbLevel = true;
    [Range(0f, 1f)]
    [SerializeField] private float eventReverbLevel = 0.75f;
    [SerializeField] private bool scaleReverbWithBreathRate = true;
    [Range(0f, 1f)]
    [SerializeField] private float slowBreathReverbAmount = 0.45f;
    [Range(0f, 1f)]
    [SerializeField] private float fastBreathReverbAmount = 0.24f;
    [Range(0f, 1f)]
    [SerializeField] private float intenseBreathReverbReduction = 0.24f;
    [Range(0f, 1f)]
    [SerializeField] private float idleReverbAmount = 0f;
    [SerializeField] private float reverbActivityPower = 1.8f;
    [SerializeField] private Vector2 breathPhaseDurationRange = new Vector2(0.35f, 2.0f);
    [SerializeField] private float breathRateSmoothSpeed = 4f;
    [SerializeField] private float reverbReleaseSmoothSpeed = 24f;
    [SerializeField] private string breathReverbParam = "audioBreathReverb";
    [SerializeField] private bool sendBreathReverbParam;
    [Range(0f, 1f)]
    [SerializeField] private float breathReverbAmount = 0.75f;

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
    [Range(0f, 2f)]
    [SerializeField] private float outputVolume = 1f;
    [SerializeField] private float eventVolumeDb = 0f;

    [Header("Q Volume Mapping (dB, Q=2.0)")]
    [SerializeField] private float qVolumeMinDb = 0f;
    [SerializeField] private float qVolumeMaxDb = 12.5f;
    [SerializeField] private float qVolumeStillDb = 0f;

    [Header("Mask Resonance Mapping")]
    [SerializeField] private Vector2 lowResonanceHz = new Vector2(175f, 300f);
    [SerializeField] private Vector2 midResonanceHz = new Vector2(500f, 600f);
    [SerializeField] private Vector2 highResonanceHz = new Vector2(700f, 1000f);
    [SerializeField] private float lowResonanceGainDb = 6f;
    [SerializeField] private float midResonanceGainDb = 4.5f;
    [SerializeField] private float highResonanceGainDb = 5.5f;
    [SerializeField] private float inhaleLowGainMultiplier = 0.65f;
    [SerializeField] private float inhaleMidGainMultiplier = 1.0f;
    [SerializeField] private float inhaleHighGainMultiplier = 1.15f;
    [SerializeField] private float exhaleLowGainMultiplier = 1.15f;
    [SerializeField] private float exhaleMidGainMultiplier = 0.9f;
    [SerializeField] private float exhaleHighGainMultiplier = 0.65f;
    [SerializeField] private Vector2 maskResonanceQRange = new Vector2(2.2f, 6.0f);
    [SerializeField] private float maskResonanceMotionAmount = 0.12f;
    [SerializeField] private float maskResonanceSmoothSpeed = 7f;

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

    [Header("Input Smoothing")]
    [SerializeField] private float inputAttackSmoothSpeed = 18f;
    [SerializeField] private float activeInputFallSmoothSpeed = 7f;
    [SerializeField] private float inhaleInputReleaseSmoothSpeed = 2.6f;
    [SerializeField] private float exhaleInputReleaseSmoothSpeed = 2.2f;
    [SerializeField] private float smoothedInputStopThreshold = 0.015f;

    [Header("Control")]
    public DataOrigin dataOrigin = DataOrigin.SensorData;
    [SerializeField] private float sensorSpeedForFullIntensity = 6f;
    [SerializeField] private float breathStartThreshold = 0.001f;
    [SerializeField] private float breathStopThreshold = 0.0005f;
    [SerializeField] private float attackSpeed = 3.5f;
    [SerializeField] private float releaseSpeed = 1.8f;
    [SerializeField] private float exhaleReleaseSpeed = 3.4f;

    [Header("Release Tail")]
    [SerializeField] private float inhaleTailGainDb = -28f;
    [SerializeField] private float inhaleTailQVolumeDb = 1f;
    [SerializeField] private float inhaleTailGainSmoothSpeed = 3.8f;
    [SerializeField] private float inhaleTailQSmoothSpeed = 3.8f;
    [SerializeField] private float exhaleTailGainDb = -26f;
    [SerializeField] private float exhaleTailQVolumeDb = 2f;
    [SerializeField] private float exhaleTailGainSmoothSpeed = 4.6f;
    [SerializeField] private float exhaleTailQSmoothSpeed = 4.6f;
    [SerializeField] private float releaseMinInputScale = 0.5f;
    [SerializeField] private float releaseInputMemoryFalloff = 1.5f;
    [SerializeField] private float releasePauseThreshold = 0.02f;
    [SerializeField] private float releaseGainStopThreshold = 0.05f;
    [SerializeField] private bool pauseEventWhenSilent = true;
    [SerializeField] private float idlePauseDelay = 0.35f;
    [SerializeField] private float holdPauseDelay = 0.12f;

    private EventInstance breathInstance;

    private float currentBreathGain;
    private float currentBreathQVolume;
    private float currentBreathFrequency;
    private float currentBreathState;
    private float currentMaskLowFreq;
    private float currentMaskLowGain;
    private float currentMaskLowQ;
    private float currentMaskMidFreq;
    private float currentMaskMidGain;
    private float currentMaskMidQ;
    private float currentMaskHighFreq;
    private float currentMaskHighGain;
    private float currentMaskHighQ;
    private float smoothedInputIntensity;
    private float currentBreathRate;
    private float targetBreathRate;
    private float lastActivePhaseStartTime = -1f;
    private float currentDynamicReverbAmount;
    private float outputEnvelope;
    private float lastInputIntensity;
    private float releaseInputScale = 1f;
    private float idleTimer;
    private float holdTimer;
    private bool eventIsPlaying;
    private bool hadBreathInputLastFrame;
    private BreathingState previousRawActiveState = BreathingState.holdingBreath;
    private BreathingState lastActiveBreathingState = BreathingState.holdingBreath;
    private bool isActive;

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
            Debug.LogWarning("BreathFMODDriver: simulator reference is missing. Falling back to SensorData/DataContainer input.");
            dataOrigin = DataOrigin.SensorData;
        }

        // Initialize to neutral/holding breath state
        currentBreathGain = fmodParamMin;
        currentBreathQVolume = DbToFmodParam(qVolumeStillDb, qVolumeMinDb, qVolumeMaxDb);
        currentBreathFrequency = HzToFmodParam(frequencyStill);
        currentBreathState = (float)BreathingState.holdingBreath;
        currentMaskLowFreq = lowResonanceHz.x;
        currentMaskMidFreq = midResonanceHz.x;
        currentMaskHighFreq = highResonanceHz.x;
        currentMaskLowGain = 0f;
        currentMaskMidGain = 0f;
        currentMaskHighGain = 0f;
        currentMaskLowQ = maskResonanceQRange.x;
        currentMaskMidQ = maskResonanceQRange.x;
        currentMaskHighQ = maskResonanceQRange.x;
        currentDynamicReverbAmount = breathReverbAmount;
        outputEnvelope = 0f;

        breathInstance = CreateBreathEventInstance();
        breathInstance.setParameterByName(breathGainParam, currentBreathGain);
        breathInstance.setParameterByName(breathQVolumeParam, currentBreathQVolume);
        breathInstance.setParameterByName(breathFrequencyParam, currentBreathFrequency);
        ApplyEventVolume();
        SetBreathStateParameter(currentBreathState);
        ApplyBreathReverbControl();
        ApplyMaskResonanceParameters();
        breathInstance.start();
        breathInstance.setPaused(true);
        eventIsPlaying = false;

        mask.EquipEvent += Activate;
        mask.UnequipEvent += Deactivate;

    }
    private void Update()
    {
        if (activate)
        {
            activate = false;
            Activate();
        }

        if (deActivate)
        {
            deActivate = false;
            Deactivate();
        }

        if (isActive)
        {
            OnUpdate();
        }
    }

    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
        StopBreathEventImmediately();
        //breathInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    private void OnUpdate()
    {
        BreathingState sensorState = dataContainer.BreathingState;
        float rawIntensity = GetBreathIntensity();
        bool rawHasActiveBreathInput = rawIntensity >= breathStartThreshold
            && sensorState != BreathingState.holdingBreath;

        UpdateBreathRate(sensorState, rawHasActiveBreathInput);
        float intensity = SmoothInputIntensity(rawIntensity, sensorState, rawHasActiveBreathInput);
        bool hasActiveBreathInput = rawHasActiveBreathInput;
        bool isReleasingBreath = !rawHasActiveBreathInput
            && eventIsPlaying
            && lastActiveBreathingState != BreathingState.holdingBreath
            && (smoothedInputIntensity > smoothedInputStopThreshold
                || outputEnvelope > releasePauseThreshold
                || currentBreathGain > fmodParamMin + releaseGainStopThreshold);

        if (rawHasActiveBreathInput)
        {
            idleTimer = 0f;
            holdTimer = 0f;
            lastActiveBreathingState = sensorState;
            lastInputIntensity = Mathf.Max(
                intensity,
                Mathf.MoveTowards(lastInputIntensity, intensity, Time.deltaTime * releaseInputMemoryFalloff));
        }
        else if (hadBreathInputLastFrame)
        {
            releaseInputScale = Mathf.Lerp(
                Mathf.Clamp01(releaseMinInputScale),
                1f,
                Mathf.Clamp01(lastInputIntensity));
        }

        BreathingState effectiveState = sensorState;
        if (sensorState == BreathingState.holdingBreath
            && eventIsPlaying
            && lastActiveBreathingState != BreathingState.holdingBreath)
        {
            effectiveState = lastActiveBreathingState;
        }
        else if (!hasActiveBreathInput && eventIsPlaying && lastActiveBreathingState != BreathingState.holdingBreath)
        {
            effectiveState = lastActiveBreathingState;
        }

        outputEnvelope = Mathf.MoveTowards(
            outputEnvelope,
            rawHasActiveBreathInput ? 1f : 0f,
            Time.deltaTime * GetEnvelopeSpeed(hasActiveBreathInput, effectiveState));

        if (!rawHasActiveBreathInput && sensorState == BreathingState.holdingBreath && eventIsPlaying)
        {
            holdTimer += Time.deltaTime;
            ApplyBreathReverbControl();

            if (pauseEventWhenSilent && holdTimer >= holdPauseDelay)
            {
                SendSilentParameters(effectiveState);
                StopBreathEventImmediately();
                holdTimer = 0f;
                idleTimer = 0f;
                lastInputIntensity = 0f;
                hadBreathInputLastFrame = hasActiveBreathInput;
                return;
            }
        }

        if (hasActiveBreathInput && !eventIsPlaying)
        {
            PrepareFrequencyForBreathStart(effectiveState);
            StartBreathEvent();
        }

        if (!rawHasActiveBreathInput
            && outputEnvelope <= releasePauseThreshold
            && currentBreathGain <= fmodParamMin + releaseGainStopThreshold)
        {
            SendSilentParameters(effectiveState);
            idleTimer += Time.deltaTime;

            if (pauseEventWhenSilent && idleTimer >= idlePauseDelay)
            {
                StopBreathEventImmediately();
                idleTimer = 0f;
            }
            lastInputIntensity = 0f;
            hadBreathInputLastFrame = hasActiveBreathInput;
            return;
        }

        if (isReleasingBreath && effectiveState != BreathingState.holdingBreath)
        {
            FadeOutBreathWithoutFrequencySweep(effectiveState);
            hadBreathInputLastFrame = hasActiveBreathInput;
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

        targetBreathGain = ApplyOutputVolume(Mathf.Lerp(fmodParamMin, targetBreathGain, outputEnvelope));

        // Smooth transitions
        float gainSpeed = effectiveState == BreathingState.exhaling ? exhaleGainSmoothSpeed : gainSmoothSpeed;
        float qSpeed = effectiveState == BreathingState.exhaling ? exhaleQSmoothSpeed : qSmoothSpeed;
        float frequencySpeed = effectiveState == BreathingState.exhaling ? exhaleFrequencySmoothSpeed : frequencySmoothSpeed;

        currentBreathGain = Mathf.Lerp(currentBreathGain, targetBreathGain, Time.deltaTime * gainSpeed);
        currentBreathQVolume = Mathf.Lerp(currentBreathQVolume, targetBreathQVolume, Time.deltaTime * qSpeed);
        currentBreathFrequency = Mathf.Lerp(currentBreathFrequency, targetBreathFrequency, Time.deltaTime * frequencySpeed);
        currentBreathState = (float)effectiveState;
        UpdateMaskResonance(effectiveState, intensity);

        // Send to FMOD
        breathInstance.setParameterByName(breathGainParam, currentBreathGain);
        breathInstance.setParameterByName(breathQVolumeParam, currentBreathQVolume);
        breathInstance.setParameterByName(breathFrequencyParam, currentBreathFrequency);
        ApplyEventVolume();
        SetBreathStateParameter(currentBreathState);
        ApplyBreathReverbControl();
        ApplyMaskResonanceParameters();
        hadBreathInputLastFrame = hasActiveBreathInput;
    }

    private float GetBreathIntensity()
    {
        if (dataOrigin == DataOrigin.SensorData || simulator == null)
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

    private float SmoothInputIntensity(float rawIntensity, BreathingState sensorState, bool rawHasActiveBreathInput)
    {
        float targetIntensity = rawHasActiveBreathInput ? rawIntensity : 0f;
        float smoothSpeed;

        if (rawHasActiveBreathInput)
        {
            smoothSpeed = targetIntensity >= smoothedInputIntensity
                ? inputAttackSmoothSpeed
                : activeInputFallSmoothSpeed;
        }
        else
        {
            smoothSpeed = lastActiveBreathingState == BreathingState.exhaling
                ? exhaleInputReleaseSmoothSpeed
                : inhaleInputReleaseSmoothSpeed;
        }

        smoothedInputIntensity = Mathf.MoveTowards(
            smoothedInputIntensity,
            targetIntensity,
            Time.deltaTime * smoothSpeed);

        if (!rawHasActiveBreathInput && smoothedInputIntensity < smoothedInputStopThreshold)
        {
            smoothedInputIntensity = 0f;
        }

        return smoothedInputIntensity;
    }

    private void UpdateBreathRate(BreathingState sensorState, bool rawHasActiveBreathInput)
    {
        if (rawHasActiveBreathInput && sensorState != previousRawActiveState)
        {
            if (lastActivePhaseStartTime >= 0f)
            {
                float phaseDuration = Mathf.Max(0.01f, Time.time - lastActivePhaseStartTime);
                float fastDuration = Mathf.Min(breathPhaseDurationRange.x, breathPhaseDurationRange.y);
                float slowDuration = Mathf.Max(breathPhaseDurationRange.x, breathPhaseDurationRange.y);
                targetBreathRate = 1f - Mathf.InverseLerp(fastDuration, slowDuration, phaseDuration);
            }

            lastActivePhaseStartTime = Time.time;
            previousRawActiveState = sensorState;
        }
        else if (!rawHasActiveBreathInput && previousRawActiveState != BreathingState.holdingBreath)
        {
            previousRawActiveState = BreathingState.holdingBreath;
        }

        currentBreathRate = Mathf.Lerp(
            currentBreathRate,
            targetBreathRate,
            Time.deltaTime * breathRateSmoothSpeed);
    }

    private EventInstance CreateBreathEventInstance()
    {
        if (!string.IsNullOrEmpty(breathEvent.Path))
            return RuntimeManager.CreateInstance(breathEvent.Path);

        return RuntimeManager.CreateInstance(breathEvent);
    }

    private void StartBreathEvent()
    {
        if (!breathInstance.isValid())
            breathInstance = CreateBreathEventInstance();

        breathInstance.setParameterByName(breathGainParam, currentBreathGain);
        breathInstance.setParameterByName(breathQVolumeParam, currentBreathQVolume);
        breathInstance.setParameterByName(breathFrequencyParam, currentBreathFrequency);
        ApplyEventVolume();
        ApplyBreathReverbControl();
        ApplyMaskResonanceParameters();
        breathInstance.setPaused(false);
        breathInstance.start();
        eventIsPlaying = true;
    }

    private void StopBreathEventImmediately()
    {
        if (!breathInstance.isValid())
            return;

        breathInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        eventIsPlaying = false;
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

        float baseSpeed = state == BreathingState.exhaling ? exhaleReleaseSpeed : releaseSpeed;
        return baseSpeed / Mathf.Max(0.01f, releaseInputScale);
    }

    private void FadeOutBreathWithoutFrequencySweep(BreathingState state)
    {
        bool isExhale = state == BreathingState.exhaling;
        float tailGainDb = isExhale ? exhaleTailGainDb : inhaleTailGainDb;
        float tailQVolumeDb = isExhale ? exhaleTailQVolumeDb : inhaleTailQVolumeDb;
        float tailGainSpeed = isExhale ? exhaleTailGainSmoothSpeed : inhaleTailGainSmoothSpeed;
        float tailQSpeed = isExhale ? exhaleTailQSmoothSpeed : inhaleTailQSmoothSpeed;

        float dynamicSpeedScale = 1f / Mathf.Max(0.01f, releaseInputScale);
        float tailGain = ApplyOutputVolume(DbToFmodParam(tailGainDb, gainMinDb, gainMaxDb));
        float tailQVolume = DbToFmodParam(tailQVolumeDb, qVolumeMinDb, qVolumeMaxDb);
        float releaseFade = Mathf.InverseLerp(releasePauseThreshold, 1f, outputEnvelope);

        float targetGain = Mathf.Lerp(fmodParamMin, tailGain, releaseFade);
        float targetQVolume = Mathf.Lerp(fmodParamMin, tailQVolume, releaseFade);

        currentBreathGain = Mathf.Lerp(currentBreathGain, targetGain, Time.deltaTime * tailGainSpeed * dynamicSpeedScale);
        currentBreathQVolume = Mathf.Lerp(currentBreathQVolume, targetQVolume, Time.deltaTime * tailQSpeed * dynamicSpeedScale);

        breathInstance.setParameterByName(breathGainParam, currentBreathGain);
        breathInstance.setParameterByName(breathQVolumeParam, currentBreathQVolume);
        ApplyEventVolume();
        ApplyBreathReverbControl();
        UpdateMaskResonance(state, outputEnvelope * lastInputIntensity);
        ApplyMaskResonanceParameters();
    }

    private void UpdateMaskResonance(BreathingState state, float intensity)
    {
        float clampedIntensity = Mathf.Clamp01(intensity) * outputEnvelope;
        float volumeIntensity = GetVolumeIntensity(clampedIntensity);
        float inhaleAmount = state == BreathingState.inhaling ? 1f : 0f;
        float exhaleAmount = state == BreathingState.exhaling ? 1f : 0f;
        float motion = Mathf.Clamp01(maskResonanceMotionAmount);

        float lowPosition = Mathf.Clamp01(0.25f + clampedIntensity * 0.45f + inhaleAmount * 0.15f - exhaleAmount * 0.1f);
        float midPosition = Mathf.Clamp01(0.35f + clampedIntensity * 0.35f + inhaleAmount * 0.1f);
        float highPosition = Mathf.Clamp01(0.2f + clampedIntensity * 0.65f + inhaleAmount * 0.15f - exhaleAmount * 0.1f);

        lowPosition = AddResonanceDrift(lowPosition, 0.71f, 0.0f, motion);
        midPosition = AddResonanceDrift(midPosition, 0.93f, 1.7f, motion);
        highPosition = AddResonanceDrift(highPosition, 1.23f, 3.1f, motion);

        float lowMultiplier = state == BreathingState.exhaling ? exhaleLowGainMultiplier : inhaleLowGainMultiplier;
        float midMultiplier = state == BreathingState.exhaling ? exhaleMidGainMultiplier : inhaleMidGainMultiplier;
        float highMultiplier = state == BreathingState.exhaling ? exhaleHighGainMultiplier : inhaleHighGainMultiplier;

        float targetLowFreq = Mathf.Lerp(lowResonanceHz.x, lowResonanceHz.y, lowPosition);
        float targetMidFreq = Mathf.Lerp(midResonanceHz.x, midResonanceHz.y, midPosition);
        float targetHighFreq = Mathf.Lerp(highResonanceHz.x, highResonanceHz.y, highPosition);
        float targetLowGain = lowResonanceGainDb * volumeIntensity * lowMultiplier;
        float targetMidGain = midResonanceGainDb * volumeIntensity * midMultiplier;
        float targetHighGain = highResonanceGainDb * volumeIntensity * highMultiplier;
        float targetQ = Mathf.Lerp(maskResonanceQRange.x, maskResonanceQRange.y, clampedIntensity);

        float smooth = Time.deltaTime * maskResonanceSmoothSpeed;
        currentMaskLowFreq = Mathf.Lerp(currentMaskLowFreq, targetLowFreq, smooth);
        currentMaskMidFreq = Mathf.Lerp(currentMaskMidFreq, targetMidFreq, smooth);
        currentMaskHighFreq = Mathf.Lerp(currentMaskHighFreq, targetHighFreq, smooth);
        currentMaskLowGain = Mathf.Lerp(currentMaskLowGain, targetLowGain, smooth);
        currentMaskMidGain = Mathf.Lerp(currentMaskMidGain, targetMidGain, smooth);
        currentMaskHighGain = Mathf.Lerp(currentMaskHighGain, targetHighGain, smooth);
        currentMaskLowQ = Mathf.Lerp(currentMaskLowQ, targetQ, smooth);
        currentMaskMidQ = Mathf.Lerp(currentMaskMidQ, targetQ, smooth);
        currentMaskHighQ = Mathf.Lerp(currentMaskHighQ, targetQ, smooth);
    }

    private float AddResonanceDrift(float position, float speed, float phase, float amount)
    {
        float drift = Mathf.Sin((Time.time * speed + phase) * Mathf.PI * 2f) * amount;
        return Mathf.Clamp01(position + drift);
    }

    private void ApplyMaskResonanceParameters()
    {
        if (!sendMaskResonanceParams)
            return;

        SetParameterIfNamed(maskLowFreqParam, currentMaskLowFreq);
        SetParameterIfNamed(maskLowGainParam, currentMaskLowGain);
        SetParameterIfNamed(maskLowQParam, currentMaskLowQ);
        SetParameterIfNamed(maskMidFreqParam, currentMaskMidFreq);
        SetParameterIfNamed(maskMidGainParam, currentMaskMidGain);
        SetParameterIfNamed(maskMidQParam, currentMaskMidQ);
        SetParameterIfNamed(maskHighFreqParam, currentMaskHighFreq);
        SetParameterIfNamed(maskHighGainParam, currentMaskHighGain);
        SetParameterIfNamed(maskHighQParam, currentMaskHighQ);
    }

    private void SetParameterIfNamed(string parameterName, float value)
    {
        if (string.IsNullOrEmpty(parameterName))
            return;

        breathInstance.setParameterByName(parameterName, value);
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
            ApplyEventVolume();
            SetBreathStateParameter(currentBreathState);
            ApplyBreathReverbControl();
        }
    }

    private void SetBreathStateParameter(float value)
    {
        if (!sendBreathStateParam || string.IsNullOrEmpty(breathStateParam))
            return;

        breathInstance.setParameterByName(breathStateParam, value);
    }

    private void ApplyBreathReverbControl()
    {
        SetEventReverbLevel();
        SetBreathReverbParameter();
    }

    private void SetEventReverbLevel()
    {
        if (!controlEventReverbLevel)
            return;

        breathInstance.setReverbLevel(0, GetCurrentReverbAmount());
    }

    private void SetBreathReverbParameter()
    {
        if (!sendBreathReverbParam || string.IsNullOrEmpty(breathReverbParam))
            return;

        breathInstance.setParameterByName(breathReverbParam, GetCurrentReverbAmount());
    }

    private float GetCurrentReverbAmount()
    {
        float targetReverbAmount = scaleReverbWithBreathRate
            ? Mathf.Lerp(slowBreathReverbAmount, fastBreathReverbAmount, currentBreathRate)
            : breathReverbAmount;

        float reverbActivity = Mathf.Pow(
            Mathf.Clamp01(Mathf.Max(outputEnvelope, smoothedInputIntensity)),
            Mathf.Max(0.01f, reverbActivityPower));
        if (dataContainer.BreathingState == BreathingState.holdingBreath)
        {
            reverbActivity = 0f;
        }

        targetReverbAmount -= smoothedInputIntensity * intenseBreathReverbReduction;
        targetReverbAmount = Mathf.Lerp(idleReverbAmount, targetReverbAmount, reverbActivity);
        targetReverbAmount = Mathf.Clamp01(targetReverbAmount);

        float smoothSpeed = targetReverbAmount < currentDynamicReverbAmount
            ? reverbReleaseSmoothSpeed
            : breathRateSmoothSpeed;

        currentDynamicReverbAmount = Mathf.Lerp(
            currentDynamicReverbAmount,
            targetReverbAmount,
            Time.deltaTime * smoothSpeed);

        return currentDynamicReverbAmount;
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

    private float ApplyOutputVolume(float gainParameterValue)
    {
        return Mathf.Clamp(gainParameterValue * outputVolume, fmodParamMin, fmodParamMax);
    }

    private void ApplyEventVolume()
    {
        if (!breathInstance.isValid())
            return;

        breathInstance.setVolume(DbToLinear(eventVolumeDb));
    }

    private float DbToLinear(float decibels)
    {
        return Mathf.Pow(10f, decibels / 20f);
    }

    private void OnDestroy()
    {
        if (!breathInstance.isValid())
            return;

        breathInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        breathInstance.release();
    }
}
