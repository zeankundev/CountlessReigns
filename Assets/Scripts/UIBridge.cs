using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIBridge : MonoBehaviour
{
    private TMP_Text subtitleText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        subtitleText = transform.Find("OutlineText").GetComponent<TMP_Text>();
        Debug.LogWarning(subtitleText);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DisplayText(string text, float duration)
    {
        CancelInvoke("ClearText");
        subtitleText.text = text;
        subtitleText.alpha = 1.0f;
        subtitleText.CrossFadeAlpha(1.0f, 0.5f, false);
        Invoke("ClearText", duration + 0.5f);
    }
    private void ClearText()
    {
        subtitleText.CrossFadeAlpha(0.0f, 0.5f, false);
    }
}
