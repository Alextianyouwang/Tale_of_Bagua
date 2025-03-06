using UnityEngine;

public class Lazer_OriantationHelper 
{
    public enum Orientation { Top, Right, Bot, Left }
    public static void Editor_ChangeOriantationUI(Orientation oriantationOptions, GameObject visual)
    {
        if (!visual)
            return;
        Vector3 eularAngle = Vector3.zero;
        switch (oriantationOptions)
        {
            case Orientation.Top:
                eularAngle = new Vector3(90, 180, 0);
                break;
            case Orientation.Bot:
                eularAngle = new Vector3(90, 0, 0);
                break;
            case Orientation.Left:
                eularAngle = new Vector3(90, 90, 0);
                break;
            case Orientation.Right:
                eularAngle = new Vector3(90, 270, 0);
                break;
        }
        visual.transform.eulerAngles = eularAngle;
    }
    public static Vector3 GetDirection(Orientation oriantationOptions)
    {
        Vector3 direction = Vector3.zero;
        switch (oriantationOptions)
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
    public static Orientation NextOriantation(Orientation oriantationOptions)
    {
        int currentOriaintation = (int)oriantationOptions;
        currentOriaintation += 1;
        currentOriaintation %= 4;
        return (Orientation)currentOriaintation;

    }
}
