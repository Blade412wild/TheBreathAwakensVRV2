using FMODUnity;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class SCBA : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private bool mayUpdate;
    [SerializeField] private int maxCyclesPerSample;

    [Header("References")]
    // SCBA = Self-Contained breathing Apparatus 
    [SerializeField] private OxygonTank tank;
    [SerializeField] private OxygonMask mask;
    [SerializeField] private SCBASmartWatch watch;
    [SerializeField] private BreathingDeviceData breathingData;
    [SerializeField] private MessageFinishedReceived messageFinishedReceived;
    //[SerializeField] private In_ExhaleSpeedDataReceived received;

    [Header("FMOD")]
    [SerializeField] private EventReference BreathingStateChangeFMOD;

    private BreathingSystemManager breathingSystemManager;
    private OxygonPrediction oxygonPrediction;

    private void Start()
    {
        CheckReferences();
        breathingSystemManager = new BreathingSystemManager(messageFinishedReceived, breathingData, maxCyclesPerSample);

        tank.Setup();
        watch.UpdateOxygonUI(tank.AvailableOxygon, tank.AvailableOxygonPercentage);

        //received.OnDataReceivedEvent += Test;
    }

    private void Update()
    {
        if (!mayUpdate) return;
        RunSCBASystem();
    }

    private void RunSCBASystem()
    {
        breathingSystemManager.OnUpdate();

        if (breathingData.BreathingState == BreathingState.inhaling)
        {
            HandleInhaling();
            watch.UpdateOxygonUI((int)tank.AvailableOxygon, (int)tank.AvailableOxygonPercentage);
        }
    }

    private void HandleInhaling()
    {
        tank.UseOxygonTank(breathingData.inExhaleSpeed);

    }

    private void HandleBreathingStateChange()
    {


        if (!BreathingStateChangeFMOD.IsNull)
        {
            RuntimeManager.PlayOneShot(BreathingStateChangeFMOD);
        }
        else
        {
            //Debug.LogWarning("didn;t reference for breathingStateChanged");
        }

    }

    private void CheckReferences()
    {

        if (tank == null)
        {
            Debug.LogWarning("oxygonTank is Null");
        }

        if (mask == null)
        {
            Debug.LogWarning("Mask is Null");
        }

        if (breathingData == null)
        {
            Debug.LogWarning("BreathingData is Null");
        }

        if (BreathingStateChangeFMOD.IsNull)
        {
            Debug.LogWarning("FMOD reference is Null");
        }
    }
}


