using UnityEngine;

public class LazerReceiver : RationalObject
{
    public void BranchReceived(LazerEmitter.Oriantations oriantation) 
    {
        print("Receiver Hit, name: " + name);
    }
}
