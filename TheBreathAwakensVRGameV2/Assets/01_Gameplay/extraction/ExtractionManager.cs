using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;
using System;

public class ExtractionManager : MonoBehaviour
{
    public event Action<TimeLeftStruct> UpdateVisualTimerEvent;

    public event Action ExtractionSucceeded;
    public event Action ExtractionFailed;

    [Header("control")]
    [SerializeField] private bool StartTimer;

    [Header("ref")]
    [SerializeField] private TextMeshProUGUI VisualTimer;
    [SerializeField] private Timer timer;
    [SerializeField] private TriggerListener playerEnteredExtractionAreaTriggerListener;

    private TimeLeftStruct timeLeft = new TimeLeftStruct();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer.OnSecondPastEvent += handleTimerSecondPast;
        timer.OnTimerIsDone += HandleTimerIsDoneEvent;
        //handleTimerSecondPast();

        playerEnteredExtractionAreaTriggerListener.OnTargetEnteredTriggerEvent += HandlePlayerEnteredExtractionAreaEvent;
        playerEnteredExtractionAreaTriggerListener?.StartListening();
    }

    // Update is called once per frame
    void Update()
    {
        if (StartTimer)
        {
            StartTimer = false;
            timer.StartTimer();
            timeLeft = TimeLeftConversions.CreateLeftStruct(timer.currentTime);
            
        }

    }

    private void HandlePlayerEnteredExtractionAreaEvent()
    {
        ExtractionSucceeded?.Invoke();
        timer.StopTimer();
        Debug.Log("Succeeded Extraction");
    }

    private void HandleTimerIsDoneEvent()
    {
        ExtractionFailed?.Invoke();
        timer.StopTimer();
        Debug.Log("failed Extraction");
    }

    private void handleTimerSecondPast()
    {
        //Debug.Log("second passed");
        UpdateVisualTimerEvent?.Invoke(timer.timeleft);
        //string digitalTime = TimeLeftConversions.ConvertTimeToDigitalClock(timeLeft);
        //VisualTimer.text = digitalTime;
    }
}
