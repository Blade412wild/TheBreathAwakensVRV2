using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class OxygonTank : MonoBehaviour
{
    [SerializeField] private float maxVolume;
    public float CurrentVolume { get; private set; }


    public float GetAir()
    {
        return 0;
    }

    public void Refill(float amount)
    {

    }

    private float CalculateVolume(float speed)
    {
        // 
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


    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void UpdateOxygon()
    {

    }




}
