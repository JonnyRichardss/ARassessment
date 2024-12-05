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

    private enum CRTState
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
                //IEnumerator c = FlyIn();
                //StartCoroutine(c);
                smoke.Emit(smokeCount);
                transform.position = value.position;
                transform.localScale = value.localScale * imageScaleMultiplier;
                currentState = CRTState.OnWorld;
            }
            else
            {
                //IEnumerator c = FlyOut();
                //StartCoroutine(c);
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
    private CRTState currentState;
    //private VisualEffect smoke;
    private ParticleSystem smoke;
    private Vector3 startPos;
    private Vector3 startScale;
    private Camera activeCam;
    private void Awake()
    {
        startPos = transform.position;
        startScale = transform.localScale;
        smoke = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        //look at current cam
        activeCam = (currentState == CRTState.OnWorld) ? Camera.main : SecondaryCamera;
        transform.LookAt(activeCam.transform);



    }
    private bool Visible(Vector3 offset)
    {
        Vector3 viewPos = activeCam.WorldToViewportPoint(transform.position + offset);
        return HelperMath.LessThan(viewPos, new Vector3(1, 1, float.MaxValue)) && HelperMath.GreaterThan(viewPos, Vector3.zero);
    }
    //flight needs improving
    //i think setting a target for both directions is needed
    //and lerp smoothing between them
    //going to be harder to get a target pos for flying out
    private IEnumerator FlyOut()
    {
        //do fly down
            //TODO
        //do fly up
        while (Visible(transform.up * -1 * sizeOffset))
        {
            transform.position += activeCam.transform.up * flySpeed;
            yield return null;
        }
        currentState = CRTState.OnUi;
        yield break;
    }
    private IEnumerator FlyIn()
    {
        transform.position = _targetTransform.position + _targetTransform.up * 5;
        //do fly down
        while (Vector3.Distance(transform.position,_targetTransform.position) > posLeniency)
        {
            transform.position -= _targetTransform.up * flySpeed;
            yield return null;
        }
        currentState = CRTState.OnWorld;
        yield break;
    }

}

