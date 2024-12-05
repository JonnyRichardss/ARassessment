using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchHandler : MonoBehaviour
{
    public float AngleLeniencyDeg = 10;
    public float RequiredSpeed = 2;
    public UIScripting UI;
    public TouchDebug touchDebug;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(UI != null,"CRITICAL - TOUCHHANDLER HAS NO UI SCRIPT ASSIGNED");
    }

    void OnDelta(InputValue val)
    {
        Vector2 delta = val.Get<Vector2>();
        if (touchDebug)  touchDebug.ShowDelta(delta);

        if (delta.magnitude < RequiredSpeed) return;
        else if (Vector2.Angle(Vector2.left  ,delta) < AngleLeniencyDeg) UI.ShowBadges();
        else if (Vector2.Angle(Vector2.right ,delta) < AngleLeniencyDeg) UI.HideBadges();
        else if (Vector2.Angle(Vector2.up    ,delta) < AngleLeniencyDeg) UI.ShowWelcome();
        else if (Vector2.Angle(Vector2.down  ,delta) < AngleLeniencyDeg) UI.HideWelcome();

    }
}
