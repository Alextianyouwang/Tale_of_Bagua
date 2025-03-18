using UnityEngine;

public class CamWiggle : MonoBehaviour
{

    float _startEuler;
    void Start()
    {
        _startEuler = transform.eulerAngles.y;
    }

    void Update()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, _startEuler + Mathf.Sin(Time.time * 0.3f) * 10f, transform.eulerAngles.z);
    }
}
