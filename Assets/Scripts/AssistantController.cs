using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.VFX;
using UnityEditor;

public class AssistantController : MonoBehaviour
{
    //POTENTIAL TODO - link state to a layer mask

    public enum CRTState
    {
        OnUi,
        OnWorld
    }
    public Camera SecondaryCamera;
    private Transform _targetTransform;
    public Transform targetTransform
    {

        get {  return _targetTransform; } 
        set
        {
            if (_targetTransform == value) return; //prevent re-setting (important)
            _targetTransform = value;
            if (value != null)
            {
                smoke.Emit(smokeCount);
                transform.position = value.position;
                transform.localScale = value.localScale * imageScaleMultiplier;
                currentState = CRTState.OnWorld;
            }
            else
            {
                smoke.Emit(smokeCount);
                transform.position = startPos;
                transform.localScale = startScale;
                currentState= CRTState.OnUi;
            }
        } 
    }
    public float imageScaleMultiplier = 0.5f;
    public int smokeCount = 50;
    public float sizeOffset = 10.0f;
    public float posLeniency = 2.0f;
    public float flySpeed = 1.0f;
    public static CRTState currentState {
        get;
        private set;
    }
    //private VisualEffect smoke;
    private ParticleSystem smoke;
    private Vector3 startPos;
    private Vector3 startScale;
    private Camera activeCam;
    private TextBox textBox;
    private void Awake()
    {
        startPos = transform.position;
        startScale = transform.localScale;
        smoke = GetComponent<ParticleSystem>();
        textBox = GetComponentInChildren<TextBox>(true);
    }
    private void Start()
    {
        Say("Hello World!");
    }
    // Update is called once per frame
    void Update()
    {
        //look at current cam
        activeCam = (currentState == CRTState.OnWorld) ? Camera.main : SecondaryCamera;
        transform.LookAt(activeCam.transform);



    }  
    public void Say(string text)
    {
        textBox.gameObject.SetActive(true);
        textBox.SetText(text);

    }
}

