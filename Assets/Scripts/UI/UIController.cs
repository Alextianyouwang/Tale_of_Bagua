using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public Slider slider;
    private void OnEnable()
    {
        MirrorManager.OnChargingCollapse += UpdateUICharge;
        MirrorManager.OnChargedCollapse += UpdateUIFinishedCharge;
    }

    private void OnDisable()
    {
        MirrorManager.OnChargingCollapse -= UpdateUICharge;
        MirrorManager.OnChargedCollapse -= UpdateUIFinishedCharge;

    }

    private void UpdateUICharge(float timer, float target) 
    {
        if (!slider)
            return;
        slider.value = timer / target;

    }

    private void UpdateUIFinishedCharge(float timer, float target)
    {
        if (!slider)
            return;

        slider.value = 0;
    }
}
