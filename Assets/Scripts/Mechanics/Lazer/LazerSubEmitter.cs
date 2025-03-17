using TriInspector;
using UnityEngine;
public class LazerSubEmitter : Lazer, IInteractable
{
    [ReadOnly]
    public Lazer_Helper.Orientation OrientationOptions;
    [Button]
    public void UpdateOriantation()
    {
        OrientationOptions = Lazer_Helper.NextOriantation(OrientationOptions, 1);
        Lazer_Helper.Editor_ChangeOriantationUI(OrientationOptions, VisualCueUI);
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

    public void Hold() { }
    public void Disengage() {}
    public bool IsVisible() => IsObjectVisibleAndSameLevelWithPlayer();
    public IconType GetIconType() => IconType.kavaii;

}
