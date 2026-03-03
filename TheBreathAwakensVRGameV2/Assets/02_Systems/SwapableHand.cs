using UnityEngine;

public class SwapableHand : MonoBehaviour, ISwapable
{
    public bool SwapableItemActiveState { get ; set ; }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

}
