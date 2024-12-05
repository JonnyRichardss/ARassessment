using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class TouchDebug : MonoBehaviour
{
    // Start is called before the first frame update
    TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {

        text = GetComponent<TMP_Text>();

    }
    public void ShowDelta(Vector2 delta)
    {
        text.SetText(string.Format("{0},{1}", Mathf.Round(delta.x ) , Mathf.Round(delta.y )));
    }
}
