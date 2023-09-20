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

public class Tutorial : MonoBehaviour
{

    public GameObject clickUIPrefab;

    private GameObject clickUI;

    private Mirror[] hoodMirrors;
    private bool havePlayedTutorial = false, conditionMet = false, canExitTutorial = false;
    public static Func<UIController> OnRequestTutorialMasterSupport;
    public static Func<UIController> OnRequestMovementTutorial;

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

    void CheckMirrorCollapseTutorialCondition() 
    {
        if (hoodMirrors.Length == 2 && !conditionMet) 
        {
            conditionMet = true;
            InitiateTutorial(OnRequestTutorialMasterSupport?.Invoke());
        }
        
    }

    private void Start()
    {
        InitialArrowData();

        MovementTutorial();
    }
    void InitialArrowData()
    {
        ArrowData[] arrows = new ArrowData[4];
        for (int i = 0; i < 4; i++)
            arrows[i] = new ArrowData();
        tutorialArrowData = arrows;
    }
    void MovementTutorial() 
    {
        UIController uc = OnRequestMovementTutorial.Invoke();
        Ray centerRay =  Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        if (Physics.Raycast(centerRay, out hit, 1000, LayerMask.GetMask("MirrorPlane")))
            StartCoroutine(uc.MoveArrowsAsGroup(hit.point, PlayerMove.playerTransform, 3f, 1f, 0f, 0f, 1f, 1f, tutorialArrowData, movementTutorialAnimationCurve, MovementTutorial_FollowArrowWithPlayer));
           
    }
    void MovementTutorial_FollowArrowWithPlayer(UIController uc) 
    {
        playerStartPosition = PlayerMove.playerTransform.position;
        StartCoroutine(uc.ArrowsFollowObject(PlayerMove.playerTransform, 1f, 0f, 1f, tutorialArrowData, MovementTutorial_FollowArrowWithPlayer_Condition, MovementTutorial_ArrowFade));
    }

    bool MovementTutorial_FollowArrowWithPlayer_Condition() 
    {
        return Vector3.Distance(playerStartPosition,PlayerMove.playerTransform.position) < 4f && keyPressCounter < 4;
    }

    void MovementTutorial_ArrowFade(UIController uc) 
    {
        StartCoroutine(uc.MoveArrowsAsGroup(PlayerMove.playerTransform.position, PlayerMove.playerTransform, 1f, 3f, 0f, 1f, 0f, 1f, tutorialArrowData, movementTutorialAnimationCurve, MovementTutorial_TurnOff));

    }

    void MovementTutorial_TurnOff(UIController uc) 
    {
        uc.GroupControlArrows(Vector3.zero, 0, 0, 0, false, tutorialArrowData);
        InitialArrowData();
    }

    void CheckMovementTutorialExitCondition() 
    {
        if (Input.GetAxis("Vertical") > 0  && !hasPressedW) 
        {
            hasPressedW = true;
            keyPressCounter++;
            tutorialArrowData[0].RadiusOffsetIncreaseTo(1f, 4f);
            //tutorialArrowData[0].AlphaOffsetDecreaseTo(-0.15f, 4f);
        }
        if (Input.GetAxis("Horizontal") > 0 && !hasPressedD)
        {
            hasPressedD = true;
            keyPressCounter++;
            tutorialArrowData[1].RadiusOffsetIncreaseTo(1f, 4f);
            //tutorialArrowData[1].AlphaOffsetDecreaseTo(-0.15f, 4f);
        }
        if (Input.GetAxis("Vertical") < 0 && !hasPressedS)
        {
            hasPressedS = true;
            keyPressCounter++;
            tutorialArrowData[2].RadiusOffsetIncreaseTo(1f, 4f);
            //tutorialArrowData[2].AlphaOffsetDecreaseTo(-0.15f, 4f);
        }
        if (Input.GetAxis("Horizontal") < 0 && !hasPressedA)
        {
            hasPressedA = true;
            keyPressCounter++;
            tutorialArrowData[3].RadiusOffsetIncreaseTo(1f, 4f);
            //tutorialArrowData[3].AlphaOffsetDecreaseTo(-0.15f, 4f);
        }
    }

   
    private void Update()
    {
        CheckMirrorCollapseTutorialCondition();
        CheckMovementTutorialExitCondition();

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
