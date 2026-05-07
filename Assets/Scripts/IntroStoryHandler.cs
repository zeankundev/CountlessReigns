using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public enum CreditureType
{
    Image,
    Text,
}

[System.Serializable]
public class Crediture
{
    public float startTime;
    public float duration;
    public CreditureType creditureType;
    public string role;
    public string content;
    public Sprite imageContentIfAvailable;
}

[System.Serializable]
public class IStoryCueHandler
{
    public float startTime;
    public float endTime;
    public string message;
}

[RequireComponent(typeof(AudioSource))]

public class IntroStoryHandler : MonoBehaviour
{
    [SerializeField]
    public List<IStoryCueHandler> storyCues = new List<IStoryCueHandler>();
    public float alphaDuration = 0.25f;
    public GameObject mainGameEntry;
    [SerializeField]
    public List<Crediture> credits = new List<Crediture>();
    private TMP_Text storyText;
    private AudioSource narrationSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        narrationSource = GetComponent<AudioSource>();
        GameObject storyTextObj = GameObject.Find("StoryText");
        if (storyTextObj != null)
        {
            storyText = storyTextObj.GetComponent<TMP_Text>();
            storyText.CrossFadeAlpha(0f, 0f, false);
        }
        if (narrationSource != null)
        {
            narrationSource.Play();
        }
    }

    IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < storyCues.Count; i++)
        {
            if (narrationSource.time >= storyCues[i].startTime && narrationSource.time <= storyCues[i].endTime)
            {
                storyText.text = storyCues[i].message;
                storyText.CrossFadeAlpha(1f, alphaDuration, false);
                // After that, 0.25 seconds before the end time, start fading out
                if (narrationSource.time >= storyCues[i].endTime - alphaDuration)
                {
                    storyText.CrossFadeAlpha(0f, alphaDuration, false);
                }
            }
            // If this is the last cue and the narration has finished, transition to the main game
            if (i == storyCues.Count - 1 && narrationSource.time >= narrationSource.clip.length)
            {
                mainGameEntry.SetActive(true);
                // fade out the raw image
                Image rawImage = GameObject.Find("RawImage").GetComponent<Image>();
                rawImage.CrossFadeAlpha(0f, 2f, false);
                StartCoroutine(DeactivateAfterDelay(2f));
                GameObject.Find("RawImage").SetActive(false);
            }
        }
        // Run the credits the same thread along with the story cues
        for (int i = 0; i < credits.Count; i++)
        {
            float elapsed = narrationSource.time;
            float start = credits[i].startTime;
            float end = credits[i].startTime + credits[i].duration;

            // Find the hierarchy objects
            GameObject creditureThing = GameObject.Find("CreditureThing");
            if (creditureThing == null) continue;

            TMP_Text roleText = creditureThing.transform.Find("TextSub")?.GetComponent<TMP_Text>();
            Transform subSel = creditureThing.transform.Find("Crediture_SubSel");
            Image contentImage = subSel?.Find("Image")?.GetComponent<Image>();
            TMP_Text contentText = subSel?.Find("Text")?.GetComponent<TMP_Text>();

            if (elapsed >= start && elapsed <= end)
            {
                // Populate content on first frame of entry
                if (roleText != null) roleText.text = credits[i].role;

                if (credits[i].creditureType == CreditureType.Image)
                {
                    if (contentImage != null)
                    {
                        contentImage.gameObject.SetActive(true);
                        if (credits[i].imageContentIfAvailable != null)
                            contentImage.sprite = credits[i].imageContentIfAvailable;
                    }
                    if (contentText != null) contentText.gameObject.SetActive(false);
                }
                else
                {
                    if (contentText != null)
                    {
                        contentText.gameObject.SetActive(true);
                        contentText.text = credits[i].content;
                    }
                    if (contentImage != null) contentImage.gameObject.SetActive(false);
                }

                // Fade in at start
                if (elapsed < start + alphaDuration)
                {
                    roleText?.CrossFadeAlpha(1f, alphaDuration, false);
                    contentImage?.CrossFadeAlpha(1f, alphaDuration, false);
                    contentText?.CrossFadeAlpha(1f, alphaDuration, false);
                }

                // Fade out near end
                if (elapsed >= end - alphaDuration)
                {
                    roleText?.CrossFadeAlpha(0f, alphaDuration, false);
                    contentImage?.CrossFadeAlpha(0f, alphaDuration, false);
                    contentText?.CrossFadeAlpha(0f, alphaDuration, false);
                }
            }
        }
    }
}
