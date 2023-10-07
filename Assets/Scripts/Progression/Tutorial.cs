using System.Collections;
using UnityEngine;
using System;

using static UIController;
using Unity.VisualScripting;

public class Tutorial : MonoBehaviour
{

    public GameObject clickUIPrefab;

    private GameObject clickUI;

    private Mirror[] hoodMirrors;
    private bool havePlayedMirrorCollapseTutorial = false, mirrorCollapseConditionMet = false, canExitMirrorCollapseTutorial = false;
    private int mirrorCollapseClickTime = 0;
    public static Func<UIController> OnRequestTutorialMasterSupport;
    public static Action<bool> OnToggleDarkenScreen;
    
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

    void MirrorCollapseTutorial_Condition() 
    {
        if (hoodMirrors!= null)
        if (hoodMirrors.Length == 2 && !mirrorCollapseConditionMet) 
        {
            mirrorCollapseConditionMet = true;
            InitiateMirrorCollapseTutorial();
        }
        if (MirrorCollapseTutorial_ExitCondition())
            MirrorManager.canUseRightClick = true;

        if (havePlayedMirrorCollapseTutorial) 
        {
            if (Input.GetMouseButtonDown(1))
                mirrorCollapseClickTime++;
        }

    }
    bool MirrorCollapseTutorial_ExitCondition() 
    {
        return (Input.GetMouseButtonDown(1) && havePlayedMirrorCollapseTutorial);
      
    }

    bool MirrorCollapseTutorial_UI_ExitCondition()
    {
        return mirrorCollapseClickTime == 2;

    }
    void InitiateMirrorCollapseTutorial()
    {
        if (havePlayedMirrorCollapseTutorial)
            return;
        havePlayedMirrorCollapseTutorial = true;
        StartCoroutine(QueueTutorialActions(uc, MirrorCollapseTutorial_ExitCondition));
        StartCoroutine(ClickButtonShow(uc, uc.hoodMirrors, 0.7f, false,-1, MirrorCollapseTutorial_UI_ExitCondition));
    }
    IEnumerator QueueTutorialActions(UIController master,Func<bool> exitCondition)
    {
        MirrorManager.canUseRightClick = false;

        Coroutine co;
        co = StartCoroutine(master.UI_EaseInOut(1f,true,false));
        yield return co;

        while (!exitCondition.Invoke())
        {
            master.ConstantlyUpdatingUIPos(0.2f, master.screenMargin);
            for (int i = 0; i < 4; i++)
                master.arrows[i].gameObject.SetActive(hoodMirrors.Length >= 2);
            yield return null;
        }
       
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
        MirrorCollapseTutorial_Condition();
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

    Vector3 GetScreenCenterPosition() 
    {
        Ray centerRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit[] hits = Physics.RaycastAll (centerRay,Mathf.Infinity, LayerMask.GetMask("MirrorPlane"));
        RaycastHit finalHit = new RaycastHit();
        foreach (RaycastHit hit in hits) 
        {
            if (hit.transform.tag.Equals("MirrorPlane"))
                finalHit = hit;
        }

        return finalHit.point;
    }
    void MovementTutorial() 
    {
        StartCoroutine(uc.MoveArrowsAsGroup(GetScreenCenterPosition(), PlayerMove.playerTransform, 3f, 1f, 0f, 0f, 1f, 1.5f, tutorialArrowData, movementTutorialAnimationCurve,uc.SyncOtherStuffWithArrow, MovementTutorial_FollowArrowWithPlayer));   
    }
    void MovementTutorial_FollowArrowWithPlayer(UIController uc) 
    {
        playerStartPosition = PlayerMove.playerTransform.position;
        StartCoroutine(uc.ArrowsFollowObject(PlayerMove.playerTransform, 1f, 0f, 1f, tutorialArrowData, MovementTutorial_FollowArrowWithPlayer_Condition,uc.SyncOtherStuffWithArrow, MovementTutorial_ArrowFade));
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
        StartCoroutine(uc.MoveArrowsAsGroup(PlayerMove.playerTransform.position, PlayerMove.playerTransform, 1f, 3f, 0f, 1f, 0f, 1f, tutorialArrowData, movementTutorialAnimationCurve, uc.SyncOtherStuffWithArrow,Tutorial_TurnOff));

    }

    void Tutorial_TurnOff(UIController uc) 
    {
        uc.ResetArrowDatas(tutorialArrowData);
        uc.GroupControlArrows(Vector3.zero, 0, 0, 1, false, tutorialArrowData, null);
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
        t.transform.localPosition = target;
        return t;
    }
    void MirrorDragTutorial() 
    {
        PlayerMove.canUseWASD = false;
        MirrorManager.canUseLeftClick = false;
        OnToggleDarkenScreen?.Invoke(true);
        StartCoroutine(uc.MoveArrowsAsGroup(GetScreenCenterPosition(), NewTransformFromPositon(GetScreenCenterPosition()), 4f, 1f, 0f, 0f, 1f, 6f, tutorialArrowData, movementTutorialAnimationCurve, null, MirrorDragTutorial_LockMirror));
    }
    void MirrorDragTutorial_LockMirror(UIController uc) 
    {
        MirrorManager.canUseLeftClick = true;
        Mirror currentMirror = FindObjectOfType<Mirror>();
        StartCoroutine(MirrorDragTutorial_QueueAction(currentMirror,MirrorDragTutorial_ExitCondition));

    }

    void MirrorDragTutorial_FreeArrow(UIController uc) 
    {
        Mirror cm = FindObjectOfType<Mirror>();
        foreach (ArrowData d in tutorialArrowData) 
        {
            d.lerpValue = Mathf.Max(d.lerpValue, 0);
            d.alphaMultiplier = 0;
        }

        StartCoroutine(uc.MoveArrowsAsGroup(GetScreenCenterPosition(),cm.transform, 1, 5f, 0f, 1f, 0f, 2f, tutorialArrowData, movementTutorialAnimationCurve, null, Tutorial_TurnOff));

    }

    bool MirrorDragTutorial_ExitCondition(Vector3 initialPos, Mirror cm) 
    {
        return Vector3.Distance(initialPos,cm.transform.position) >= 2.5f;
    }

    void MirrorDragTutorial_UpdateArrow(float currnentTime,Vector3 initialPos, Mirror cm) 
    {
        float dist = Vector3.Distance(initialPos, cm.transform.position);
        
        float wavingValue = Mathf.Sin((Time.time - currnentTime) * 3f) * 0.25f + 0.25f;

        tutorialArrowData[0].lerpValue = (cm.transform.position.z - initialPos.z >= 0 ? cm.transform.position.z - initialPos.z : 0) + wavingValue;
        tutorialArrowData[1].lerpValue = (cm.transform.position.x - initialPos.x >= 0 ? cm.transform.position.x - initialPos.x : 0) + wavingValue;
        tutorialArrowData[2].lerpValue = (cm.transform.position.z - initialPos.z < 0 ? -cm.transform.position.z + initialPos.z : 0) + wavingValue;
        tutorialArrowData[3].lerpValue = (cm.transform.position.x - initialPos.x < 0 ? -cm.transform.position.x + initialPos.x : 0) + wavingValue;
        foreach (ArrowData d in tutorialArrowData)
            d.alphaMultiplier = -1.5f;
    }

    IEnumerator MirrorDragTutorial_QueueAction(Mirror cm,Func<Vector3,Mirror,bool> exitCondition) 
    {

        Coroutine clickUI_Co = StartCoroutine(ClickButtonShow(uc, new Mirror[] { cm },0.7f, true, -1, () => false));
        Coroutine arrowFollowCo = StartCoroutine(uc.ArrowsFollowObject(NewTransformFromPositon(GetScreenCenterPosition()), 1f, 0f, 0f, tutorialArrowData, () => true,null,null)); 
        Vector3 initialPos = cm.transform.position;
        float currentTime = Time.time;
        while (!exitCondition.Invoke(initialPos,cm)) {
            MirrorDragTutorial_UpdateArrow(currentTime,initialPos, cm);
            
            if (Input.GetMouseButtonDown(0) && IsCursorOnMirror())
                cm.AbortMovement();
            if (Input.GetMouseButtonUp(0)) 
                cm.MoveMirrorTowards(1f, GetScreenCenterPosition(), movementTutorialAnimationCurve);

            yield return null;

        }
        PlayerMove.canUseWASD = true;
        if (!exitCondition.Invoke(cm.transform.position, cm)) 
        {
            StopCoroutine(clickUI_Co);
            StopCoroutine(arrowFollowCo);
            MirrorDragTutorial_FreeArrow(uc);
            OnToggleDarkenScreen?.Invoke(false);
            StartCoroutine(ClickButtonShow(uc, new Mirror[] { cm },0.2f, true, 1, () => true));
        }
    }
    bool IsCursorOnMirror() 
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(mouseRay, out hit, 1000, LayerMask.GetMask("MirrorPlane"))) 
            if(hit.transform.tag.Equals("Mirror"))
                return true ;
            else return false;
        else
            return false ;
    }
    void InitiateUIMouse() 
    {
        clickUI = Instantiate(clickUIPrefab);
        clickUI.SetActive(false);
    }
 

    IEnumerator ClickButtonShow(UIController master, Mirror[] mirrors, float timeBetweenFlash, bool isLeft, int flashTime, Func<bool> exitCondition) 
    {
        clickUI.SetActive(true);
        float timer = 0;

        bool ping = false, pong = true;
        MaterialPropertyBlock mbp = new MaterialPropertyBlock();
        int counter = 0;
        while (!exitCondition.Invoke())
        {
            Vector3[] corners = master.Util_GetMirrorCorner(mirrors, 2f, 0f);
            Vector3 pos = Vector3.zero;
            foreach (Vector3 v in corners)
                pos += v;
            pos /= 4f;

            clickUI.SetActive(hoodMirrors.Length >= 2 ? true : mirrors.Length == 1);
            clickUI.transform.position = pos;

            timer  += Time.deltaTime;
            if (timer > timeBetweenFlash && (counter <= flashTime|| flashTime == -1)) 
            {
                if (pong) 
                {
                    ping = true;
                    pong = false;
                    if (!isLeft)
                        mbp.SetFloat("_RightAlpha", 1f);
                    else
                        mbp.SetFloat("_LeftAlpha", 1f);

                    counter += 1;
                }
                else if (ping)
                {
                    if (!isLeft)
                    {
                        mbp.SetFloat("_RightAlpha", 0.15f);
                        mbp.SetFloat("_LeftAlpha", 1f);
                    }
                    else 
                    {
                        mbp.SetFloat("_LeftAlpha", 0.15f);
                        mbp.SetFloat("_RightAlpha", 1f);
                    }
                    counter += 1;
                    ping = false;
                    pong = true;
                }
                timer = 0;
            }
            clickUI.GetComponent<Renderer>().SetPropertyBlock(mbp);
            yield return null;
        }
        clickUI.SetActive(false);
    }
  
}
