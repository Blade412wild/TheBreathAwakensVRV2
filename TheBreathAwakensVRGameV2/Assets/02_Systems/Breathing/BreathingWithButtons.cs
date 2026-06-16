using System.Drawing.Text;
using Unity.XR.CoreUtils.GUI;
using UnityEngine;
using UnityEngine.InputSystem;

public class BreathingWithButtons : MonoBehaviour
{
    [Header("Control")]
    [SerializeField] private bool useThis;
    [SerializeField] private bool activate;
    [SerializeField] private bool deactivate;

    [SerializeField] private AnimationCurve StartCurve;
    [SerializeField] private AnimationCurve EndCurve;

    [SerializeField] private float maxInhaleSpeed;
    [SerializeField] private float maxExhaleSpeed;


    [Range(0, 15)]
    [SerializeField] private float speed;

    [Header("Ref")]
    [SerializeField] private InputActionReference actionReference;
    [SerializeField] private BreathingDeviceData data;
    [SerializeField] private MessageFinishedReceived MessageFinishedReceived;

    [SerializeField] private Timer timer;

    private StopWatch stopWatch = new StopWatch();
    private BreathingState state;

    private bool IsActive;
    private AnimationCurve activeCurve;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        actionReference.action.Enable();

        timer.OnTimerIsDone += HandleTimerDoneEvent;
        timer.SetTimer(0.125f, true);

        actionReference.action.started += HandleStart;
        actionReference.action.canceled += HandleRelease;

    }

    // Update is called once per frame
    void Update()
    {
        if (activate)
            Activate();

        if (deactivate)
            Deactivate();

        if (!IsActive && useThis)
        {
            float value = actionReference.action.ReadValue<float>();
            if (value == 1.0f)
            {
                Activate();
                //HandleStart();
            }
        }


        if (!IsActive) return;

        stopWatch.OnUpdate();

        float currentTime = stopWatch.currentTime;
        float speed = GetSpeedFromCurve(currentTime);

        //Debug.Log("speed: " + speed + " | state " + state);

        if (state != BreathingState.holdingBreath && speed <= 0)
        {
            state = BreathingState.holdingBreath;
            data.BreathingState = BreathingState.holdingBreath;
            Debug.Log("state == holdingbreath");
        }

        data.inExhaleSpeed = speed;
    }


    public void Activate()
    {
        if (IsActive) return;

        actionReference.action.started += HandleStart;
        actionReference.action.canceled += HandleRelease;

        Debug.Log("activate");
        IsActive = true;
        stopWatch.Run();
        timer.StartTimer();
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        actionReference.action.started -= HandleStart;
        actionReference.action.canceled -= HandleRelease;

        IsActive = false;
        timer.PauzeTimer();
        stopWatch.Pause();
    }
    private void SetAudioVariable()
    {
        //Debug.Log(data.inExhaleSpeedAudioScale);
    }
    private void HandleStart(InputAction.CallbackContext contex)
    {
        stopWatch.ResetWatch();
        float value = actionReference.action.ReadValue<float>();
        SetbreathingState(value);
        activeCurve = StartCurve;


    }

    private void HandleRelease(InputAction.CallbackContext contex)
    {
        stopWatch.ResetWatch();
        //float value = actionReference.action.ReadValue<float>();
        //SetbreathingState(value);
        activeCurve = EndCurve;


    }

    private float GetSpeedFromCurve(float time)
    {
        if(activeCurve == EndCurve)
        {

        }
        float value = activeCurve.Evaluate(time);
        float mappedValue = 0.0f;

        //Debug.Log("time : " + time);
        //Debug.Log("local state : " + state + " | datacvontqainer " + data.BreathingState);

        if (data.BreathingState == BreathingState.inhaling)
        {
            mappedValue = Mapping.MapValue(value, 0, 1, 0, maxInhaleSpeed);
            //Debug.Log("speed : " + mappedValue);

        }

        if (data.BreathingState == BreathingState.exhaling)
        {
            mappedValue = Mapping.MapValue(value, 0, 1, 0, maxExhaleSpeed);
            //Debug.Log("speed : " + mappedValue);

        }

        return mappedValue;
    }

    private void SetInhaleExhaleSpeed(float input)
    {
        float artificialSpeed = ManipulateSpeed(input);
        data.inExhaleSpeed = artificialSpeed;

    }

    private void SetbreathingState(float input)
    {
        if (input == 0)
        {
            data.BreathingState = BreathingState.holdingBreath;
            state = BreathingState.holdingBreath;
        }
        else if (input == -1)
        {
            data.BreathingState = BreathingState.exhaling;
            state = BreathingState.exhaling;

        }
        else if (input == 1)
        {
            data.BreathingState = BreathingState.inhaling;
            state = BreathingState.inhaling;
        }

    }

    private float ManipulateSpeed(float input)
    {
        float newSpeed = speed;
        //Debug.Log("value : " + rawValue);
        return newSpeed;

    }

    private void HandleTimerDoneEvent()
    {
        MessageFinishedReceived.OnDataReceived("Button");
    }
}
