using System.Collections.Generic;
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
    public List<ItemInstance> playerInventory;
    public Dictionary<string, int> virtualCurrency;

    public string loginErrorMessage;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            instance = this;
            instance.InRoom = false;
            instance.playerInventory = new List<ItemInstance>();
            instance.virtualCurrency = new Dictionary<string, int>();
        }
    }

    public void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login success.");
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), GetUserInventory, GetUserInventoryError);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.NickName = Username;
    }

    public void OnLoginFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
        loginErrorMessage = "";
        var details = error.ErrorDetails;
        if (details != null)
            foreach (var list in details.Values)
                loginErrorMessage += list[0];
        else
            loginErrorMessage = error.ErrorMessage;
        SceneManager.LoadScene("LoginMenu");
    }

    private void GetUserInventory(GetUserInventoryResult res)
    {
        instance.playerInventory.Clear();
        instance.virtualCurrency = res.VirtualCurrency;
        foreach (var eachItem in res.Inventory)
        {
            instance.playerInventory.Add(eachItem);
        }
    }

    private void GetUserInventoryError(PlayFabError error)
    {
        Debug.LogError("Could not get player inventory" + error.ErrorMessage);
    }

    public void AddSBCurrency(int amount)
    {
        var request = new AddUserVirtualCurrencyRequest
        {
            Amount = amount,
            VirtualCurrency = "SB"
        };
        PlayFabClientAPI.AddUserVirtualCurrency(request, AddCurrencySuccess, AddCurrencyError);
    }

    private void AddCurrencySuccess(ModifyUserVirtualCurrencyResult result)
    {
        instance.virtualCurrency["SB"] = result.Balance;
    }

    private void AddCurrencyError(PlayFabError error)
    {
        Debug.LogError("Could not add player currency" + error.ErrorMessage);
    }
}
