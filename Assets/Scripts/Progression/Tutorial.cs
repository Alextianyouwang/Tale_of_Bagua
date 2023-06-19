using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Timeline;
using static UnityEngine.Rendering.DebugUI;

public class Tutorial : MonoBehaviour
{

    public GameObject clickUIPrefab;

    private GameObject clickUI;

    private Mirror[] hoodMirrors;
    private bool havePlayedTutorial = false, conditionMet = false, canExitTutorial = false;
    public static Func<UIController> OnRequestTutorialMasterSupport;

    [ColorUsage(true, true)]
    public Color normalCol;
    [ColorUsage(true, true)]
    public Color flickerCol;

    private void OnEnable()
    {
        Prepareation();
        LayerCheck.OnShareHoodMirror += ReceiveHoodMirror;
    }
    private void OnDisable()
    {
        LayerCheck.OnShareHoodMirror -= ReceiveHoodMirror;

    }
    void ReceiveHoodMirror(Mirror[] hoodMirror)
    {
        hoodMirrors = hoodMirror;
    }

    void CheckCondition() 
    {
        if (hoodMirrors.Length == 2 && !conditionMet) 
        {
            conditionMet = true;
            InitiateTutorial(OnRequestTutorialMasterSupport?.Invoke());
        }
        
    }

    private void Update()
    {
        CheckCondition();

        if (Input.GetMouseButtonDown(1)) 
        {
            if (!canExitTutorial && havePlayedTutorial) 
            {
                MirrorManager.canUseRightClick = true;
                canExitTutorial = true;
            }
          
        }
      
    }


    void Prepareation() 
    {
        clickUI = Instantiate(clickUIPrefab);
        clickUI.SetActive(false);
    }
    void InitiateTutorial(UIController master) 
    {
        if (havePlayedTutorial)
            return;
        havePlayedTutorial = true;
        StartCoroutine(QueueTutorialActions(master));
        StartCoroutine(ClickButtonShow(master));
    }

    IEnumerator ClickButtonShow(UIController master) 
    {
        clickUI.SetActive(true);
        float timer = 0;
        float timeBetweenFlash = 0.7f;
        bool ping = false, pong = true;
        clickUI.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = normalCol;
        while (!canExitTutorial)
        {
            Vector3[] corners = master.GetHoodMirrorCorner(2f, 0f);
            Vector3 pos = Vector3.zero;
            foreach (Vector3 v in corners)
                pos += v;
            pos /= 4f;

            clickUI.SetActive(hoodMirrors.Length >= 2);
            clickUI.transform.position = pos;

            timer  += Time.deltaTime;
            if (timer > timeBetweenFlash) 
            {
                if (pong) 
                {
                    ping = true;
                    pong = false;
                    clickUI.transform.GetChild(0). GetComponent<MeshRenderer>().material.color = normalCol;
                }
                else if (ping)
                {
                    clickUI.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = flickerCol;
                    ping = false;
                    pong = true;
                }
                timer = 0;
            }

            yield return null;
        }
        clickUI.SetActive(false);
    }
    IEnumerator QueueTutorialActions(UIController master)
    {
        MirrorManager.canUseRightClick = false;

        Coroutine co;
        co = StartCoroutine(master.ControlledUI_EaseInOut(1f, 2f, 0.2f, 0f, 1f, true, null)); 
        yield return co;

        while (!canExitTutorial) 
        {
            master.ConstantlyUpdatingUIPos(0.2f, 50f);
            for (int i = 0; i < 4; i++)
            {

                master.arrows[i].gameObject.SetActive(hoodMirrors.Length >= 2);
            }

           
            yield return null;
        }
        if (canExitTutorial)
        { StopCoroutine(co); }
    }

}
