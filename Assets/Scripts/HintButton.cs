using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class HintButton : MonoBehaviour
{
    //functionality
    //list of indices in main script that contain hints
    public int[] Hints =
    {
        10,11,12
    };
    public int SpecialIndex = 9;
    public GameObject HintDisplay;
    public AssistantController CRT;
    public TextBox TextBox;
    public AudioSource hintClip;
    public void Pressed()
    {
        CRT.Say(Hints[Random.Range(0, Hints.Length)]);
        hintClip.Play();
        HintDisplay.SetActive(true);
        TextBox.callback = TalkCallback;
    }
    public void TalkCallback()
    {
        HintDisplay.SetActive(false);
    }
    public IEnumerator SpecialLine()
    {
        //this entire shenanigan is to make it wait 1 (one) frame so that CRT.Say() is triggered AFTER OnDisable() has finished executing
        yield return null;
        HintDisplay.SetActive(true);
        CRT.Say(SpecialIndex);
        TextBox.callback = TalkCallback;
        yield break;
    }


    //aesthetics

    float timer = 0;
    public float minTime = 20.0f;
    public float maxTime = 40.0f;
    public float bobScale = 0.2f;
    public float bobSpeed = 2.0f;
    public float SpinSpeed = 8.0f;
    Vector3 initPos;
    private void Start()
    {
        HintDisplay.SetActive(false);
        timer = Random.Range(minTime, maxTime);
        initPos = transform.position;
    }
    private void Update()
    {
        if (timer <= 0)
        {
            timer = Random.Range(minTime, maxTime);
            IEnumerator spin = Spin();
            StartCoroutine(spin);
        }
        timer -= Time.deltaTime;

        transform.position = new Vector3(initPos.x, initPos.y+Mathf.Sin(Time.time * bobSpeed) * bobScale,initPos.z);
    }
    IEnumerator Spin()
    {
        float angleRotated = 0.0f;
        float prevAngle = 0.0f;
        float t = 0 ;
        while (angleRotated < 180)
        {
            t += Time.deltaTime;
           
            angleRotated += MathF.Exp(t);
            transform.Rotate(Vector3.up, angleRotated - prevAngle);
            prevAngle = angleRotated;
            yield return null;
        }
        while (angleRotated < 360)
        {
            angleRotated = HelperMath.expDecay(angleRotated, 360, SpinSpeed, Time.deltaTime);
            transform.Rotate(Vector3.up, angleRotated - prevAngle);
            prevAngle = angleRotated;
            yield return null;
        }
        yield break;
    }
}
