using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class FeetSim : MonoBehaviour
{
    public event Action<Vector3> HitTerrainEvent;
    public event Action HitGameobjectEvent;

    [SerializeField] private float timeBetweenStep = 1;
    [SerializeField] private float groundDistance;
    [SerializeField] private TerrainCollider targetCollder;

    [Space]
    [SerializeField] private bool update;
    private float currentTime;
    private Vector3 defaultPoint = new Vector3(0, 1000000, 0);


    private void Update()
    {
        if (!update) return;
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        currentTime++;

        if (currentTime >= timeBetweenStep)
        {
            currentTime = 0;
            ShootRay();
        }
    }

    public void ShootRay()
    {

        Ray hit;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, groundDistance))
        {
            if (hitInfo.collider == targetCollder)
            {
                HitTerrainEvent?.Invoke(hitInfo.point);
            }
            else
            {
                Debug.Log("No terraincollider");

            }
        }
        else
        {
            Debug.Log("No htt");
        }
    }
}
