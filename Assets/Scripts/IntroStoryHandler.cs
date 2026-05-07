using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

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
                RawImage rawImage = GameObject.Find("RawImage").GetComponent<RawImage>();
                rawImage.CrossFadeAlpha(0f, 2f, false);
                StartCoroutine(DeactivateAfterDelay(2f));
            }
        }
    }
}
