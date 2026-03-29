using System;
using UnityEngine;

public class Foot : MonoBehaviour
{
    public event Action<Vector3> HitTerrainEvent;
    public event Action<Vector3> HitObjectEvent;

    public enum FootType { Left, Right };
    public enum FootState { grounded, inAir }

    [SerializeField] private FootType side;
    [SerializeField] private FootState state;
    private FootState previousState;

    [SerializeField] private LayerMask walkableLayers;
    [SerializeField] private float groundDistance;
    [SerializeField] private Collider terrainCollider;

    private bool Grounded = true;
    private bool mayCheckGroundedState;
    private Vector3 velocity;
    private Vector3 previousPos;

    public void SetCurrentState(FootState newState)
    {
        state = newState;

        if (newState == previousState) return;
        previousState = newState;

        if (newState == FootState.grounded)
        {
            ShootRay();
        }
    }


    private void FixedUpdate()
    {
        if (mayCheckGroundedState)
        {
            CheckIfGrounded();
        }
    }

    private void CheckIfGrounded()
    {
        ShootRay();
    }
    public void ShootRay()
    {

        Ray hit;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, groundDistance, walkableLayers))
        {
            if (hitInfo.collider == terrainCollider)
            {
                HitTerrainEvent?.Invoke(hitInfo.point);
                Grounded = true;
                Debug.Log("Hit");
            }
            else
            {
                Debug.Log("No terraincollider");
                HitObjectEvent?.Invoke(hitInfo.point);
            }
        }
        else
        {
            Debug.Log("No htt");
        }
    }

}
