using System;
using UnityEngine;

public class SettingsModifier : MonoBehaviour
{
    private string[] resolutionOptions = {"640x480", "960x480", "1024x768", "1280x720", "1366x768", "1600x900", "1920x1080", "2560x1440", "3840x2160"};
    public void SaveGraphicsSettings(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("GraphicsQuality", qualityIndex);
        PlayerPrefs.Save();
    }
    public void ToggleFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex < 0 || resolutionIndex >= resolutionOptions.Length)
        {
            Debug.LogError("Invalid resolution index: " + resolutionIndex);
            return;
        }

        string[] dimensions = resolutionOptions[resolutionIndex].Split('x');
        int width = int.Parse(dimensions[0]);
        int height = int.Parse(dimensions[1]);

        Screen.SetResolution(width, height, Screen.fullScreen);
        PlayerPrefs.SetInt("Resolution", resolutionIndex);
        PlayerPrefs.Save();
    }
}
