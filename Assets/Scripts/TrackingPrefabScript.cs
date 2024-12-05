using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using System.Threading;
public class TrackingPrefabScript : MonoBehaviour
{
    public float timeout = 2.0f;
    private float timer;

    private TrackingState state;
    private ARTrackedImage image;
    private AssistantController crt;
    // Start is called before the first frame update
    private void Awake()
    {
        crt = GameObject.FindGameObjectWithTag("CRT").GetComponent<AssistantController>();
        image = GetComponent<ARTrackedImage>();
        timer = timeout;
    }
    // Update is called once per frame
    void Update()
    {
        //this only works for one target at a time LOL
        if (image.trackingState == TrackingState.Tracking)
        {
            timer = timeout;
            crt.targetTransform = transform;
            return;
        }
        timer -= Time.deltaTime;
        if (timer < 0 && crt.targetTransform == transform)
        {
            crt.targetTransform = null;
        }
    }
}
