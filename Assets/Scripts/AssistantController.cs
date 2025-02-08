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
    public string[] script =
    {
        "Welcome Back!",
        "Good to see you again!",
        "Hello. I am I.S.A.A.C., your virtual assistant. I am here to help you find information about the computer science course here at Lincoln!",
        "This is an image designed by Dr. Craig Green to cause a voxel-art model of himself to appear in the augmented world! Unfortunately, we seem to have lost the model! Sorry!",
        "",
        "",
        "",
        "",
        "",
        "",
        "Hint0",
        "Hint1",
        "Hint2",
        ""
    };
    public enum CRTState
    {
        OnUi,
        OnWorld
    }
    public Camera SecondaryCamera;
    public Transform startParent;
    public AudioSource teleportSound;
    public AudioSource completion;
    //considering we are now also piping name through it might have been better to use a reference to the script instead of the transform but oh well
    [HideInInspector]
    public string imageName;
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
                teleportSound.Play();
                RDG.Vibration.Vibrate(50, -1, true);
                transform.SetParent(targetTransform,true);
                transform.position = value.position;
                transform.localScale = Vector3.one * imageScaleMultiplier;

                currentState = CRTState.OnWorld;
                int ScriptID = NameToScriptID(imageName);
                if(ScriptID >=0) Say(ScriptID);   //-1 is error value - throw instead of silent error for out of range otherwise
            }
            else
            {
                smoke.Emit(smokeCount);
                RDG.Vibration.Vibrate(50, -1, true);
                teleportSound.Play();
                transform.SetParent(startParent, true);
                transform.position = startPos;
                transform.localScale = startScale;
                currentState = CRTState.OnUi;
            }
        } 
    }
    public float CompletionTextTime = 4.0f;
    public GameObject CompletedText;
    public HintButton hintButton;
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
        if (!BadgeManager.loadSucceded || BadgeManager.hasReset)
        {
            Say(8);
            textBox.callback = () => {StartCoroutine(hintButton.SpecialLine());};
          
        }
        else
        {
            Say(Random.Range(0, 2));
        }
    }
    // Update is called once per frame
    void Update()
    {
        //look at current cam
        activeCam = (currentState == CRTState.OnWorld) ? Camera.main : SecondaryCamera;
        transform.LookAt(activeCam.transform);



    }  
    private IEnumerator CompletionText()
    {
        completion.Play();
        RDG.Vibration.Vibrate(200, -1, true);
        CompletedText.SetActive(true);
        yield return new WaitForSeconds(CompletionTextTime);
        CompletedText.SetActive(false);

    }
    private IEnumerator SayCompletion()
    {
        yield return null;
        Say(5);
        StartCoroutine(CompletionText());
        yield break;
    }
    public void ShowCompletion()
    {
        textBox.callback = () => {StartCoroutine(SayCompletion()); };
    }
    public void Say(string text)
    {
        textBox.gameObject.SetActive(true);
        textBox.SetText(text);

    }
    public void Say(int scriptID)
    {
        if (scriptID < script.Length && scriptID >=0)
        {
            Say(script[scriptID]);
        }
    }
    public int NameToScriptID(string name)
    {
        switch (name)
        {
            case "INB":
                return 2;
            case "Labs":
                return 3;
            case "CSS":
                return 4;
            case "EasterEgg":
                return 6;
            case "Dr_Green":
                return 7;
            default:
                return -1;
        }
    }
}

