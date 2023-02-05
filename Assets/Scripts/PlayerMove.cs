using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Vector3 playerCenterOffset = Vector3.zero;
    [SerializeField] private float moveSpeed,moveIncrement =1;
    private Vector3 horizontal, vertical, movePoint;
    public LayerMask obstacles,mirrorMask;
 
    private void Start()
    {
        movePoint = transform.position;
    }
    private void Update()
    {
        PlayerMoveSnapToGrid();
    }

    private void PlayerMoveSnapToGrid()
    {
        horizontal = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
        vertical = new Vector3(0f, 0f, Input.GetAxisRaw("Vertical"));
        transform.position = Vector3.MoveTowards(transform.position, movePoint + playerCenterOffset, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint + playerCenterOffset) <= 0.05f)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1)
            {
                Collider[] colliders = Physics.OverlapSphere(movePoint + horizontal * moveIncrement, 0.1f, obstacles);
                if (colliders.Length == 0 || colliders.Where(x => x.isTrigger == false).ToArray().Length == 0)
                {
                    movePoint += horizontal* moveIncrement;
                }
            }
            if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1)
            {
                Collider[] colliders = Physics.OverlapSphere(movePoint + vertical * moveIncrement, 0.1f, obstacles);
                if (colliders.Length == 0 || colliders.Where(x => x.isTrigger == false).ToArray().Length == 0)
                {
                    movePoint += vertical * moveIncrement;
                }
            }
        }
    }

   

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(movePoint + horizontal, 0.1f);
        Gizmos.DrawSphere(movePoint + vertical, 0.1f);
    }
}
