using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandObjectSwapFeature : MonoBehaviour
{
    [SerializeField] private InputActionProperty buttonReference;

    [SerializeField] private GameObject[] objects;

    private GameObject currentObject;
    private int counter = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonReference.action.Enable();
        buttonReference.action.started += HandleButtonPress;
        currentObject = objects[0];
    }

    private void HandleButtonPress(InputAction.CallbackContext context)
    {
        HandleObjectActiveState();

    }


    private void HandleObjectActiveState()
    {
        GameObject previousObject = currentObject;
        currentObject = GetNextObject();

        if (previousObject != null)
            previousObject.SetActive(false);

        if (currentObject != null)
            currentObject.SetActive(true);
    }
    private GameObject GetNextObject()
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

        Debug.Log("get : " + objects[counter].name);
        return objects[counter];
    }





}
