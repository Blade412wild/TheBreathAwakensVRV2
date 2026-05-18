using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;
using System;

public class ExtractionManager : MonoBehaviour
{
    public event Action ExtractionSucceeded;
    public event Action ExtractionFailed;


    [SerializeField] private bool StartTimer;

    [Header("Time")]
    [SerializeField] private float minutes; 
    [SerializeField] private float seconds;


    [Header("ref")]
    [SerializeField] private TextMeshProUGUI VisualTimer;

    private Timer timer = new Timer();
    private float totalTime => minutes * 60 + seconds;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer.OnSecondPastEvent += UpdateVisualTimer;
        timer.OnTimerIsDone += HandleTimerIsDoneEvent;
        Debug.Log("totalTime : " +  totalTime);
        timer.SetTimer(totalTime);
        UpdateVisualTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (StartTimer)
        {
            StartTimer = false;
            timer.Start();
        }

        timer.OnUpdate();
    }

    private void HandleTimerIsDoneEvent()
    {
        ExtractionFailed?.Invoke();
        Debug.Log("failed Extraction");
    }



    private void UpdateVisualTimer()
    {
        string digitalTime = TimeToDIgitalClockConverter.ConvertTime(timer.currentTime);
        VisualTimer.text = digitalTime;
    }
}
