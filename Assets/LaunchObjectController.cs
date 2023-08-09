using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LaunchObjectController : MonoBehaviour
{
    [SerializeField]
    private float speedMultiplier;
    [SerializeField]
    private GameObject launchObjectPrefab;
    [SerializeField]
    private TMP_Text phaseText;
    [SerializeField]
    private float throwForce;

    private Camera cam;
    private GameObject launchObject = null;
    private bool isObjectLaunched = false;
    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 swipeForce;
    private float startTime;
    private float endTime;
    private Rigidbody rb;
    
    private int fingerId = -1;

    void Start()
    {
        cam = Camera.main;
        launchObject = Instantiate(launchObjectPrefab);
        launchObject.transform.position = cam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.2f, 10));
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
        if (Input.touchCount == 0 || isObjectLaunched)
        {
            return;
        }

        Touch touch;

        if (fingerId == -1)
        {
            touch = Input.GetTouch(0);
            fingerId = touch.fingerId;
        }
        else
        {
            if (!TryGetTouch(fingerId, out touch))
            {
                touch = Input.GetTouch(0);
                fingerId = touch.fingerId;
            }
        }

        TouchPhase touchPhase = touch.phase;
        phaseText.text = touchPhase.ToString();

        if (touchPhase == TouchPhase.Began || touchPhase == TouchPhase.Stationary)
        {
            startPos = touch.position;
            startPos.z = launchObject.transform.position.z - cam.transform.position.z;
            startPos = cam.ScreenToWorldPoint(startPos);
            startTime = Time.time;
            rb.isKinematic = false;
        }

        if (touchPhase == TouchPhase.Stationary || touchPhase == TouchPhase.Moved)
        {
            // get the touch position from the screen touch to world point
            Vector3 touchedPos = cam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10));
            launchObject.transform.position = touchedPos;
        }
        else if (touchPhase == TouchPhase.Ended)
        {
            isObjectLaunched = true;
            endPos = touch.position;
            endPos.z = launchObject.transform.position.z - cam.transform.position.z;
            endPos = cam.ScreenToWorldPoint(endPos);
            endTime = Time.time;
            swipeForce = (endPos - startPos);
            swipeForce.z = swipeForce.magnitude;
            swipeForce /= (endTime - startTime);
            rb.AddForce(swipeForce * throwForce);
            Destroy(launchObject, 3.0f);
            fingerId = -1;
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
        launchObject.transform.position = cam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.2f, 10));
        rb = launchObject.GetComponent<Rigidbody>();
    }

    private static bool TryGetTouch(int fingerID, out Touch touch)
    {
        foreach (var t in Input.touches)
        {
            if (t.fingerId == fingerID)
            {
                touch = t;
                return true;
            }
        }

        // No touch with given ID exists
        touch = default;
        return false;
    }
}
