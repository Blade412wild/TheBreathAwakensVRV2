using AYellowpaper;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandObjectSwapFeature : MonoBehaviour
{
    [SerializeField] private InputActionProperty buttonReference;

    [SerializeField] private InterfaceReference<ISwapable>[] objects;
    [SerializeField] private bool Swap = false;

    private ISwapable currentObject;
    private int counter = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonReference.action.Enable();
        buttonReference.action.started += HandleButtonPress;
        currentObject = objects[0].Value;
    }

    private void Update()
    {
        if (Swap)
        {
            Swap = false;
            HandleObjectActiveState();
        }
    }

    private void HandleButtonPress(InputAction.CallbackContext context)
    {
        HandleObjectActiveState();

    }



    private void HandleObjectActiveState()
    {
        ISwapable previousObject = currentObject;
        currentObject = GetNextObject();

        if (previousObject != null)
            previousObject.Deactivate();

        if (currentObject != null)
            currentObject.Activate();
    }
    private ISwapable GetNextObject()
    {
        int nextState = counter + 1;
        if (nextState == objects.Length)
        {
            counter = 0;
        }
        else
        {
            counter++;
        }

        return objects[counter].Value;
    }





}
