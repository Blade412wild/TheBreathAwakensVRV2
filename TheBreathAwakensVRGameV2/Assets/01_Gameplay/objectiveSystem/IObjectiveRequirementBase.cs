using System;
using UnityEngine;

public interface IObjectiveRequirementBase
{
    public event Action<IObjectiveRequirementBase> RequirementCompletedEvent;

    public bool IsCompleted { get; set; }
    public void InitRequirements();

    public void RequirementCompleted();

}
