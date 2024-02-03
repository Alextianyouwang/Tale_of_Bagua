using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Obj;
    private void Awake()
    {
        if (!Obj)
            return;
        Instantiate(Obj, Vector3.zero, Quaternion.identity);
    }


}
