using TriInspector;
using UnityEngine;
public class LazerEmitter : Lazer, IInteractable
{
    [ReadOnly]
    public Orientation OrientationOptions;
    [Button]
    public void UpdateOriantation()
    {
        OrientationOptions = DirectionHelper.NextOriantation(OrientationOptions, 1);
        DirectionHelper.Editor_ChangeOriantationUI(OrientationOptions,VisualCueUI);
    }
    public void DetactPlayer() { }


    public void Interact(Vector3 pos)
    {
        _path.Add(this);
        RationalObject hit;
        ShootLazer(80, 0.23f,OrientationOptions,out hit);
        ProcessChain(hit);
    }
    public void Hold() { }

    public void Disengage()
    {
        StopAllLazerInChain();
    }
    public bool IsVisible() => IsObjectVisibleAndSameLevelWithPlayer();
    public IconType GetIconType() => IconType.space;
    public Orientation GetIconOrientation() => Orientation.Top;

}
