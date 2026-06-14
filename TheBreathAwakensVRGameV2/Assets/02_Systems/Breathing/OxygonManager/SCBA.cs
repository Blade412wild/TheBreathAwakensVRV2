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
    [SerializeField] private BreathingSystem breathingSystem;
    [SerializeField] private ExtractionManager extractionManager;
    //[SerializeField] private SampleToInputConverter SampleToInputConverter;
    //[SerializeField] private In_ExhaleSpeedDataReceived received;

    [Header("FMOD")]
    [SerializeField] private EventReference BreathingStateChangeFMOD;


    public float InExhaleSpeed => mask.CurrentInExhaleSpeed;
    public BreathingState BreathingState => mask.CurrentBreathingState;

    private BreathingSystemManager breathingSystemManager;
    private OxygonPrediction oxygonPrediction;

    private void Start()
    {
        CheckReferences();
        //breathingSystemManager = new BreathingSystemManager(messageFinishedReceived, breathingData, maxCyclesPerSample);
        //breathingSystemManager.Activate();

        breathingSystem.OnStart();
        watch.UpdateOxygonPercentageUI(tank.AvailableOxygon, tank.AvailableOxygonPercentage);
        breathingSystem.OxygonPredictionDoneEvent += (x) => watch.UpdateOxygonEstimation(x);
        extractionManager.UpdateVisualTimerEvent += (x) => watch.UpdateExtraction(x);

        mask.EquipEvent += HandleGaskMaskEquipEvent;
        mask.UnequipEvent += HandleGaskMaskUnequipEvent;

        //received.OnDataReceivedEvent += Test;
    }

    private void Update()
    {
        if (!mayUpdate) return;
        RunSCBASystem();
    }

    private void OnDisable()
    {
        breathingSystem.OnDeactivate();
        //breathingSystemManager.OnDisable();
        breathingSystem.OxygonPredictionDoneEvent -= (x) => watch.UpdateOxygonEstimation(x);
        extractionManager.UpdateVisualTimerEvent -= (x) => watch.UpdateExtraction(x);
        mask.EquipEvent -= HandleGaskMaskEquipEvent;
        mask.UnequipEvent -= HandleGaskMaskUnequipEvent;
    }

    private void RunSCBASystem()
    {
        //breathingSystemManager.OnUpdate();
        breathingSystem.OnUpdate();

        if (breathingData.BreathingState == BreathingState.inhaling)
        {
            HandleInhaling();
            watch.UpdateOxygonPercentageUI((int)tank.AvailableOxygon, (int)tank.AvailableOxygonPercentage);
        }

    }

    private void HandleInhaling()
    {
        //tank.UseOxygonTank(breathingData.inExhaleSpeed);

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

    private void HandleGaskMaskEquipEvent()
    {
        Debug.Log("handle equip");
        messageFinishedReceived.OnDataReceivedEvent += UpdateMask;
    }

    private void HandleGaskMaskUnequipEvent()
    {
        Debug.Log("handle unequip");
        messageFinishedReceived.OnDataReceivedEvent -= UpdateMask;
    }

    private void UpdateMask()
    {
        Debug.Log("updating mask");
        mask.OnUpdate();
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


