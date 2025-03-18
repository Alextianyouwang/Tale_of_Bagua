using TriInspector;
using UnityEngine;
public class LazerReflector : Lazer, IInteractable
{
    [ReadOnly]
    public DirectionHelper.ReflectionDirection ReflectionOptions;
    [Button]
    public void UpdateOriantation()
    {
        ReflectionOptions = DirectionHelper.NextReflectionDirection(ReflectionOptions);
        DirectionHelper.Editor_ChangeReflectionUI(ReflectionOptions, VisualCueUI);
    }
    protected new void OnEnable()
    {
        OnReceive += BranchReceived;
    }
    public void BranchReceived(RationalObject ro)
    {
        Orientation o = DirectionHelper.GetReflectedDirection(ReflectionOptions, _upstreamEmitter.Orientation);
        RationalObject hit;
        ShootLazer(80, 0.23f,o, out hit);
        ProcessChain(hit);
    }
   
    public void Interact(Vector3 pos)
    {
        UpdateOriantation();
    }
    public void DetactPlayer() { }
    public Orientation GetIconOrientation() => Orientation.Top;
    public void Hold() { }
    public void Disengage() {}
    public bool IsVisible() => IsObjectVisibleAndSameLevelWithPlayer();
    public IconType GetIconType() => IconType.space;

}
