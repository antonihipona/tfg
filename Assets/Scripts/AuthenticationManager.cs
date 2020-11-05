using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;

public class AuthenticationManager : MonoBehaviour
{
    public static AuthenticationManager instance;

    public string playFabPlayerId;

    public delegate void UpdateInventoryAction();
    public static event UpdateInventoryAction OnInventoryUpdate;

    public string Username { get; set; }
    public bool InRoom { get; set; }
    public Dictionary<string, int> virtualCurrency;
    public HashSet<ItemInstance> inventoryItems;

    public string loginErrorMessage;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            instance = this;
            instance.InRoom = false;
            instance.virtualCurrency = new Dictionary<string, int>();
            instance.inventoryItems = new HashSet<ItemInstance>();
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    public void OnLoginSuccess(LoginResult result)
    {
        instance.playFabPlayerId = result.PlayFabId;
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

    public void GetUserInventory(GetUserInventoryResult res)
    {
        inventoryItems.Clear();
        instance.virtualCurrency = res.VirtualCurrency;
        foreach (var item in res.Inventory)
        {
            inventoryItems.Add(item);
        }
        OnInventoryUpdate?.Invoke();
    }

    public void UpdateUserInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), GetUserInventory, GetUserInventoryError);
    }

    public void LoadUserInventory()
    {
    }

    public void GetUserInventoryError(PlayFabError error)
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
