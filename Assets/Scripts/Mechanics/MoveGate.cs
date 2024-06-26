using System.Collections;
using UnityEngine;

public class MoveGate : MonoBehaviour
{
    public Transform StartPos;
    public Transform EndPos;

    private Coroutine _move_CO;

    public void StartMove(bool value) 
    {
        if (_move_CO != null)
        {
            StopCoroutine(_move_CO);
            _move_CO = null;
        }
       _move_CO =  StartCoroutine(Move(value));
    }
    IEnumerator Move( bool toEnd)
    {

        float distToStart = Vector3.Distance(transform.position, StartPos.position);
        float distToEnd = Vector3.Distance(transform.position, EndPos.position);
        float startToEnd = Vector3.Distance(StartPos.position, EndPos.position);
        float percentage = toEnd? distToEnd/startToEnd : distToStart/startToEnd;
        Vector3 startPos = toEnd ? StartPos.position : EndPos.position; 
        Vector3 endPos = toEnd ? EndPos.position : StartPos.position; 

        while (percentage > 0)
        {
            percentage -= Time.deltaTime;

            transform.position = Vector3.Lerp(endPos, startPos, percentage);
            yield return null;
        }

    }
}
