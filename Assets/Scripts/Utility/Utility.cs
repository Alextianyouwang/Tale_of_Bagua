
using UnityEngine;

public static class Utility
{
    public static Vector3 GetScreenCenterPosition_WorldSpace()
    {
        Ray centerRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit[] hits = Physics.RaycastAll(centerRay, Mathf.Infinity, LayerMask.GetMask("MirrorPlane"));
        RaycastHit finalHit = new RaycastHit();
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.tag.Equals("MirrorPlane"))
                finalHit = hit;
        }

        return finalHit.point;
    }

    public static Vector2[] RadiusPosition(int numSlices)
    {
        Vector2[] dirs = new Vector2[numSlices];
        float inc = Mathf.PI * 2 / numSlices;
        for (int i = 0; i < numSlices; i++)
        {
            dirs[i] = new Vector2(Mathf.Sin(inc * i), Mathf.Cos(i * inc));
        }
        return dirs;
    }
}
