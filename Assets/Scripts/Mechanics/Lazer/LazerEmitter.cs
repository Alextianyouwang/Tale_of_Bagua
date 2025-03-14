using TriInspector;
using UnityEngine;
public class LazerEmitter : Lazer, IInteractable
{
    [ReadOnly]
    public Lazer_Helper.Orientation OrientationOptions;
    [Button]
    public void UpdateOriantation()
    {
        OrientationOptions = Lazer_Helper.NextOriantation(OrientationOptions, 1);
        Lazer_Helper.Editor_ChangeOriantationUI(OrientationOptions,VisualCueUI);
    }

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
    public IconType GetIconType() => IconType.kavaii;

}
