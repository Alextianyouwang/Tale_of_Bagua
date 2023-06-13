using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    public GameObject[] Slides;
    int slideIndex = 0;
<<<<<<< HEAD
    float timeCounter = 4f;
=======
    float timeCounter = 0f;
    public float slideTime = 4f;
>>>>>>> BeforeJamBuild

    private void Start()
    {
        Slides[slideIndex].SetActive(true);
    }

    public void Update()
    {
        timeCounter -= Time.deltaTime;

        if(timeCounter < 0)
        {
            Slides[slideIndex].SetActive(false); 
            slideIndex++;
<<<<<<< HEAD
            timeCounter = 4f;
            Slides[slideIndex].SetActive(true);
=======
            timeCounter = slideTime;
>>>>>>> BeforeJamBuild
        }

        if(slideIndex >= Slides.Length-1)
        {
            gameObject.SetActive(false);
            slideIndex = 0;
        }
    }
}
