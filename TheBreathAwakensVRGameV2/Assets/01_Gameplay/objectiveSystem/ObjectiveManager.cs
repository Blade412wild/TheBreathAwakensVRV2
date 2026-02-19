using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ObjectiveManager : MonoBehaviour
{

    [SerializeField] private List<ObjectiveBaseClass> objectives;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(ObjectiveBaseClass objective in objectives)
        {
            objective.Init();
        } 
    }

    // Update is called once per frame
    void Update()
    {

    }
}
