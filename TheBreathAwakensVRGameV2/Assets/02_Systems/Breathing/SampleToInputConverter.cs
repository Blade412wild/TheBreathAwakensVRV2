using UnityEngine;

public class SampleToInputConverter : MonoBehaviour
{
    [Header("Control")]
    [SerializeField] private bool update;

    [Header("Refs")]
    [SerializeField] private BreathingSample2 sample;
    [SerializeField] private BreathingDeviceData dataContainer;


    private StopWatch stopWatch = new StopWatch();

    private float startTime;
    private float endTime;
    private float currentTime;

    private void Start()
    {
        startTime = sample.Curve.keys[0].time;
        endTime = sample.Curve.keys[sample.Curve.keys.Length-1].time;

        stopWatch.Setup(startTime);
        stopWatch.Run();
    }

    private void Update()
    {
        HandleStopWatch();
        UpdateDataContainer(currentTime, sample.Curve, dataContainer);
    }

    private void HandleStopWatch()
    {
        stopWatch.OnUpdate();

        if(stopWatch.currentTime >= endTime)
        {
            stopWatch.ResetWatch();
        }

        currentTime = stopWatch.currentTime;

    }

    public void UpdateDataContainer(float currentTime, AnimationCurve curve, BreathingDeviceData dataContainer)
    {
        float rawValue = curve.Evaluate(currentTime);
        BreathingState state = GetBreathingState(rawValue);
        float convertedValue = GetTrueValue(state, rawValue);

        if(convertedValue <= 0.5)
        {
            convertedValue = 0.0f ;
        }

        dataContainer.inExhaleSpeed = convertedValue;
        dataContainer.BreathingState = state;
    }

    private BreathingState GetBreathingState(float value)
    {
        if (value > 0.05)
        {
            return BreathingState.inhaling;
        }
        else if (value < -0.5)
        {
            return BreathingState.exhaling;
        }
        else
        {
            return BreathingState.holdingBreath;
        }
    }
    private float GetTrueValue(BreathingState state, float value)
    {
        if (state == BreathingState.exhaling)
        {
            return value * -1;
        }
        else
        {
            return value;

        }
    }




}
