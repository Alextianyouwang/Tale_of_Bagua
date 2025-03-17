
using UnityEngine;


public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed,moveIncrement =1;
    private Vector3 horizontal, vertical;

    public static Transform playerTransform;
    public static Vector3 playerPosition;
    public static bool canUseWASD = true;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerTransform = transform;
        playerPosition = transform.position;

    }
    private void OnDisable()
    {
        canUseWASD = true;
    }


    private void FixedUpdate()
    {
        playerPosition = transform.position;

        if (canUseWASD) 
        {
            Movement();
            rb.isKinematic = false;
        }
         
      // else
      //     rb.isKinematic = true;
    }
    private void Movement()
    {
        horizontal = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
        vertical = new Vector3(0f, 0f, Input.GetAxisRaw("Vertical"));
        Vector3 force = (horizontal + vertical).normalized * moveIncrement;
        rb.AddForce(  force,ForceMode.Force);
    }
/*    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag.Equals("Mirror")) 
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce((horizontal + vertical).normalized * 0.1f, ForceMode.Impulse);
        }
    }*/

}
