using System.Collections.Generic;
using TriInspector;
using UnityEngine;
public class LazerEmitter : RationalObject,IInteractable
{

    [SerializeField] public SOVB_LazerObjectBase SelectInteractionType;


    public Material[] RayVisualMaterial;
    private List<Vector3> _rayCastPositionTracker = new List<Vector3>();
    private RaycastHit _hitReceiverObject;
    private LineRenderer[] _rayVisual;


    private LazerEmitter _chainedEmitter = null;
    public Transform GUI_Parent;
    [ReadOnly]
    public int FaceDirection = 0;

    protected void OnEnable()
    {
        PrepareLineVisual();
        OnReceive += BranchReceived;
        GameObject g = Instantiate(SelectInteractionType.GUI, GUI_Parent,false);
        g.layer = gameObject.layer;
    }
    protected void OnDisable()
    {
        DestroyImmediate(GUI_Parent.GetChild(0).gameObject);
    }

    private Vector3 CalculateDirectionBaseOnType(RationalObject ro) 
    {
        SOB_Lazer_Reflect reflector = SelectInteractionType as SOB_Lazer_Reflect;
        SOB_Lazer_SubEmit emitter = SelectInteractionType as SOB_Lazer_SubEmit;

        if (emitter != null)
        {
            return emitter.CreateDirection((SOB_Lazer_SubEmit.Orientation)FaceDirection);

        }
        else return Vector3.zero;
       
    }

    [Button]
    public void ChangeDirection() 
    {
        SOB_Lazer_Reflect reflector = SelectInteractionType as SOB_Lazer_Reflect;
        SOB_Lazer_SubEmit emitter = SelectInteractionType as SOB_Lazer_SubEmit;

        if (emitter != null)
        {
            FaceDirection = emitter.LoopOriantation(FaceDirection);
            GUI_Parent.eulerAngles = emitter.CreateEulerAngle((SOB_Lazer_SubEmit.Orientation)FaceDirection);
            GUI_Parent.localScale = emitter.Reflector ? Vector3.one * 0.5f : Vector3.one;
        }
    }
    public void DetermineInteractionBaseOnType() 
    {
        SOB_Lazer_Reflect reflector = SelectInteractionType as SOB_Lazer_Reflect;
        SOB_Lazer_SubEmit emitter = SelectInteractionType as SOB_Lazer_SubEmit;

        if (emitter != null) 
        {
            if (!emitter.Reflector)
                ReceiveShootCommand(this);
            else 
            {
                FaceDirection = emitter.LoopOriantation(FaceDirection);
                GUI_Parent.eulerAngles = emitter.CreateEulerAngle((SOB_Lazer_SubEmit.Orientation)FaceDirection);
                GUI_Parent.localScale = emitter. Reflector ? Vector3.one * 0.5f : Vector3.one;
            }
        }
    }

    private bool FalseConditionBasedOnType(RationalObject ro) 
    {
        LazerEmitter l = ro.GetComponent<LazerEmitter>();
        if (l == null)
            return false;
        SOB_Lazer_Reflect reflector = SelectInteractionType as SOB_Lazer_Reflect;
        SOB_Lazer_SubEmit emitter = SelectInteractionType as SOB_Lazer_SubEmit;
        if (emitter != null)
        {
            SOB_Lazer_SubEmit other = l.SelectInteractionType as SOB_Lazer_SubEmit;
            return FaceDirection != (l.FaceDirection + 2) % 4;
        }
        else return true;
       //switch (SelectInteractionType) 
       //{
       //    case (SOB_Lazer_SubEmit):
       //        
       //        break;
       //}
    }
    private void PrepareLineVisual()
    {
        _rayVisual = new LineRenderer[5];
        for (int i = 0; i < _rayVisual.Length; i++)
        {
            GameObject g = new GameObject();
            g.transform.parent = transform;
            g.layer = 6 + i;
            g.AddComponent<LineRenderer>();
            _rayVisual[i] = g.GetComponent<LineRenderer>();
            _rayVisual[i].material = RayVisualMaterial[i];
            _rayVisual[i].enabled = false;
        }

    }

    public void Interact(Vector3 pos) 
    {
        //if (InteractionOptions == Type.Reflect)
        //    return;
        //if (!Reflector)
        //    ReceiveShootCommand(this);
        //else 
        //{
        //    int currentOriaintation = (int)OriantationOptions;
        //    currentOriaintation += 1;
        //    currentOriaintation %= 4 ;
        //    OriantationOptions = (Orientation)currentOriaintation;
        //    Editor_ChangeOriantationUI();
        //}
        DetermineInteractionBaseOnType();
    }
    public void Hold() { }

    public void Disengage() 
    {
        RecursivelyStop();
    }
    public bool IsVisible()
    {
        return IsObjectVisibleAndSameLevelWithPlayer();
    }

    public IconType GetIconType() 
    {
        return IconType.kavaii;
    }
    private void ReceiveShootCommand(RationalObject ro) 
    {
        ShootLazer(80, 0.23f, CalculateDirectionBaseOnType(ro));
    }
    public bool IsActive() 
    {
        return true;
    }
    private void RecursivelyStop() 
    {
        LazerEmitter current = this;
        while (current != null)
        {
            current.ReceiveStopCommand();
            current = current._chainedEmitter;
        }
    }
    private void ReceiveStopCommand() 
    {
        foreach (LineRenderer r in _rayVisual)
            r.enabled = false;
        _rayCastPositionTracker.Clear();
    }
    private void ShootLazer(int steps, float increment, Vector3 direction) 
    {
        print(direction);
        _rayCastPositionTracker.Clear();

        Vector3 currentPosition = transform.position;
        
        for (int i = 0; i < steps; i++) 
        {
            _rayCastPositionTracker.Add(currentPosition);
            if (HandShake(currentPosition, increment * direction, out _hitReceiverObject))
            {
                if (CheckVisibility(_hitReceiverObject))
                {
                    LazerEmitter chain = _hitReceiverObject.transform.GetComponent<LazerEmitter>();
                    if (chain != null)
                        _chainedEmitter = chain;

                   
                    _hitReceiverObject.transform.GetComponent<RationalObject>().Receive(this);
                    break;
                }
            }
            if (!FreeToProceed(currentPosition, 0.01f))
                break;

            currentPosition += increment * direction;
        }
        foreach (LineRenderer r in _rayVisual) 
        {
            r.enabled = true;
            r.SetPosition(0, transform.position);
            r.SetPosition(1, _hitReceiverObject.transform == null ? currentPosition : _hitReceiverObject.point);
            r.startWidth = 0.1f;
            r.endWidth = 0.1f;
        }
      
    }
    public void BranchReceived(RationalObject ro)
    {
        //LazerEmitter l = ro.GetComponent<LazerEmitter>();
        //if (l == null)
        //    return;
        //
        //if ((int)l. OriantationOptions == ((int)OriantationOptions + 2) % 4)
        //    return;

        if (FalseConditionBasedOnType(ro))
            return;
        SOB_Lazer_SubEmit emitter = SelectInteractionType as SOB_Lazer_SubEmit;
        if ( emitter. Reflector)
            ReceiveShootCommand(ro);
    }


   // public void Editor_ChangeOriantationUI() 
   // {
   //     if (!VisualCueUI_Dir)
   //         return;
   //     Vector3 eularAngle = Vector3.zero;
   //     switch (OriantationOptions)
   //     {
   //         case Orientation.Top:
   //             eularAngle = new Vector3(90, 180, 0);
   //             break;
   //         case Orientation.Bot:
   //             eularAngle = new Vector3(90, 0, 0);
   //             break;
   //         case Orientation.Left:
   //             eularAngle = new Vector3(90, 90, 0);
   //             break;
   //         case Orientation.Right:
   //             eularAngle = new Vector3(90, 270, 0);
   //             break;
   //     }
   //     VisualCueUI_Dir.transform.eulerAngles = eularAngle;
   //     VisualCueUI_Dir.transform.localScale = Reflector? Vector3.one * 0.5f : Vector3.one;
   // }

   // public void Editor_SwapUI(Type type) 
   // {
   //     if (type == Type.Reflect)
   //     {
   //         VisualCueUI_Ref.SetActive(true);
   //         VisualCueUI_Dir.SetActive(false);
   //     }
   //     else if (type == Type.SubEmit)
   //     {
   //         VisualCueUI_Ref.SetActive(false);
   //         VisualCueUI_Dir.SetActive(true);
   //     }
   //     VisualCueUI_Ref.transform.localScale = Vector3.one;
   // }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        foreach (Vector3 v in _rayCastPositionTracker)
            Gizmos.DrawSphere(v,0.1f);
        if (_hitReceiverObject.transform?.GetComponent<LazerReceiver>()) 
        {
            if (_hitReceiverObject.transform.GetComponent<LazerReceiver>().tag.Equals("LazerReceiver")) 
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_hitReceiverObject.point, 0.2f);
            }
        }
     
    }
}
