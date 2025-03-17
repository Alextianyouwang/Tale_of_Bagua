using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : RationalObject, IInteractable
{
    public GameObject ObjectToRotate;
    public GameObject[] Blocks;
    private bool _inAction = false;
    public void DetactPlayer() { }

    IEnumerator Rotate90Degree() 
    {
        float percentage = 0;
        _inAction = true;
        float startAngle = ObjectToRotate.transform.eulerAngles.y;
        ToggleBlocksCollider(false);
        while (percentage < 1) 
        {
            percentage += Time.deltaTime;
            float angle = Mathf.Lerp(startAngle, startAngle + 90, percentage);
            ObjectToRotate.transform.eulerAngles = new Vector3(ObjectToRotate.transform.eulerAngles.x, angle,ObjectToRotate.transform.eulerAngles.z);
            yield return null;
        }
        ToggleBlocksCollider(true);
        _inAction = false;
    }
    private void ToggleBlocksCollider(bool value) 
    {
        foreach (GameObject g in Blocks) 
        {
            g.GetComponent<BoxCollider>(). enabled = value;
        }
    }
    public void Interact(Vector3 pos) 
    {
        if (Vector3.Distance(new Vector2 (pos.x,pos.z),new Vector2 (transform.position.x, transform.position.z)) > 0.4f)
            return;
        if(_inAction == false)
            StartCoroutine(Rotate90Degree());   
    }

    public void Hold() { }

    public void Disengage()
    {

    }

    public bool IsVisible()
    {
        return IsObjectVisibleAndSameLevelWithPlayer();
    }

    public bool IsActive()
    {
        return true;
    }
    public IconType GetIconType()
    {

        return IconType.exclamation;
    }
}
