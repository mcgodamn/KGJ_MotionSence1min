using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordSetter : MonoBehaviour
{
    [SerializeField]
    Image bg;

    [SerializeField]
    Text text;

    RectTransform bgRT;
    void Awake()
    {
        bgRT = bg.GetComponent<RectTransform>();
    }

    public void SetText(string _s)
    {
        text.text = _s;
        var width = _s.Length;
        width = (width-1) * 400 + 580;
        bgRT.sizeDelta = new Vector2(width,bgRT.sizeDelta.y);
    }
}
