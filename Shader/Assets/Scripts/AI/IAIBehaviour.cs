public interface IAIBehaviour
{
    void Initialize(object controller);
    void Tick();
    void FixedTick();
    void OnStateChanged(int oldState, int newState);
}
