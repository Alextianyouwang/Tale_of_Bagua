using TriInspector;
using UnityEngine;
public class LazerSubEmitter : Lazer, IInteractable
{
    [ReadOnly]
    public Orientation OrientationOptions;
    [Button]
    public void UpdateOriantation()
    {
        OrientationOptions = DirectionHelper.NextOriantation(OrientationOptions, 1);
        DirectionHelper.Editor_ChangeOriantationUI(OrientationOptions, VisualCueUI);
        VisualCueUI.transform.localScale = Vector3.one * 0.5f;
    }
    protected new void OnEnable()
    {
        OnReceive += BranchReceived;
    }
    public void BranchReceived(RationalObject ro)
    {
        RationalObject hit;
        ShootLazer(80, 0.23f, OrientationOptions, out hit);
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
