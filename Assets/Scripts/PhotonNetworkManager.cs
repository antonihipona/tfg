﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public enum MapType : int { Desert, Snow, Forest };

public class PhotonNetworkManager : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public static PhotonNetworkManager instance;
    public List<RoomInfo> roomList;

    void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void CreateRoom(string text, MapType mapType, byte playerNumber)
    {
        Debug.Log(mapType + " " + playerNumber);
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
        if (!AuthenticationManager.instance.InRoom)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            AuthenticationManager.instance.InRoom = false;
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("PUN: OnCreatedRoom() called by PUN.");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("PUN: OnCreateRoomFailed() called by PUN. " + message);
    }

    public override void OnJoinedRoom()
    {
        AuthenticationManager.instance.InRoom = true;
        Debug.Log("PUN: OnJoinedRoom() called by PUN. Now this client is in a room.");
        SceneManager.LoadScene("RoomMenu");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("PUN: Room left");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        this.roomList = roomList;

        UISearchMatchManager manager = FindObjectOfType<UISearchMatchManager>();
        if (manager != null)
            manager.UpdateUI();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UIRoomManager manager = FindObjectOfType<UIRoomManager>();
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
                username.transform.SetParent(manager.playersGroup.transform, false);
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UIRoomManager manager = FindObjectOfType<UIRoomManager>();
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

    
}