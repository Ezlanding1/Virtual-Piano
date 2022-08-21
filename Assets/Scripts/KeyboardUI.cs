using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardUI : MonoBehaviour
{
    [SerializeField] float speed = 50;
    float t;
    Color color;
    Color textColor;

    void Start()
    {
        color = GetComponent<Image>().color;
        textColor = GetComponentInChildren<Text>().color;
    }
    // Move to the target end position.
    void Update()
    {
        t += 0.005f * Time.deltaTime * speed;
        color.a = Mathf.Lerp(1, 0, t);
        textColor.a = Mathf.Lerp(1, 0, t);
        GetComponent<Image>().color = color;
        GetComponentInChildren<Text>().color = textColor; 
        
    }
}
