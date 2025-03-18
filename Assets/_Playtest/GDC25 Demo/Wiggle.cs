using UnityEngine;

public class Wiggle : MonoBehaviour
{
    Vector3 _startTransform;
    private void Start()
    {
       _startTransform =  transform.position;
    }
    private void Update()
    {
      transform.position = _startTransform +  new Vector3(0,Mathf.PerlinNoise(Time.time * 0.05f + transform.position.x*2, Time.time *0.11f+ transform.position.z*2),0) * 1.2f ;
    }
}
