using Photon.Realtime;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UILoginManager : UIBase
{

    public TMPro.TMP_InputField inputUsername;
    public TMPro.TMP_InputField inputPassword;
    public Text errorText;

    private void Start()
    {
        DisplayErrors(AuthenticationManager.instance.loginErrorMessage);
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
        PlayFabClientAPI.LoginWithPlayFab(request, AuthenticationManager.instance.OnLoginSuccess, AuthenticationManager.instance.OnLoginFailure);
        AuthenticationManager.instance.Username = inputUsername.text;
    }

    

    public void OnClickRegister()
    {
        inputUsername.enabled = false;
        inputPassword.enabled = false;
        SceneManager.LoadScene("RegisterMenu");
    }

    public void DisplayErrors(string errors)
    {
        errorText.text = errors;
    }
}
