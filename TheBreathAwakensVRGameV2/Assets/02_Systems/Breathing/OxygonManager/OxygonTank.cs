using UnityEngine;
using UnityEngine.Rendering;

public class OxygonTank : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float targetMinutes;
    [SerializeField] private float targetSeconds;

    [SerializeField] private float averageSpeedperCycle;
    [SerializeField] private float multiplier;

    [SerializeField] private float TankSize; // in Liters




    public float AvailableOxygon { get; private set; } //  in Liters
    public float MaxVolume { get; private set; } // in Liters
    public bool IsEmpty { get; private set; } //= false;
    public float AvailableOxygonPercentage { get; private set; }
    public float SurfaceArea { get; private set; }

    public float TargetTime { get; private set; }


    private const int TankPressure = 200; // in bar
    private const float Pi = Mathf.PI;
    private float noseRadius = 0.004f; // m //TODO this needs to be calibrated

    //[Header("testing")]
    //public float TEST = 1000;
    //public float Amin;
    //public float Amax;
    //public float Bmin;
    //public float Bmax;
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

    public void Setup()
    {
        SurfaceArea = Pi * Mathf.Pow(noseRadius, 2) * 2; // *2 is because of the 2 noseholes 
        //CalculateVolumeBasedOnTimeAndSample(averageSpeedperCycle);
        MaxVolume = 1000.0f;
        AvailableOxygon = MaxVolume;
        TargetTime = GetTargetTime();
    }

    public void UpdateOxygonTankVolume(float recordedflowRate)
    {
        MaxVolume = recordedflowRate * TargetTime;
        AvailableOxygon = MaxVolume;

    }

    public void UpdatePercentage()
    {

        CalculatePercentage();
    }

    public void UseOxygonTank(float airVelocity, float time)
    {
        if (IsEmpty) return;

        float oxygonUsed = CalculateOxygonUsed(airVelocity, time);
        AvailableOxygon -= oxygonUsed;
        //Debug.Log("oxygon Left : " + AvailableOxygon);

        if (AvailableOxygon <= 0)
        {
            AvailableOxygon = 0;
            IsEmpty = true;
        }
        else
        {
            IsEmpty = false;
        }

        CalculatePercentage();
    }

    public void Refill()
    {
        AvailableOxygon = MaxVolume;
        IsEmpty = false;
    }

    public void Refill(float amount)
    {
        AvailableOxygon += amount;

        if (AvailableOxygon >= MaxVolume)
        {
            AvailableOxygon = MaxVolume;
        }

        IsEmpty = false;
    }

    public float CalculateOxygonUsed(float velocity, float time)
    {
        float volumeSpeed = CalculateBreathingFlowrate(velocity); // in m3/s
        float volumeSpeedLiters = volumeSpeed * 1000;

        float oxygonUsed = volumeSpeedLiters * time;

        //Debug.Log("volumeSpeed = " + volumeSpeed + " m3/s | volumeSpeedLiters : " + volumeSpeedLiters + " L/s" + " | OxygonUsed : " + oxygonUsed);
        return oxygonUsed;
    }

    public float CalculateBreathingFlowrate(float velocity)
    {
        // Flowrate
        //  Q = A * V
        float flowrate = SurfaceArea * velocity;


        return flowrate;
    }

    private void CalculateMaxAvailableOxygon()
    {
        MaxVolume = TankSize * TankPressure;
    }

    private void CalculatePercentage()
    {
        if (AvailableOxygon > 0)
        {
            AvailableOxygonPercentage = Mapping.MapValue(AvailableOxygon, 0, MaxVolume, 0, 100);
        }
        else
        {
            AvailableOxygonPercentage = 0.0f;
        }
        //float test = Mapping.MapValue(TEST, Amin, Amax, Bmin, Bmax);
        //Debug.Log("test percentage : " + test);
    }

    private float GetTargetTime()
    {
        float seconds = targetSeconds;
        seconds += targetMinutes * 60;
        return seconds;
    }

    private void CalculateVolumeBasedOnTimeAndSample(float averageSpeed)
    {
        float targetTime = GetTargetTime();
        float flowRate = CalculateBreathingFlowrate(averageSpeed) * 1000.0f;
        MaxVolume = flowRate * targetTime;
        MaxVolume *= multiplier;

        //float flowRatePerCycle = flowRate / timeDuration;

    }
}


