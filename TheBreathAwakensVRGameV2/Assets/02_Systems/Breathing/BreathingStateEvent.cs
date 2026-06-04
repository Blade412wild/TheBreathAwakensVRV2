using FMODUnity;
using UnityEngine;

public class BreathingStateEvent : MonoBehaviour
{
    [SerializeField] private EventReference BreathingStateChangeFMOD;
    [SerializeField] private BreathingDeviceData breathingDeviceData;

    private BreathingState previousBreathingState;

    private void Start()
    {
        previousBreathingState = BreathingState.holdingBreath;
    }


    // Update is called once per frame
    void Update()
    {
        HandleBreathingStateChange();
    }



    public void HandleBreathingStateChange()
    {
        if (breathingDeviceData.BreathingState != previousBreathingState)
        {
            previousBreathingState = breathingDeviceData.BreathingState;
            Debug.Log("Breathing state changed to : " + breathingDeviceData.BreathingState);

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
}
