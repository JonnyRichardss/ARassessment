using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public static class UIState
{
    public enum State
    {
        WelcomeScreen,
        ARScene,
        BadgeMenu
    }
    public static State CurrentState = State.WelcomeScreen;

}
public class UIScripting : MonoBehaviour
{
    public AudioSource slideIn;
    public AudioSource slideOut;
    public GameObject objectToEnable;
    public RectTransform BadgeUITransform;
    public RectTransform WelcomeUITransform;
    public Vector2 BadgeShow   = new Vector2(-(1080 / 2), 0);
    public Vector2 BadgeHide   = new Vector2(1080 / 2, 0);
    public Vector2 WelcomeShow = new Vector2(0, 2340 / 2);
    public Vector2 WelcomeHide = new Vector2(0, -(2340 / 2));
    public float speed = 16; //roughly between 1 and 25
    public float endDistance = 5f; // distance to stop animating
    //bool to stop multiple animations firing
    private bool animating;
    private IEnumerator BadgeCoroutine;
    private IEnumerator WelcomeCoroutine;
    public void Start()
    {
        if (!BadgeUITransform || !WelcomeUITransform) Destroy(gameObject);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }
    void StartGame()
    {
        //do stuff
    }
    public void ShowBadges()
    {
        if (UIState.CurrentState != UIState.State.ARScene) return;
        slideIn.Play();
        RDG.Vibration.Vibrate(50, -1, true);
        BadgeCoroutine = AnimateWindow(BadgeShow,BadgeUITransform, UIState.State.BadgeMenu);
        StartCoroutine(BadgeCoroutine);

    }
    public void HideBadges()
    {
        if (UIState.CurrentState != UIState.State.BadgeMenu) return;
        slideOut.Play();
        RDG.Vibration.Vibrate(50, -1, true);
        BadgeCoroutine = AnimateWindow(BadgeHide, BadgeUITransform, UIState.State.ARScene);
        StartCoroutine(BadgeCoroutine);
    }
    public void ShowWelcome()
    {
        if (UIState.CurrentState != UIState.State.ARScene) return;
        slideIn.Play();
        WelcomeCoroutine = AnimateWindow(WelcomeShow, WelcomeUITransform, UIState.State.WelcomeScreen);
        StartCoroutine(WelcomeCoroutine);
    }
    public void HideWelcome()
    {
        if (UIState.CurrentState != UIState.State.WelcomeScreen) return;
        slideOut.Play();
        WelcomeCoroutine = AnimateWindow(WelcomeHide, WelcomeUITransform, UIState.State.ARScene);
        StartCoroutine(WelcomeCoroutine);
        objectToEnable.SetActive(true);
    }
    private IEnumerator AnimateWindow(Vector2 targetPos,RectTransform transform,UIState.State targetState)
    {
        if (animating) yield break;
        animating = true;
        for (;;) //for ever is better (real)
        {
            Vector2 newPos = HelperMath.expDecay(transform.anchoredPosition, targetPos, speed, Time.deltaTime);
            if (Mathf.Abs((targetPos - newPos).magnitude) < endDistance)
            {
                transform.anchoredPosition = targetPos;
                UIState.CurrentState = targetState;
                animating = false;
                yield break;
            }
            else
            {
                transform.anchoredPosition = newPos;
                yield return null;
            }
        }
    }
}
public static class HelperMath
{
    public static float expDecay(float a, float b, float decay, float dT)
    {
        return b + (a - b) * Mathf.Exp(-decay * dT);
    }
    public static Vector2 expDecay(Vector2 a, Vector2 b, float decay ,float dT)
    {
        return b + (a - b) * Mathf.Exp(-decay * dT);
    }
    public static bool LessThan(Vector3 a, Vector3 b)
    {
        return (a.x < b.x) && (a.y < b.y) && (a.z < b.z);
    }
    public static bool GreaterThan(Vector3 a, Vector3 b)
    {
        return (a.x > b.x) && (a.y > b.y) && (a.z > b.z);
    }

}
