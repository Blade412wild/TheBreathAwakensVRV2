public interface ISwapable
{
    public bool SwapableItemActiveState { get; set; }

    void Activate();
    void Deactivate();
}
