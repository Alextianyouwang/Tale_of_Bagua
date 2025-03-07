using TriInspector;
using UnityEngine;
public class LazerReflector : Lazer, IInteractable
{
    [ReadOnly]
    public Lazer_Helper.ReflectionDirection ReflectionOptions;
    [Button]
    public void UpdateOriantation()
    {
        ReflectionOptions = Lazer_Helper.NextReflectionDirection(ReflectionOptions);
        Lazer_Helper.Editor_ChangeReflectionUI(ReflectionOptions, VisualCueUI);
    }
    protected new void OnEnable()
    {
        OnReceive += BranchReceived;
    }
    public void BranchReceived(RationalObject ro)
    {
        Lazer_Helper.Orientation o = Lazer_Helper.GetReflectedDirection(ReflectionOptions, _upstreamEmitter.Orientation);
        RationalObject hit;
        ShootLazer(80, 0.23f,o, out hit);
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
