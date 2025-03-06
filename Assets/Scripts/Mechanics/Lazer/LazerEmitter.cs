using TriInspector;
using UnityEngine;

public class LazerEmitter : Lazer, IInteractable
{
    [ReadOnly]
    public Orientation OriantationOptions;
    [Button]
    public void UpdateOriantation()
    {
        OriantationOptions = NextOriantation(OriantationOptions);
        Editor_ChangeOriantationUI(OriantationOptions);
    }
    protected override void Editor_ChangeOriantationUI(Orientation oriantation)=> base.Editor_ChangeOriantationUI(oriantation);

    public void Interact(Vector3 pos)
    {
        RationalObject hit;
        ShootLazer(80, 0.23f, GetDirection(OriantationOptions),out hit);
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
