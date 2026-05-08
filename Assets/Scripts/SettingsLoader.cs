using UnityEngine;
using UnityEngine.UI;

public enum InputType
{
    Dropdown,
    Checkbox
}

public class SettingsLoader : MonoBehaviour
{
    public string settingKey;
    public InputType inputType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // We don't know if it's a certain type, it may be either float or bool
        if (inputType == InputType.Dropdown)
        {
            Dropdown dropdown = gameObject.GetComponent<Dropdown>();
            if (dropdown != null)
            {
                int value = PlayerPrefs.GetInt(settingKey, 0);
                dropdown.value = value;
            }
        }
        else if (inputType == InputType.Checkbox)
        {
            Toggle toggle = gameObject.GetComponent<Toggle>();
            if (toggle != null)
            {
                int value = PlayerPrefs.GetInt(settingKey, 0);
                toggle.isOn = value == 1;
            }
        }
    }
}
