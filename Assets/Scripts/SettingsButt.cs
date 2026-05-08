using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    public void OpenSettings()
    {
        GameObject mainMenuScreen = Utilities.FindInHierarchy("MainMenu");
        GameObject settingsScreen = Utilities.FindInHierarchy("Settings");
        if (mainMenuScreen != null && settingsScreen != null)
        {
            mainMenuScreen.SetActive(false);
            settingsScreen.SetActive(true);
        }
        else
        {
            Debug.LogError("SettingsButton: Could not find MainMenu or Settings in the scene.");
        }
    }
}
