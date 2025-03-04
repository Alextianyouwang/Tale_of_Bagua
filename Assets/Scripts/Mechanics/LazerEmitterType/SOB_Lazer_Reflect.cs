using UnityEngine;

[CreateAssetMenu(menuName = "Lazer/ReflectType")]
public class SOB_Lazer_Reflect :SOVB_LazerObjectBase
{
    public enum Type { SubEmit, Reflect }
    public Type InteractionOptions;
    public bool SwapReflectAxis = false;
}
