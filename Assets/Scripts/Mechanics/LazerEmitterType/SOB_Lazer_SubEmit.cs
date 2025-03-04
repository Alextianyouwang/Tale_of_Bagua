using UnityEngine;
[CreateAssetMenu (menuName = "Lazer/SubEmitType")]
public class SOB_Lazer_SubEmit : SOVB_LazerObjectBase
{
    public enum Orientation { Top, Right, Bot, Left }
    public bool Reflector = false;

     public Vector3 CreateEulerAngle(Orientation OriantationOptions)
    {

        Vector3 eularAngle = Vector3.zero;
        switch (OriantationOptions)
        {
            case Orientation.Top:
                eularAngle = new Vector3(0, 180, 0);
                break;
            case Orientation.Bot:
                eularAngle = new Vector3(0, 0, 0);
                break;
            case Orientation.Left:
                eularAngle = new Vector3(0, 90, 0);
                break;
            case Orientation.Right:
                eularAngle = new Vector3(0, 270, 0);
                break;
        }
        return eularAngle;
    }
    public Vector3 CreateDirection(Orientation OriantationOptions) 
    {
        Vector3 direction = Vector3.zero;
        switch (OriantationOptions)
        {
            case Orientation.Top:
                direction = Vector3.forward;
                break;
            case Orientation.Bot:
                direction = Vector3.back;
                break;
            case Orientation.Left:
                direction = Vector3.left;
                break;
            case Orientation.Right:
                direction = Vector3.right;
                break;
        }
        return direction;
    }

    public int LoopOriantation(int value) 
    {
        int currentOriaintation = value;
        currentOriaintation += 1;
        currentOriaintation %= 4;
        return currentOriaintation;
    }
}
