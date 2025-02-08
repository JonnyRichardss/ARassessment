using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class TextBox : MonoBehaviour
{
    public Animator animator;
    public Camera secondaryCam;
    public GameObject triangle;
    private Canvas canvas;
    private bool animating = false;
    private float speed = 4.0f;
    public  float animationSpeed = 4.0f;
    public  float speedMultiplier = 4.0f;
    public float triangleTime = 0.5f;
    private float textTimer = 0.0f;
    private float triangleTimer = 0.0f;
    private string text;
    int startChar = 0;
    int stringLen = 0;
    private TMP_Text TextDisplay;
    public delegate void TalkingCallback();
    public TalkingCallback callback;
    public AudioSource talking;
    public AudioSource beginTalk;    
    public AudioSource endTalk;

    private void Awake()
    {
        if (!animator)
        {
            Debug.Log("Text box has no animator! destroying!");
            Destroy(gameObject);
        }
        if (!secondaryCam)
        {
            Debug.Log("Text box has no secondary camera! destroying!");
            Destroy(gameObject);
        }
        canvas = GetComponent<Canvas>();
        TextDisplay = GetComponentInChildren<TMP_Text>();
        speed = animationSpeed;
    }
    private void Update()
    {
        Camera targetCam;
        if (AssistantController.currentState == AssistantController.CRTState.OnUi) targetCam = secondaryCam;
        else targetCam = Camera.main;

        if (canvas.worldCamera != targetCam) canvas.worldCamera = targetCam; //preventing re-setting will be faster i assume


        transform.rotation = Quaternion.LookRotation(transform.position - targetCam.transform.position);
        if (animating)
        {
            if (textTimer >= (1.0f / speed))
            {
                textTimer = 0.0f;
                if (startChar + stringLen < text.Length)
                {
                    talking.Play();
                    stringLen++;
                }
                else
                {//when done
                    animating = false;
                    speed = animationSpeed;
                }
            }
            textTimer += Time.deltaTime;
        }
        DisplayText();
        AnimateTriangle();
    }
    private void OnEnable()
    {
        animator.SetBool("IsTalking", true);
    }
    private void OnDisable()
    {
        endTalk.Play();
        animator.SetBool("IsTalking", false);
        if (callback != null)
        {
            callback();
            callback = null;
        }
        
    }
    public void SetText(string newText)
    {
        beginTalk.Play();
        text = newText;
        animating = true;
        stringLen = 0;
        startChar = 0;
    }
    public void SpeedUp()
    {
        speed = animationSpeed * speedMultiplier;
    }
    private void DisplayText()
    {
        
        if (TextDisplay.isTextOverflowing)
        {
            int lineLen = TextDisplay.textInfo.lineInfo[0].characterCount - 1;
            startChar += lineLen;
            stringLen -= lineLen;
           // startLine++;
        }
        string textToDisplay;
        try
        {
             textToDisplay = text.Substring(startChar, stringLen);
        }
        catch (ArgumentOutOfRangeException) 
        {
            textToDisplay = text;
        }
        
        TextDisplay.text = textToDisplay;
    }
    private void AnimateTriangle()
    {
        triangle.transform.rotation = Quaternion.Euler(0, 0, animating ? 180 : -90);
        
        if (triangleTimer >= triangleTime)
        {
            triangle.SetActive(!triangle.activeSelf);
            triangleTimer = 0;
        }
        triangleTimer += Time.deltaTime;
    }
    public void OnClick()
    {
        if (animating) SpeedUp();
        else gameObject.SetActive(false);
    }
}
