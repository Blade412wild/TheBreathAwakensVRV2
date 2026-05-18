using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Timers;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Timer
{
    public event Action OnTimerIsDone;
    public event Action<Timer> OnRemoveTimer;
    public event Action OnSecondPastEvent;
    public event Action OnMinutePastEvent;

    // timer 
    private float startTime = 0;
    public float currentTime;
    private float endTime = 0;
    private bool repeat = false;
    private int repeatAmount = 0;
    private int currentAmount = 1;

    private float second;
    private float minute;

    private bool mayUpdate = false;

    public Timer()
    {

    }

    //public Timer(float _seconds)
    //{
    //    startTime = _seconds;
    //    currentTime = startTime;
    //}

    //public Timer(float _seconds, bool _repeat)
    //{
    //    startTime = _seconds;
    //    repeat = _repeat;
    //    currentTime = startTime;
    //}

    //public Timer(float _seconds, bool _repeat, int _amount)
    //{
    //    startTime = _seconds;
    //    repeat = _repeat;
    //    repeatAmount = _amount;
    //    currentTime = startTime;
    //}

    public void SetTimer(float _seconds)
    {
        startTime = _seconds;
        currentTime = startTime;
    }

    public void SetTimer(float _seconds, int _amount)
    {
        startTime = _seconds;
        currentTime = startTime;

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

    public void Start()
    {
        Debug.Log("Start Timer");
        mayUpdate = true;
    }
    public void Stop()
    {
        mayUpdate = false;
    }

    public void Pauze()
    {
        mayUpdate = false;
    }

    public void Unpause()
    {
        mayUpdate = true;
    }
    public void Reset()
    {
        currentTime = startTime;
    }

    public void Destroy()
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
            OnSecondPastEvent?.Invoke();
        }
        else
        {
            second++;
        }

        if (minute >= 60.0f)
        {
            minute = 0.0f;
            OnMinutePastEvent?.Invoke();
        }
        else
        {
            minute++;
        }



        if (currentTime <= endTime)
        {
            //Debug.Log(" Timer is finished, [" + endTime + "] have past");
            if (repeat == true && currentAmount < repeatAmount)
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
                Stop();
                //OnRemoveTimer?.Invoke(this);
            }
        }
    }
}