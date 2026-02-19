using System;
using UnityEngine;

public class TouchObject : MonoBehaviour, IObjectiveRequirementBase
{
    [SerializeField] private Collider ownCollider;
    [SerializeField] private Collider[] hands;

    public event Action<IObjectiveRequirementBase> RequirementCompletedEvent;
    public bool IsCompleted { get;  set; }

    private void OnTriggerEnter(Collider other)
    {
        if (CheckIfcolliderIsHands(other))
            RequirementCompleted();
    }

    private bool CheckIfcolliderIsHands(Collider collider)
    {
        if (collider == hands[0] || collider == hands[1]) return true;
        return false;
    }

    public void RequirementCompleted()
    {
        RequirementCompletedEvent?.Invoke(this);
        Debug.Log(transform.name + " : requirement completed");
    }

    public void InitRequirements()
    {
        IsCompleted = false;
    }
}