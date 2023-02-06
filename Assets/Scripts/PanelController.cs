using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    public GameObject[] Slides;
    int slideIndex = 0;
    float timeCounter = 0f;

    private void Start()
    {

    }

    public void Update()
    {
        timeCounter -= Time.deltaTime;
        if(timeCounter < 0)
        {
            Slides[slideIndex-1<0?0: slideIndex -1].SetActive(false);
            if (slideIndex <= Slides.Length-1) 
                Slides[slideIndex].SetActive(true);
            if (slideIndex == Slides.Length )
                gameObject.SetActive(false);

            slideIndex++;
            timeCounter = 4f;
        }

    }
}
