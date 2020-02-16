using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;

public class AuthenticationManager : MonoBehaviour
{
    public static AuthenticationManager instance;

    //private string _playFabPlayerIdCache;

    public string Username { get; set; }
    public bool InRoom { get; set; }

    public string loginErrorMessage;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            instance = this;
            instance.InRoom = false;
        }
    }

    public void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login success.");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.NickName = Username;
    }

    public void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        loginErrorMessage = "";
        var details = error.ErrorDetails;
        if (details != null)
            foreach (var list in details.Values)
                loginErrorMessage += list[0];
        else
            loginErrorMessage = error.ErrorMessage;
        SceneManager.LoadScene("LoginMenu");
    }

}
