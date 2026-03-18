using UnityEngine;

public class IKTargetFollowVRRig : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float turnSmoothness = 0.1f;
    [SerializeField] private VRMap head;
    [SerializeField] private VRMap leftHand;
    [SerializeField] private VRMap rightHand;

    [SerializeField] private Vector3 headBodyPositionOffset;
    [SerializeField] private float headBodyYawOffset;

    private void LateUpdate()
    {
        transform.position = head.IKtarget.position + headBodyPositionOffset;
        float yaw = head.vrTarget.eulerAngles.y;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, yaw, transform.eulerAngles.z), turnSmoothness);

        head.Map();
        leftHand.Map();
        rightHand.Map();
    }
}

[System.Serializable]
public class VRMap
{
    public Transform vrTarget;
    public Transform IKtarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public void Map()
    {
        IKtarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        IKtarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}
