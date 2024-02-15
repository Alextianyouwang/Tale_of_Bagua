using UnityEngine;

public class LazerReceiver : RationalObject
{
    protected override void OnEnable()
    {
        base.OnEnable();
        OnReceive += BranchReceived;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }
    public void BranchReceived() 
    {
        print("Receiver Hit, name: " + name);
    }
}
