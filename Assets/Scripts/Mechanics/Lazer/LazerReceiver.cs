using UnityEngine;

public class LazerReceiver : RationalObject
{
    public void BranchReceived(RationalObject ro) 
    {
        print("Receiver Hit, name: " + name);
    }
}
