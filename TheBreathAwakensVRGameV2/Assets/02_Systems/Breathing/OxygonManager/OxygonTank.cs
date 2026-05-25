using FMODUnity;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class OxygonTank : MonoBehaviour
{
    [SerializeField] private float TankSize; // in Liters

    public float AvailableOxygon { get; private set; } //  in Liters
    public float MaxVolume { get; private set; } // in Liters
    public bool IsEmpty { get; private set; }


    private const int TankPressure = 200; // in bar
    private const float Pi = 3.14159f;
    private float noseRadius = 0.004f; // m //TODO this needs to be calibrated

    /*
     * formulas
     * 
     *  Available Air
     *   Va = Vt * P;
     *  
     *   Va = Available Air (L)
     *   Vt = Tank Volume (L)
     *   P  = Pressure (P)
     *  
     *  Flowrate
     *   Q = A * V
     *  
     *   Q = flow rate (m3/s)
     *   A = nostril cross-sectional area (m2)
     *   v = air velocity (m/s)
     *  
     *  radius to surfaceArea (m2)
     *   A  = Pi * r2
     *  
     *   pi = 3.14159
     *   A  = surfaceArea (m2)
     *   r  = radius (m)
     *  
     * 
     */




    public void UseOxygonTank(float airVelocity)
    {
        if (IsEmpty) return;

        float volumeUsed = CalculateOxygonUsed(airVelocity);

        AvailableOxygon -= volumeUsed;

        if (AvailableOxygon <= 0)
        {
            AvailableOxygon = 0;
            IsEmpty = true;
        }
        else
        {
            IsEmpty = false;
        }
    }

    public void Refill(float amount)
    {
        AvailableOxygon += amount;

        if (AvailableOxygon >= TankSize)
        {
            AvailableOxygon = TankSize;
        }

        IsEmpty = false;
    }


    private float CalculateOxygonUsed(float speed)
    {
        float volumeSpeed = CalculateBreathingFlowrate(speed);
        return volumeSpeed * Time.deltaTime;
    }

    private float CalculateBreathingFlowrate(float speed)
    {
        //TODO CalculateVolumeSpeedPerSecond

        // Flowrate

        return 0;
    }

   

}

public class OxygonMask : MonoBehaviour
{

    [SerializeField] private bool active;

    public bool isOnhead { get; private set; }

    private float glassIntegrity;
    private float tubeIntergrity;

}

public class SCBA : MonoBehaviour
{
    [Header("References")]
    // SCBA = Self-Contained breathing Apparatus 
    [SerializeField] private OxygonTank tank;
    [SerializeField] private OxygonMask mask;
    [SerializeField] private BreathingDeviceData breathingData;

    [Header("FMOD")]
    [SerializeField] private EventReference BreathingStateChangeFMOD;

    private BreathingState previousBreathingState;
    private BreathingState currentBreathingState;
    [SerializeField] private bool mayUpdate;

    private void Start()
    {
        previousBreathingState = BreathingState.holdingBreath;
        currentBreathingState = BreathingState.holdingBreath;

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
        }
    }

    private void HandleInhaling()
    {

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
}

public class SCBASmartWatch : MonoBehaviour
{




    public void UpdateOxygonUI()
    {

    }

    public void UpdateTimerUI()
    {

    }
}

