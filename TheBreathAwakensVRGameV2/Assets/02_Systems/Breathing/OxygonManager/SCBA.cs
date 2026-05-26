using FMODUnity;
using UnityEngine;

public class SCBA : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private bool mayUpdate;

    [Header("References")]
    // SCBA = Self-Contained breathing Apparatus 
    [SerializeField] private OxygonTank tank;
    [SerializeField] private OxygonMask mask;
    [SerializeField] private SCBASmartWatch watch;
    [SerializeField] private BreathingDeviceData breathingData;

    [Header("FMOD")]
    [SerializeField] private EventReference BreathingStateChangeFMOD;

    private BreathingState previousBreathingState;
    private BreathingState currentBreathingState;

    

    private void Start()
    {
        previousBreathingState = BreathingState.holdingBreath;
        currentBreathingState = BreathingState.holdingBreath;
        CheckReferences();
        tank.Setup();
        watch.UpdateOxygonUI(tank.AvailableOxygon, tank.AvailableOxygonPercentage);

    }

    private void Update()
    {
        if (!mayUpdate) return;
        RunSCBASystem();
    }

    private void RunSCBASystem()
    {
        AnalyzeBreathing();

        if (currentBreathingState == BreathingState.inhaling)
        {
            HandleInhaling();
            watch.UpdateOxygonUI(tank.AvailableOxygon, tank.AvailableOxygonPercentage);
        }
    }

    private void HandleInhaling()
    {
        tank.UseOxygonTank(breathingData.inExhaleSpeed);
        
    }

    private void AnalyzeBreathing()
    {
        AnalyzeBreathingState();

    }

    private void AnalyzeBreathingState()
    {
        currentBreathingState = breathingData.BreathingState;
        if (currentBreathingState != previousBreathingState)
        {
            HandleBreathingStateChange();
        }
    }

    private void HandleBreathingStateChange()
    {
        previousBreathingState = currentBreathingState;
        Debug.Log("Breathing state changed to : " + currentBreathingState);

        if (!BreathingStateChangeFMOD.IsNull)
        {
            RuntimeManager.PlayOneShot(BreathingStateChangeFMOD);
        }
        else
        {
            Debug.LogWarning("didn;t reference for breathingStateChanged");
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


