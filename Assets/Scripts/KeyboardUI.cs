using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardUI : MonoBehaviour
{
    [SerializeField] float speed = 50;
    [SerializeField] float uiFadeDelaySeconds = 4;
    float UiTransparency;
    Color color;
    Color textColor;

    [SerializeField] GameObject[] UiObjectsToFade;

    void Start()
    {
        // Save initial states of keyboard ui object colors
        color = UiObjectsToFade[0].GetComponent<Image>().color;
        textColor = UiObjectsToFade[0].GetComponentInChildren<Text>().color;
        StartCoroutine(UiFade());
    }
    
    IEnumerator UiFade()
    {
        yield return new WaitForSeconds(uiFadeDelaySeconds);
        
        // Make the keyboard UI icons fade out, Lerp transparency to 0
        while (UiTransparency < 1f)
        {
            UiTransparency += 0.005f * Time.deltaTime * speed;
            color.a = Mathf.Lerp(1, 0, UiTransparency);
            textColor.a = Mathf.Lerp(1, 0, UiTransparency);

            foreach (var ui in UiObjectsToFade)
            {
                ui.GetComponent<Image>().color = color;
                ui.GetComponentInChildren<Text>().color = textColor;
            }
            yield return null;
        }
    }
}
