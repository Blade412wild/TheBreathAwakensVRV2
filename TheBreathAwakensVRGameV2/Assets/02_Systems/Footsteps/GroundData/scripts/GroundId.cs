using UnityEngine;

public class GroundId : MonoBehaviour
{
    [SerializeField] private GroundType type;

    public GroundType GetGroundType()
    {
        return type;
    }
}

