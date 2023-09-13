using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowProjectile : MonoBehaviour
{
    [SerializeField]
    private Vector3 endPoint;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float maximumHeight;
    [SerializeField]
    private float gravity;

    private Rigidbody rb;
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SimulateThrowingItem();
        }
    }

    void SimulateThrowingItem()
    {
        Vector3 xzVectorToTarget = new Vector3(target.position.x - transform.position.x, 0f, target.position.z - transform.position.z);
        float heightOfTarget = target.position.y;

        Vector3 horizontalVelocity = xzVectorToTarget / (Mathf.Sqrt(-2 * maximumHeight / gravity) + Mathf.Sqrt(2 * (heightOfTarget - maximumHeight) / gravity));
        Vector3 verticalVelocity = Vector3.up * Mathf.Sqrt(-2 * gravity * maximumHeight);

        rb.isKinematic = false;
        Physics.gravity = Vector3.up * gravity;
        rb.AddForce(horizontalVelocity + verticalVelocity, ForceMode.VelocityChange);
        //rb.velocity = horizontalVelocity + verticalVelocity;

        //Vector2 swipeVector = cam.WorldToScreenPoint(endPoint) - cam.WorldToScreenPoint(transform.position);
        //Vector3 forceVector = new Vector3(swipeVector.x, swipeVector.y, swipeVector.magnitude);
        //rb.isKinematic = false;
        //rb.AddForce(forceVector);
    }
}
