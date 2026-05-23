using UnityEngine;

public class TriggerAreaDebug : MonoBehaviour
{
    [SerializeField] TriggerArea triggerArea;
    [SerializeField] Renderer renderer;

    [SerializeField] Material enteredMaterial;
    [SerializeField] Material ExitMaterial;

    private void Start()
    {
        triggerArea.OnTargetEnteredTriggerEvent += HandleEnteredTriggerEvent;
        triggerArea.OnTargetExitTriggerEvent += HandleExitTriggerEvent;
        HandleExitTriggerEvent();

    }

    private void HandleEnteredTriggerEvent()
    {
        if(enteredMaterial == null)
        {
            Debug.LogWarning("enter material is null");
            return;
        }
        

        ChangeMaterial(enteredMaterial);
    }

    private void HandleExitTriggerEvent()
    {
        if (enteredMaterial == null)
        {
            Debug.LogWarning("exit material is null");
            return;
        }

        ChangeMaterial(ExitMaterial);
    }
    private void ChangeMaterial(Material material)
    {
        renderer.material = material;
    }

}
