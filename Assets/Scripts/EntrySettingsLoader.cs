using UnityEngine;

public class EntrySettingsLoader : MonoBehaviour
{
    private string[] resolutionOptions = {"640x480", "960x480", "1024x768", "1280x720", "1366x768", "1600x900", "1920x1080", "2560x1440", "3840x2160"};

    void Start()
    {
        // Load graphics quality
        int qualityIndex = PlayerPrefs.GetInt("GraphicsQuality", QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(qualityIndex);

        // Load fullscreen setting
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        Screen.fullScreen = isFullscreen;

        // Load resolution setting
        int resolutionIndex = PlayerPrefs.GetInt("Resolution", 0);
        if (resolutionIndex >= 0 && resolutionIndex < resolutionOptions.Length)
        {
            string[] dimensions = resolutionOptions[resolutionIndex].Split('x');
            int width = int.Parse(dimensions[0]);
            int height = int.Parse(dimensions[1]);
            Screen.SetResolution(width, height, Screen.fullScreen);
        }
    }
}
