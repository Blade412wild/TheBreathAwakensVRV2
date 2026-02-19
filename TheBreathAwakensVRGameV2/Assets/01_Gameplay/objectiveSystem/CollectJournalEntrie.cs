using System;
using UnityEngine;

public class CollectJournalEntrie : ObjectiveBaseClass, IObjectiveRequirementBase
{

    public event Action<IObjectiveRequirementBase> RequirementCompletedEvent;

    public bool IsCompleted { get; set; }

    public void InitRequirements()
    {
        Debug.Log(transform.name + "require init");
        IsCompleted = false;
        Init();

    }

    public override void ObjectiveCompleted()
    {
        Debug.Log(transform.name + " : Objective completed 2");
        RequirementCompletedEvent?.Invoke(this);
    }

    public void RequirementCompleted()
    {
        IsCompleted = true;
        RequirementCompletedEvent?.Invoke(this);
    }
}
