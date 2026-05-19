using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;
using System;

public class ExtractionManager : MonoBehaviour
{
    public event Action ExtractionSucceeded;
    public event Action ExtractionFailed;

    [Header("control")]
    [SerializeField] private bool StartTimer;

    [Header("ref")]
    [SerializeField] private TextMeshProUGUI VisualTimer;
    [SerializeField] private Timer timer;
    [SerializeField] private TriggerListener playerEnteredExtractionAreaTriggerListener;





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer.OnSecondPastEvent += UpdateVisualTimer;
        timer.OnTimerIsDone += HandleTimerIsDoneEvent;
        UpdateVisualTimer();

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

    private void UpdateVisualTimer()
    {
        string digitalTime = TimeToDIgitalClockConverter.ConvertTime(timer.currentTime);
        VisualTimer.text = digitalTime;
    }
}
