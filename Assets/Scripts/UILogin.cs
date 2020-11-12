using Photon.Realtime;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UILogin : UIBase
{
    public TMPro.TMP_InputField inputUsername;
    public TMPro.TMP_InputField inputPassword;
    public Text errorText;

    private void Start()
    {
        DisplayErrors(AuthenticationManager.Instance.loginErrorMessage);
    }
    public void OnClickLogin()
    {
        var request = new LoginWithPlayFabRequest
        {
            Username = inputUsername.text,
            Password = inputPassword.text,
            TitleId = PlayFabSettings.TitleId
        };
        SceneManager.LoadScene("ConnectingScreen");
        PlayFabClientAPI.LoginWithPlayFab(request, AuthenticationManager.Instance.OnLoginSuccess, AuthenticationManager.Instance.OnLoginFailure);
        AuthenticationManager.Instance.Username = inputUsername.text;
    }

    public void OnClickRegister()
    {
        inputUsername.enabled = false;
        inputPassword.enabled = false;
        SceneManager.LoadScene("RegisterMenu");
    }

    public void DisplayErrors(string errors)
    {
        if (errors.Length != 0) errorText.gameObject.SetActive(true);
        errorText.text = errors;
    }
    public void OnClickGameSettings()
    {
        SceneManager.LoadScene("GameSettingsMenu");
    }
}
