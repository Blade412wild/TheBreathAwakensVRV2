using UnityEngine;
using UnityEngine.InputSystem;


public class AnimateHands : MonoBehaviour
{
    [SerializeField] private InputActionReference triggerValue;
    [SerializeField] private InputActionReference gripValue;

    [SerializeField] private Animator handAnimator;

    private void Start()
    {
        //triggerValue.action.Enable();
        //reference.action.Enable();
        //gripValue.action.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        float trigger = triggerValue.action.ReadValue<float>();

        float grip = gripValue.action.ReadValue<float>();

        handAnimator.SetFloat("Trigger", trigger);
        handAnimator.SetFloat("Grip", grip);

    }
}
