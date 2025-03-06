using TriInspector;
using UnityEngine;
public class LazerSubEmitter : Lazer, IInteractable
{
    [ReadOnly]
    public Lazer_OriantationHelper.Orientation OriantationOptions;
    [Button]
    public void UpdateOriantation()
    {
        OriantationOptions = Lazer_OriantationHelper.NextOriantation(OriantationOptions);
        Lazer_OriantationHelper.Editor_ChangeOriantationUI(OriantationOptions, VisualCueUI);
        VisualCueUI.transform.localScale = Vector3.one * 0.5f;
    }
    protected new void OnEnable()
    {
        OnReceive += BranchReceived;
    }
    public void BranchReceived(RationalObject ro)
    {
        RationalObject hit;
        ShootLazer(80, 0.23f, Lazer_OriantationHelper.GetDirection(OriantationOptions), out hit);
        ProcessChain(hit);
    }
   
    public void Interact(Vector3 pos)
    {
        UpdateOriantation();
    }

    public void Hold() { }
    public void Disengage() {}
    public bool IsVisible() => IsObjectVisibleAndSameLevelWithPlayer();
    public IconType GetIconType() => IconType.kavaii;

}
