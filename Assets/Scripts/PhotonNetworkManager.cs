using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public enum MapType : int { Desert, Snow, Forest };

public class PhotonNetworkManager : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public static PhotonNetworkManager Instance;
    public List<RoomInfo> roomList;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        Instance.roomList = new List<RoomInfo>();
        DontDestroyOnLoad(this.gameObject);
    }

    public void CreateRoom(string text, MapType mapType, byte playerNumber)
    {
        Debug.Log("Created room: Map: " + mapType + ", Players: " + playerNumber);
        if (PhotonNetwork.IsConnected)
        {
            Hashtable customProperties = new Hashtable();
            customProperties.Add("mapType", mapType);
            var options = new RoomOptions()
            {
                CustomRoomPropertiesForLobby = new string[] { "mapType" },
                CustomRoomProperties = customProperties,
                IsVisible = true,
                IsOpen = true,
                MaxPlayers = playerNumber
            };
            PhotonNetwork.CreateRoom(text, options);
        }
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("SearchMatchMenu");
        Debug.Log("PUN: Joined lobby!");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN: Successfully connected to Master!");
        if (!AuthenticationManager.Instance.InRoom)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            AuthenticationManager.Instance.InRoom = false;
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("PUN: OnCreatedRoom() called by PUN.");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        UICreateMatch uiLoginManager = FindObjectOfType<UICreateMatch>();
        if (uiLoginManager != null)
        {
            uiLoginManager.textError.gameObject.SetActive(true);
            uiLoginManager.textError.text = message;
        }
        Debug.LogError("PUN: OnCreateRoomFailed() called by PUN. " + message);
    }

    public override void OnJoinedRoom()
    {
        AuthenticationManager.Instance.InRoom = true;
        Debug.Log("PUN: OnJoinedRoom() called by PUN. Now this client is in a room.");
        SceneManager.LoadScene("RoomMenu");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("PUN: OnJoinRoomFailed called by PUN. " + message);
    }

    public override void OnLeftRoom()
    {
        roomList.Clear();
        AuthenticationManager.Instance.InRoom = false;
        Debug.Log("PUN: Room left");
        SceneManager.LoadScene("MainMenu");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room list updated!");
        UpdateCachedRoomList(roomList);
        UISearchMatch manager = FindObjectOfType<UISearchMatch>();
        if (manager != null)
            manager.UpdateUI();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UIRoom manager = FindObjectOfType<UIRoom>();
        if (manager != null)
        {
            Player[] players = PhotonNetwork.PlayerList;

            foreach (Transform child in manager.playersGroup.transform)
            {
                Destroy(child.gameObject);
            }
            if (PhotonNetwork.CurrentRoom.MaxPlayers == players.Length)
                manager.startGameButton.interactable = true;
            foreach (Player player in players)
            {
                TMPro.TextMeshProUGUI username = Instantiate(manager.textPrefab);
                username.text = player.NickName;
                if (player.IsMasterClient) username.text += " (room owner)";
                username.transform.SetParent(manager.playersGroup.transform, false);
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UIRoom manager = FindObjectOfType<UIRoom>();
        if (manager != null)
        {
            Player[] players = PhotonNetwork.PlayerList;

            foreach (Transform child in manager.playersGroup.transform)
            {
                Destroy(child.gameObject);
            }
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                manager.startGameButton.gameObject.SetActive(true);
                manager.startGameButton.interactable = false;
            }
            foreach (Player player in players)
            {
                TMPro.TextMeshProUGUI username = Instantiate(manager.textPrefab);
                username.text = player.NickName;
                username.transform.SetParent(manager.playersGroup.transform, false);
            }
        }
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList) // Source https://forum.photonengine.com/discussion/12782/pun-2-problem
    {
        foreach (RoomInfo room in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList || room.PlayerCount == 0)
            {
                if (Instance.roomList.Contains(room))
                {
                    Instance.roomList.Remove(room);
                }
                continue;
            }
            if (Instance.roomList.Contains(room))
                Instance.roomList.Remove(room);
            Instance.roomList.Add(room);
        }
    }
}
