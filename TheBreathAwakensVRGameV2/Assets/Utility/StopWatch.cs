using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class StopWatch
{
    public float currentTime { get; private set; }

    private float startTime = 0;
    private float endTime;
    private bool repeat = false;
    private int repeatAmount = 0;
    private int currentAmount = 1;
    private bool mayRun = false;

    public void Setup(float startTime)
    {
        this.startTime = startTime;
        currentTime = startTime;
    }

    
    public void Run()
    {
        mayRun = true;
    }
    public void Pause()
    {
        mayRun = false;
    }
    
    // Update is called once per frame
    public void OnUpdate()
    {
        if (!mayRun) return;
        currentTime += Time.deltaTime;


        //Debug.Log(currentTime);
        //RunStopWatch();
    }

    private void RunStopWatch()
    {
        if (currentTime >= endTime)
        {
            //Debug.Log(" Timer is finished, [" + endTime + "] have past");
            if (repeat == true && currentAmount < repeatAmount)
            {
                Debug.Log(" repeat amount = " + currentAmount);
                Debug.Log("repeat Timer");
                var t = Time.time;
                Debug.Log(t);
                currentTime = 0;
                currentAmount++;
            }
            else
            {
                //OnRemoveTimer?.Invoke(this);
            }
        }
    }

    public void ResetWatch()
    {
        currentTime = startTime;
    }
}





