using UnityEngine;

public class BackButton : MonoBehaviour
{
    public void GoBackToMenu()
    {
        GameObject settingsScreen = Utilities.FindInHierarchy("Settings");
        GameObject mainMenuScreen = Utilities.FindInHierarchy("MainMenu");
        if (settingsScreen != null && mainMenuScreen != null)
        {
            settingsScreen.SetActive(false);
            mainMenuScreen.SetActive(true);
        }
        else
        {
            Debug.LogError("BackButton: Could not find Settings or MainMenu in the scene.");
        }
    }
}
