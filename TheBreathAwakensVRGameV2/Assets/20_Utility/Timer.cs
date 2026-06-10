using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Timers;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Timer : MonoBehaviour
{
    public event Action OnTimerIsDone;
    public event Action<Timer> OnRemoveTimer;
    public event Action OnSecondPastEvent;
    public event Action OnMinutePastEvent;

    public TimeLeftStruct timeleft = new TimeLeftStruct();

    [Header("Time")]
    [SerializeField] private float minutes;
    [SerializeField] private float seconds;

    [Space]
    [SerializeField] private bool repeat = false;
    [SerializeField] private int repeatAmount = 0;


    // timer 
    public float currentTime { get; private set; }
    private float startTime = 0;
    private float endTime = 0;
    private int currentAmount = 1;
    private bool infiniteRepeat;

    private float second;
    private float minute;

    private bool mayUpdate = false;
    private float totalTime => minutes * 60 + seconds;

    private void OnEnable()
    {
        startTime = totalTime;
        currentTime = startTime;
    }

    private void Update()
    {
        OnUpdate();
    }
    public void SetTimer(float _seconds)
    {
        startTime = _seconds;
        currentTime = startTime;
        timeleft.TotalSeconds = (int)_seconds;
    }
    public void SetTimer(float _seconds, bool repeat)
    {
        startTime = _seconds;
        currentTime = startTime;
        this.repeat = true;
        infiniteRepeat = true;
        timeleft.TotalSeconds = (int)_seconds;
    }

    public void SetTimer(float _seconds, int _amount)
    {
        startTime = _seconds;
        currentTime = startTime;
        timeleft.TotalSeconds = (int)_seconds;

        if (_amount > 0)
        {
            repeat = true;
            repeatAmount = _amount;
        }
        else
        {
            Debug.LogWarning("can't have timer repeat < 1");
        }

    }

    public void StartTimer()
    {
        Debug.Log("Start Timer");
        mayUpdate = true;
    }
    public void StopTimer()
    {
        mayUpdate = false;
    }

    public void PauzeTimer()
    {
        mayUpdate = false;
    }

    public void UnpauseTimer()
    {
        mayUpdate = true;
    }
    public void Reset()
    {
        currentTime = startTime;

    }

    public void DestroyTimer()
    {

    }



    // Update is called once per frame
    public void OnUpdate()
    {
        if (!mayUpdate) return;
        RunTimer();
    }

    private void RunTimer()
    {
        currentTime -= Time.deltaTime;

        if (second >= 1.0f)
        {
            second = 0.0f;

            timeleft = TimeLeftConversions.CreateLeftStruct(currentTime);

            OnSecondPastEvent?.Invoke();
        }
        else
        {
            second += Time.deltaTime;
        }

        if (minute >= 60.0f)
        {
            
            minute = 0.0f;

            OnMinutePastEvent?.Invoke();
        }
        else
        {
            minute += Time.deltaTime;
        }



        if (currentTime <= endTime)
        {

            //Debug.Log(" Timer is finished, [" + endTime + "] have past");
            if (repeat == true && currentAmount < repeatAmount || repeat == true && infiniteRepeat == true)
            {
                var t = Time.time;
                currentAmount++;
                Reset();
                OnTimerIsDone?.Invoke();
            }
            else
            {
                OnTimerIsDone?.Invoke();
                Reset();
                StopTimer();
                //OnRemoveTimer?.Invoke(this);
            }
        }
    }
}
