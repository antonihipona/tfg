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


    // source https://docs.microsoft.com/en-us/gaming/playfab/features/social/tournaments-leaderboards/quickstart
    public void AddLeaderboardPoints(int playerScore)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> {
            new StatisticUpdate {
                StatisticName = "Points",
                Value = playerScore
            }
        }
        }, result => OnStatisticsUpdated(result), FailureCallback);
    }

    private void OnStatisticsUpdated(UpdatePlayerStatisticsResult updateResult)
    {
        Debug.Log("Successfully submitted high score");
    }

    private void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
}
