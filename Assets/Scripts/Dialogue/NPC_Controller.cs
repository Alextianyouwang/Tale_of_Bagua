
using UnityEngine;
using System;

public class NPC_Controller : MonoBehaviour
{
    public Action<Vector3,TextAsset,Sprite> OnDetactPlayer;
    public Action OnLostPlayer;
    public TextAsset InkDialogueAsset;
    public Sprite IconImage;

}
