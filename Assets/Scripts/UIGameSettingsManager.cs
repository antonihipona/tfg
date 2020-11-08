using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIGameSettingsManager : UIBase
{
    public Toggle toggleFullscreen;
    public TMPro.TMP_Dropdown dropdownResolution;

    public Slider audioSlider;

    void Start()
    {
        // Fullscreen
        toggleFullscreen.isOn = Screen.fullScreen;

        // Resolutions
        dropdownResolution.ClearOptions();
        List<string> options = new List<string>();
        foreach (Resolution res in Screen.resolutions)
        {
            float aspect = (float)res.width / (float)res.height;
            float minAspect = 16f / 9f;
            if (aspect >= minAspect)
            {
                string option = res.width + "x" + res.height;
                if (!options.Contains(option))
                    options.Add(option);
            }

        }
        dropdownResolution.AddOptions(options);
        dropdownResolution.value = options.IndexOf(Screen.width + "x" + Screen.height);

        // Audio
        audioSlider.value = AudioManager.Instance.GetVolume();
        audioSlider.onValueChanged.AddListener(UpdateAudioVolume);
    }

    public void OnToggleFullscreen()
    {
        bool fullscreen = toggleFullscreen.isOn;
        Screen.SetResolution(Screen.width, Screen.height, fullscreen);
    }

    public void OnSelectResolution()
    {
        bool fullscreen = toggleFullscreen.isOn;
        string[] widthHeight = dropdownResolution.options[dropdownResolution.value].text.Split('x');
        Screen.SetResolution(int.Parse(widthHeight[0]), int.Parse(widthHeight[1]), fullscreen);
    }

    public void OnClickBack()
    {
        if (AuthenticationManager.instance.playFabPlayerId == null || AuthenticationManager.instance.playFabPlayerId == "")
            SceneManager.LoadScene("LoginMenu");
        else
            SceneManager.LoadScene("MainMenu");
    }

    private void UpdateAudioVolume(float value)
    {
        AudioManager.Instance.SetVolume(value);
    }
}
