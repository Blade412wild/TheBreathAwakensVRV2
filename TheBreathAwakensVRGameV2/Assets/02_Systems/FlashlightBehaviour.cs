using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightBehaviour : MonoBehaviour, ISwapable
{
    public event Action<bool> ChangeLightEvent;

    private enum flashLightStates { Off, On }
    private flashLightStates flashlightState = flashLightStates.Off;
    public bool SwapableItemActiveState { get; set; }

    [SerializeField] private Light lightSource;
    [SerializeField] private InputActionProperty flashlightActivateInput;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightSource.enabled = true;
    }


    public void Activate()
    {
        flashlightActivateInput.action.started += HandleFlashlightInputEvent;

        SwapableItemActiveState = true;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        flashlightActivateInput.action.started -= HandleFlashlightInputEvent;

        SwapableItemActiveState = false;
        gameObject.SetActive(false);
    }

    private void HandleFlashlightInputEvent(InputAction.CallbackContext context)
    {
        if(flashlightState == flashLightStates.On)
            ChangeLightState(false);

        if (flashlightState == flashLightStates.Off)
            ChangeLightState(true);
    }

    private void ChangeLightState(bool state)
    {
        if (state)
        {
            flashlightState = flashLightStates.On;
            lightSource.enabled = true;
            ChangeLightEvent?.Invoke(true);
        }
        else
        {
            flashlightState = flashLightStates.Off;
            lightSource.enabled = false;
            ChangeLightEvent?.Invoke(false);

        }

    }


}
