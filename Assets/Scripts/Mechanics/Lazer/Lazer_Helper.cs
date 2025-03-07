using UnityEngine;

public class Lazer_Helper 
{
    public enum Orientation { Top, Right, Bot, Left }
    public enum ReflectionDirection { ForSlash, BackSlash}
    public static void Editor_ChangeOriantationUI(Orientation orientationOptions, GameObject visual)
    {
        if (!visual)
            return;
        Vector3 eularAngle = Vector3.zero;
        switch (orientationOptions)
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
    public static void Editor_ChangeReflectionUI(ReflectionDirection option, GameObject visual)
    {
        if (!visual)
            return;
        Vector3 eularAngle = Vector3.zero;
        switch (option)
        {
            case ReflectionDirection.ForSlash:
                eularAngle = new Vector3(90, 0, 0);
                break;
            case ReflectionDirection.BackSlash:
                eularAngle = new Vector3(90, 90, 0);
                break;
        }
        visual.transform.eulerAngles = eularAngle;
    }
    public static Vector3 GetDirection(Orientation orientationOptions)
    {
        Vector3 direction = Vector3.zero;
        switch (orientationOptions)
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
    public static Orientation NextOriantation(Orientation orientationOptions, int stride)
    {
        int currentOrieintation = (int)orientationOptions;
        currentOrieintation += stride;
        currentOrieintation %= 4;
        return (Orientation)currentOrieintation;
    }

    public static ReflectionDirection NextReflectionDirection(ReflectionDirection reflectionDir)
    {
        int currentOrieintation = (int)reflectionDir;
        currentOrieintation++;
        currentOrieintation %= 2;
        return (ReflectionDirection)currentOrieintation;
    }
    public static Orientation GetReflectedDirection(ReflectionDirection option, Orientation inOriantation) 
    {
        int o = (int)inOriantation;
        int offset = (int)option * 2 - 1;
        o = o % 2 != 0 ? o + offset : o - offset;
        o = o < 0 ? o + 4 : o;
        o %= 4;
        return (Orientation)o;
    }
}
