using System;
using UnityEngine;

public class TriggerArea : MonoBehaviour
{
    public event Action OnTargetEnteredTriggerEvent;
    public event Action OnTargetExitTriggerEvent;
    private Collider targetCollider;

    public void Set(Collider targetCollider)
    {
        this.targetCollider = targetCollider;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == targetCollider)
        {
            //Debug.Log("entered trigger : " + other.name);
            OnTargetEnteredTriggerEvent?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == targetCollider)
        {
            //Debug.Log("entered trigger : " + other.name);
            OnTargetExitTriggerEvent?.Invoke();
        }
    }
}
