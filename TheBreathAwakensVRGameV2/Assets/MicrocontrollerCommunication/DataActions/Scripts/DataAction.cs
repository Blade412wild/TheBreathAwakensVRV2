using UnityEngine;

public class DataAction : MonoBehaviour
{
    [SerializeField] protected BreathingDeviceData data;
    public virtual void OnDataReceived(string value)
    {

    }

}
