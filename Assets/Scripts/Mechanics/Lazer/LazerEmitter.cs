using TriInspector;
using UnityEngine;
public class LazerEmitter : Lazer, IInteractable
{
    [ReadOnly]
    public Lazer_OriantationHelper.Orientation OriantationOptions;
    [Button]
    public void UpdateOriantation()
    {
        OriantationOptions = Lazer_OriantationHelper.NextOriantation(OriantationOptions);
        Lazer_OriantationHelper.Editor_ChangeOriantationUI(OriantationOptions,VisualCueUI);
    }

    public void Interact(Vector3 pos)
    {
        RationalObject hit;
        ShootLazer(80, 0.23f, Lazer_OriantationHelper.GetDirection(OriantationOptions),out hit);
        ProcessChain(hit);
    }
    public void Hold() { }

    public void Disengage()
    {
        _path?.Clear();
        RecursivelyStop();
    }
    public bool IsVisible() => IsObjectVisibleAndSameLevelWithPlayer();
    public IconType GetIconType() => IconType.kavaii;

}
