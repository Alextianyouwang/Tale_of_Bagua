using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaguaShow : MonoBehaviour
{
    float speed = 18f;

    private void OnEnable()
    {
        NextLine.OnToggleBagua += SetActive;
    }
    private void OnDisable()
    {
        NextLine.OnToggleBagua -= SetActive;

    }

    public void SetActive(bool set) 
    {
        gameObject.SetActive(set);
    }
    void Start()
    {
        SetActive(false);
    }
    void Update()
    {
        transform.eulerAngles = new Vector3(Time.time, Time.time * 2, Time.time * 3) *speed;
    }
}
