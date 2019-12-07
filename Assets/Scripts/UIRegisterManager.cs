using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIRegisterManager : UIBase
{
    public TMPro.TMP_InputField inputUsername;
    public TMPro.TMP_InputField inputPassword;

    public void OnClickRegister()
    {
        var request = new RegisterPlayFabUserRequest {
            Username = inputUsername.text,
            Password = inputPassword.text,
            RequireBothUsernameAndEmail = false,
            TitleId = PlayFabSettings.TitleId 
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    private void OnRegisterFailure(PlayFabError obj)
    {
        Debug.LogWarning("Something went wrong with a registration.");
        Debug.LogError(obj.GenerateErrorReport());
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult obj)
    {
        Debug.Log("Succesfully registered!");
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("LoginMenu");
    }

}
