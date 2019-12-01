using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIGameSettingsManager : UIBase
{
    public Toggle toggleFullscreen;
    public TMPro.TMP_Dropdown dropdownResolution;


    void Start()
    {
        toggleFullscreen.isOn = Screen.fullScreen;
        dropdownResolution.ClearOptions();
        List<string> options = new List<string>();
        foreach (Resolution res in Screen.resolutions)
        {
            if (res.width > 600 && res.width / 16 * 9 == res.height)
            {
                string option = res.width + "x" + res.height;
                if (!options.Contains(option))
                    options.Add(option);
            }

        }
        dropdownResolution.AddOptions(options);
        dropdownResolution.value = options.IndexOf(Screen.width + "x" + Screen.height);
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
        SceneManager.LoadScene("MainMenu");
    }
}
