using AYellowpaper;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectiveBaseClass : MonoBehaviour
{
    public Action ObjectiveCompletedEvent;

    [SerializeField] protected ObjectiveManager manager;

    [SerializeField] protected List<InterfaceReference<IObjectiveRequirementBase>> activeRequirements;
    private InterfaceReference<IObjectiveRequirementBase> reference = new();

    protected List<IObjectiveRequirementBase> inactiveRequirements = new List<IObjectiveRequirementBase>();


    public void Init()
    {
        float test = 0;
        SetupRequirements();

    }
    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    public abstract void ObjectiveCompleted();

    private void SetupRequirements()
    {
        Debug.Log(transform.name + " subsribe");
        foreach (InterfaceReference<IObjectiveRequirementBase> reference in activeRequirements)
        {
            reference.Value.RequirementCompletedEvent += RequirementCompleted;
            reference.Value.InitRequirements();
        }

    }

    private void UnsubscribeToEvents()
    {
        foreach (InterfaceReference<IObjectiveRequirementBase> reference in activeRequirements)
        {
            if (reference.Value == null) continue;
            reference.Value.RequirementCompletedEvent -= RequirementCompleted;
        }
    }

    private void RequirementCompleted(IObjectiveRequirementBase requirement)
    {
        reference.Value = requirement;

        activeRequirements.Remove(reference);
        inactiveRequirements.Add(requirement);

        if (activeRequirements.Count == 0)
            ObjectiveCompleted();
    }



}
