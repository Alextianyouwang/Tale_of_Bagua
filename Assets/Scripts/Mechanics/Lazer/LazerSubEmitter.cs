using TriInspector;
using UnityEngine;
public class LazerSubEmitter : Lazer, IInteractable
{
    [ReadOnly]
    public Orientation OriantationOptions;
    [Button]
    public void UpdateOriantation()
    {
        OriantationOptions = NextOriantation(OriantationOptions);
        Editor_ChangeOriantationUI(OriantationOptions);
    }
    protected new void OnEnable()
    {
        OnReceive += BranchReceived;
    }
    private void OnGUI()
    {
        Editor_ChangeOriantationUI(OriantationOptions);
    }
    protected override void Editor_ChangeOriantationUI(Orientation oriantation)
    {
        base.Editor_ChangeOriantationUI(oriantation);
        VisualCueUI.transform.localScale = Vector3.one * 0.5f;
    }
    public void BranchReceived(RationalObject ro)
    {
        RationalObject hit;
        ShootLazer(80, 0.23f, GetDirection(OriantationOptions), out hit);
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
