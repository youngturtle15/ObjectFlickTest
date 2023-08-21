using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LaunchObjectController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text statusText;
    [SerializeField]
    private GameObject launchObjectPrefab;
    [SerializeField]
    private float distanceFromCam;
    [SerializeField]
    private float throwThreshold;
    [SerializeField]
    private float throwMultiplier;

    private Camera cam;
    private GameObject launchObject = null;
    private bool isObjectLaunched = false;
    private Rigidbody rb;
    private float queueCapacity;
    private Queue<Vector2> posQueue = new Queue<Vector2>();
    private Vector2 startVector;
    private Vector2 endVector;

    void Start()
    {
        queueCapacity = 0.1f / Time.deltaTime;
        cam = Camera.main;
        launchObject = Instantiate(launchObjectPrefab);
        launchObject.transform.position = cam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.2f, distanceFromCam));
        rb = launchObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateObjectPosition();
        RespawnObject();
    }

    void UpdateObjectPosition()
    {
        if (isObjectLaunched)
        {
            return;
        }

        if (Input.touchCount == 0)
        {
            launchObject.transform.position = cam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.2f, distanceFromCam));
            return;
        }
        else
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    launchObject.transform.position = cam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, distanceFromCam));
                    AddPosition(touch.position);
                    break;
                case TouchPhase.Stationary:
                    launchObject.transform.position = cam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, distanceFromCam));
                    AddPosition(touch.position);
                    break;
                case TouchPhase.Moved:
                    launchObject.transform.position = cam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, distanceFromCam));
                    AddPosition(touch.position);
                    break;
                case TouchPhase.Ended:
                    Vector2 swipeVector = endVector - startVector;
                    statusText.text = $"Start Pos: {startVector}\nEnd Pos: {endVector}\nMagnitude: {swipeVector.magnitude}";
                    if (swipeVector.magnitude > throwThreshold)
                    {
                        rb.isKinematic = false;
                        isObjectLaunched = true;
                        Vector3 forceVector = new Vector3(swipeVector.x, swipeVector.y * throwMultiplier, swipeVector.magnitude * throwMultiplier);
                        rb.AddForce(forceVector);
                        Destroy(launchObject, 3.0f);
                    }
                    break;
            }
        }
    }

    void RespawnObject()
    {
        if (launchObject != null)
        {
            return;
        }

        isObjectLaunched = false;
        launchObject = Instantiate(launchObjectPrefab);
        launchObject.transform.position = cam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.2f, distanceFromCam));
        rb = launchObject.GetComponent<Rigidbody>();
    }

    void AddPosition(Vector2 newPos)
    {
        if (posQueue.Count < queueCapacity)
        {
            if (posQueue.Count > 1)
            {
                startVector = posQueue.Peek();
                endVector = newPos;
            }
            posQueue.Enqueue(newPos);
        }
        else
        {
            posQueue.Dequeue();
            startVector = posQueue.Peek();
            endVector = newPos;
            posQueue.Enqueue(newPos);
        }
    }
}

