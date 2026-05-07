using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class IStoryCueHandler
{
    public float startTime;
    public float endTime;
    public string message;
}

public class IntroStoryHandler : MonoBehaviour
{
    [SerializeField]
    public List<IStoryCueHandler> storyCues = new List<IStoryCueHandler>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
