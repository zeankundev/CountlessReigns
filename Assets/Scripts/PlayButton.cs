using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public void StartGame()
    {
        GameObject mainMenuScreen = Utilities.FindInHierarchy("MainMenuScreenState");
        GameObject gameEntry = Utilities.FindInHierarchy("MainGameplay");
        if (mainMenuScreen != null && gameEntry != null)
        {
            mainMenuScreen.SetActive(false);
            gameEntry.SetActive(true);
        }
        else
        {
            Debug.LogError("PlayButton: Could not find MainMenu or MainGameplay in the scene.");
        }
    }
}
