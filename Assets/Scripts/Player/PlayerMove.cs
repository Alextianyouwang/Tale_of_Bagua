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
    public LayerMask obstacles;
    public static Transform playerTransform;
    public static Vector3 playerPosition;

    private void Awake()
    {
        playerTransform = transform;
        playerPosition = transform.position;

    }
    private void Start()
    {
       
        movePoint = transform.position;
    }
    private void FixedUpdate()
    {
        playerPosition = transform.position;

        PlayerMoveSnapToGrid();
    }
    private void PlayerMoveSnapToGrid()
    {
        horizontal = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
        vertical = new Vector3(0f, 0f, Input.GetAxisRaw("Vertical"));
        Vector3 force = (horizontal + vertical).normalized * moveIncrement;
        GetComponent<Rigidbody>().AddForce(  force,ForceMode.Force);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(movePoint + horizontal, 0.1f);
        Gizmos.DrawSphere(movePoint + vertical, 0.1f);
    }
}
