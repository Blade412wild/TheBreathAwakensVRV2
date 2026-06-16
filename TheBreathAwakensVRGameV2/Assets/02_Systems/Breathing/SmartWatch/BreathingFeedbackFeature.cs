using System;
using System.ComponentModel;
using System.Drawing.Text;
using TMPro;
using UnityEngine;

public class BreathingFeedbackFeature : MonoBehaviour
{
    public event Action<Vector2> dotChangedPos;

    [Header("Control")]
    [SerializeField] private bool useFeature;
    [SerializeField] private bool useOwnUpdate;

    [Header("Visuals")]
    [SerializeField] private float highestVisualPeak;
    [SerializeField] private float lowestVisualPeak;
    [SerializeField] private float visualZeroLine;

    [Header("Sensor")]
    [SerializeField] private float highestSensorDataPeak;
    [SerializeField] private float lowestSensorDataPeak;

    [Header("References")]
    [SerializeField] private RectTransform dot;
    [SerializeField] private DotTrailBehaviour dotTrailBehaviour;
    [SerializeField] private MessageFinishedReceived messageFinishedReceived;
    [SerializeField] private SCBA scba;


    private Vector2 dotpos;
    private Vector2 dotPosTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!useOwnUpdate) return;
        Init();
        Actvate();
    }

    // Update is called once per frame
    void Update()
    {
        if (!useOwnUpdate) return;
        OnUpdate();
    }

    private void OnDisable()
    {
        if (!useOwnUpdate) return;
        Deactivate();
    }

    public void Init()
    {
        dotTrailBehaviour.Init();
    }

    public void OnUpdate()
    {
        dotTrailBehaviour.OnUpdate();
    }

    public void Actvate()
    {
        messageFinishedReceived.OnDataReceivedEvent += HandleDataReceivedEvent;
        dotTrailBehaviour.Activate();

    }

    public void Deactivate()
    {
        messageFinishedReceived.OnDataReceivedEvent -= HandleDataReceivedEvent;
        dotTrailBehaviour.Deactivate();

    }

    public void OnDeactivation()
    {
        messageFinishedReceived.OnDataReceivedEvent -= HandleDataReceivedEvent;
        dotTrailBehaviour.OnDeactivation();
    }

    private void HandleDataReceivedEvent()
    {
        GetTargetPos(scba.BreathingState, scba.InExhaleSpeed);
        dot.localPosition = dotPosTarget;
    }

    private void GetTargetPos(BreathingState currentState, float speed)
    {
        float yPosTarget = 0;

        if (currentState == BreathingState.inhaling)
        {
            yPosTarget = Mapping.MapValue(speed, lowestSensorDataPeak, highestSensorDataPeak, lowestVisualPeak, highestVisualPeak);
            dotPosTarget.y = yPosTarget;

        }
        else if (currentState == BreathingState.exhaling)
        {
            yPosTarget = Mapping.MapValue(speed, lowestSensorDataPeak, highestSensorDataPeak, lowestVisualPeak, highestVisualPeak);
            yPosTarget *= -1;
            dotPosTarget.y = yPosTarget;

        }
        else
        {
            speed = 0;
            yPosTarget = speed;
            dotPosTarget.y = yPosTarget;
        }

        //Debug.Log("rawSpeed = " + speed + " m/s | targetPos : " + dotPosTarget + " m "/* + " | OxygonUsed : " + oxygonUsed*/);
    }
}
