using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Timeline;
using static UnityEngine.Rendering.DebugUI;
using Unity.VisualScripting;
using Unity.Burst.CompilerServices;
using static UIController;
using UnityEngine.Rendering;
using UnityEngine.UIElements.Experimental;

public class Tutorial : MonoBehaviour
{

    public GameObject clickUIPrefab;

    private GameObject clickUI;

    private Mirror[] hoodMirrors;
    private bool havePlayedMirrorCollapseTutorial = false, mirrorCollapseConditionMet = false, canExitMirrorCollapseTutorial = false;
    private bool havePlayedDragMirrorTutorial = false, dragMirrorConditionMet = false, canExitDragMirrorTutorial = false;
    public static Func<UIController> OnRequestTutorialMasterSupport;
    private UIController uc;

    public AnimationCurve movementTutorialAnimationCurve;
    private Vector3 playerStartPosition;

    private ArrowData[] tutorialArrowData;
    private int keyPressCounter = 0;
    private bool hasPressedW = false, hasPressedD = false, hasPressedS = false, hasPressedA = false;

    [ColorUsage(true, true)]
    public Color normalCol;
    [ColorUsage(true, true)]
    public Color flickerCol;

    private void OnEnable()
    {
        LayerCheck.OnShareHoodMirror += ReceiveHoodMirror;
        ProgressionController.OnBaguaCollected += ReceiveBaguaCollected;
    }
    private void OnDisable()
    {
        LayerCheck.OnShareHoodMirror -= ReceiveHoodMirror;
        ProgressionController.OnBaguaCollected -= ReceiveBaguaCollected;


    }
    void ReceiveHoodMirror(Mirror[] hoodMirror)
    {
        hoodMirrors = hoodMirror;
    }

    void CheckMirrorCollapseTutorialCondition() 
    {
        if (hoodMirrors.Length == 2 && !mirrorCollapseConditionMet) 
        {
            mirrorCollapseConditionMet = true;
            InitiateMirrorCollapseTutorial();
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (!canExitMirrorCollapseTutorial && havePlayedMirrorCollapseTutorial)
            {
                MirrorManager.canUseRightClick = true;
                canExitMirrorCollapseTutorial = true;
            }

        }

    }
    void InitiateMirrorCollapseTutorial()
    {
        if (havePlayedMirrorCollapseTutorial)
            return;
        havePlayedMirrorCollapseTutorial = true;
        StartCoroutine(QueueTutorialActions(uc));
        StartCoroutine(ClickButtonShow(uc));
    }
    IEnumerator QueueTutorialActions(UIController master)
    {
        MirrorManager.canUseRightClick = false;

        Coroutine co;
        co = StartCoroutine(master.UI_EaseInOut(1f,true,false));
        yield return co;

        while (!canExitMirrorCollapseTutorial)
        {
            master.ConstantlyUpdatingUIPos(0.2f, 60f);
            for (int i = 0; i < 4; i++)
                master.arrows[i].gameObject.SetActive(hoodMirrors.Length >= 2);
            yield return null;
        }
        if (canExitMirrorCollapseTutorial)
        { StopCoroutine(co); }
    }


    private void Start()
    {
        uc = OnRequestTutorialMasterSupport.Invoke();
        InitiateUIMouse();

        InitialArrowData();

        MovementTutorial();
    }

    private void Update()
    {
        CheckMirrorCollapseTutorialCondition();
        CheckMovementTutorialExitCondition();


    }
    void InitialArrowData()
    {
        ArrowData[] arrows = new ArrowData[4];
        for (int i = 0; i < 4; i++)
            arrows[i] = new ArrowData();

        foreach (ArrowData a in arrows)
            a.mpb = new MaterialPropertyBlock();

        tutorialArrowData = arrows;
    }
    void MovementTutorial() 
    {
       
        Ray centerRay =  Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        if (Physics.Raycast(centerRay, out hit, 1000, LayerMask.GetMask("MirrorPlane")))
            StartCoroutine(uc.MoveArrowsAsGroup(hit.point, PlayerMove.playerTransform, 3f, 1f, 0f, 0f, 1f, 1.5f, tutorialArrowData, movementTutorialAnimationCurve,uc.SyncOtherStuffWithArrow, MovementTutorial_FollowArrowWithPlayer));
           
    }
    void MovementTutorial_FollowArrowWithPlayer(UIController uc) 
    {
        playerStartPosition = PlayerMove.playerTransform.position;
        StartCoroutine(uc.ArrowsFollowObject(PlayerMove.playerTransform, 1f, 0f, 1f, tutorialArrowData, MovementTutorial_FollowArrowWithPlayer_Condition, MovementTutorial_ArrowFade));
    }

    bool MovementTutorial_FollowArrowWithPlayer_Condition() 
    {
        if (Vector3.Distance(playerStartPosition, PlayerMove.playerTransform.position) >= 4f) 
        {
            hasPressedW = true;hasPressedD = true;hasPressedS = true;hasPressedA = true;
        }

        return Vector3.Distance(playerStartPosition, PlayerMove.playerTransform.position) < 4f && keyPressCounter < 4;
    }

    void MovementTutorial_ArrowFade(UIController uc) 
    {
        StartCoroutine(uc.MoveArrowsAsGroup(PlayerMove.playerTransform.position, PlayerMove.playerTransform, 1f, 3f, 0f, 1f, 0f, 1f, tutorialArrowData, movementTutorialAnimationCurve, uc.SyncOtherStuffWithArrow,MovementTutorial_TurnOff));

    }

    void MovementTutorial_TurnOff(UIController uc) 
    {
        uc.ResetArrowDatas(tutorialArrowData);
        
        uc.ResetWASD();
    }

    void CheckMovementTutorialExitCondition() 
    {
        if (Input.GetAxis("Vertical") > 0  && !hasPressedW) 
        {
            hasPressedW = true;
            keyPressCounter++;
            tutorialArrowData[0].RadiusOffsetIncreaseTo(1f, 2f);
      
        }
        if (Input.GetAxis("Horizontal") > 0 && !hasPressedD)
        {
            hasPressedD = true;
            keyPressCounter++;
            tutorialArrowData[1].RadiusOffsetIncreaseTo(1f, 2f);
        }
        if (Input.GetAxis("Vertical") < 0 && !hasPressedS)
        {
            hasPressedS = true;
            keyPressCounter++;
            tutorialArrowData[2].RadiusOffsetIncreaseTo(1f, 2f);
        }
        if (Input.GetAxis("Horizontal") < 0 && !hasPressedA)
        {
            hasPressedA = true;
            keyPressCounter++;
            tutorialArrowData[3].RadiusOffsetIncreaseTo(1f, 2f);
        }
    }



    void ReceiveBaguaCollected(GameObject b) 
    {
        if (b.name == "BaguaLV0")
            MirrorDragTutorial();
    }
    Transform NewTransformFromPositon (Vector3 target) 
    {
        Transform t = new GameObject().transform;
        t.transform.position = target;
        return t;
    }
    void MirrorDragTutorial() 
    {
        Ray centerRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        if (Physics.Raycast(centerRay, out hit, 1000, LayerMask.GetMask("MirrorPlane")))
            StartCoroutine(uc.MoveArrowsAsGroup(hit.point,NewTransformFromPositon(hit.point), 4f, 1f, 0f, 0f, 1f, 1.5f, tutorialArrowData, movementTutorialAnimationCurve,null, null));
    }

    void InitiateUIMouse() 
    {
        clickUI = Instantiate(clickUIPrefab);
        clickUI.SetActive(false);
    }
 

    IEnumerator ClickButtonShow(UIController master) 
    {
        clickUI.SetActive(true);
        float timer = 0;
        float timeBetweenFlash = 0.7f;
        bool ping = false, pong = true;
        clickUI.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = normalCol;
        while (!canExitMirrorCollapseTutorial)
        {
            Vector3[] corners = master.Util_GetHoodMirrorCorner(2f, 0f);
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
  
}
