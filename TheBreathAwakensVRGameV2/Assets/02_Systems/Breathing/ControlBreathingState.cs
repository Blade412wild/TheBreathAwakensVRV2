using UnityEngine;
using UnityEngine.InputSystem;

public class ControlBreathingState : MonoBehaviour
{
    [SerializeField] private InputActionReference reference;
    [SerializeField] private BreathingDeviceData deviceData;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        reference.action.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        DecideBreathingState();
    }

    private void DecideBreathingState()
    {

        float inExhaleVelocity = deviceData.inExhaleSpeed;
        var spacebarButtonValue = reference.action.ReadValue<float>();

        if (spacebarButtonValue == 1 && inExhaleVelocity > 0)
        {
            deviceData.BreathingState = BreathingState.inhaling;
        }
        else if (spacebarButtonValue == 0 && inExhaleVelocity > 0)
        {
            deviceData.BreathingState = BreathingState.exhaling;

        }
        else
        {
            deviceData.BreathingState = BreathingState.holdingBreath;
        }


    }
}
