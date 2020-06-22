using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UIRegisterManager : UIBase
{
    public TMPro.TMP_InputField inputUsername;
    public TMPro.TMP_InputField inputPassword;
    public Text textMessage;

    public void OnClickRegister()
    {
        inputUsername.enabled = false;
        inputPassword.enabled = false;
        var request = new RegisterPlayFabUserRequest
        {
            Username = inputUsername.text,
            Password = inputPassword.text,
            RequireBothUsernameAndEmail = false,
            TitleId = PlayFabSettings.TitleId
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        inputUsername.enabled = true;
        inputPassword.enabled = true;
        float r = 1f, g = 0.4f, b = 0.4f; // Red color
        textMessage.color = new Color(r, g, b);
        textMessage.text = "";
        var details = error.ErrorDetails;
        if (details != null)
        {
            foreach (var list in details.Values)
            {
                if (list[0].Contains("must be defined")) continue;
                textMessage.text += list[0];
            }
        }
        else
            textMessage.text = error.ErrorMessage;
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult obj)
    {
        InitializeUserData();

        float r = 0.4f, g = 1f, b = 0.4f; // Green color
        textMessage.color = new Color(r, g, b);
        var successMessage = "Succesfully registered!";
        textMessage.text = successMessage;

        // Initialize player data
        Debug.Log(successMessage);
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("LoginMenu");
    }

    // From playfab https://docs.microsoft.com/en-us/gaming/playfab/features/data/playerdata/quickstart
    private void InitializeUserData()
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
            {"speed", "5"},
            {"damage", "5"},
            {"life", "10" }
        }
        },
        result => Debug.Log("Successfully set user data"),
        error =>
        {
            Debug.Log("Got error setting user data");
            Debug.Log(error.GenerateErrorReport());
        });
    }

}
