using UnityEngine;

public class OxygonTank : MonoBehaviour
{
    [SerializeField] private float TankSize; // in Liters

    public float AvailableOxygon { get; private set; } //  in Liters
    public float MaxVolume { get; private set; } // in Liters
    public bool IsEmpty { get; private set; }
    public float AvailableOxygonPercentage { get; private set; }


    private const int TankPressure = 200; // in bar
    private const float Pi = Mathf.PI;
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

    public void Setup()
    {
        CalculateMaxAvailableOxygon();
        AvailableOxygon = MaxVolume;

    }



    public void UseOxygonTank(float airVelocity)
    {
        if (IsEmpty) return;

        float oxygonUsed = CalculateOxygonUsed(airVelocity);

        AvailableOxygon -= oxygonUsed;

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

    public void Refill(float amount)
    {
        AvailableOxygon += amount;

        if (AvailableOxygon >= MaxVolume)
        {
            AvailableOxygon = MaxVolume;
        }

        IsEmpty = false;
    }


    private float CalculateOxygonUsed(float velocity)
    {
        float volumeSpeed = CalculateBreathingFlowrate(velocity);
        return volumeSpeed * Time.deltaTime;
    }

    private float CalculateBreathingFlowrate(float velocity)
    {
        // radius to surfaceArea(m2)
        //  A = Pi * r2
        float surfaceArea = Pi * Mathf.Pow(noseRadius, 2);

        // Flowrate
        //  Q = A * V
        float flowrate = surfaceArea * velocity;


        return flowrate;
    }

    private void CalculateAvailableAir()
    {
        // Available Air
        //  Va = Vt * P;

    }

    private void CalculateMaxAvailableOxygon()
    {
        MaxVolume = TankSize * TankPressure;
    }

    private void CalculatePercentage()
    {
        AvailableOxygonPercentage = Mapping.MapValueClamped(AvailableOxygon, MaxVolume, 0, 100, 0);
    }
}


