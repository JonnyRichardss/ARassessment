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
    private Canvas canvas;
    private bool animating = false;
    private int speed = 1;
    public  int animationSpeed = 1;
    public  int speedMultiplier = 4;
    private string text;
    int visibleCharacters = 0;
    private TMP_Text TextDisplay;
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
    }
    private void Update()
    {
        Camera targetCam;
        if (AssistantController.currentState == AssistantController.CRTState.OnUi) targetCam = secondaryCam;
        else targetCam = Camera.main;


        if (canvas.worldCamera != targetCam) canvas.worldCamera = targetCam; //preventing re-setting will be faster i assume


        if (animating)
        {
            //do text animation stuff
            if (visibleCharacters < text.Length)
            {
                visibleCharacters += speed;
            }
            else
            {//when done
                animating = false;
                speed = animationSpeed;
            }
        }
        else
        {
            visibleCharacters = text.Length;
        }
        DisplayText();
    }
    private void OnEnable()
    {
        animator.SetBool("IsTalking", true);
        
    }
    private void OnDisable()
    {
        animator.SetBool("IsTalking", false);
    }
    public void SetText(string newText)
    {
        text = newText;
        animating = true;
        visibleCharacters = 0;
    }
    public void SpeedUp()
    {
        speed = animationSpeed * speedMultiplier;
    }
    private void DisplayText()
    {
        string textToDisplay;
        try
        {
             textToDisplay = text.Substring(0, visibleCharacters);
        }
        catch (ArgumentOutOfRangeException e) 
        {
            textToDisplay = text;
        }
        TextDisplay.text = textToDisplay;
    }
    //TODO - 
    //speed up on click then hide on click
}
