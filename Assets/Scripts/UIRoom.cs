﻿using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;

public class UIRoom : MonoBehaviourPunCallbacks
{
    public TMPro.TextMeshProUGUI textPrefab;

    public VerticalLayoutGroup dataGroup;
    public VerticalLayoutGroup playersGroup;

    public Button startGameButton;
    void Start()
    {
        var room = PhotonNetwork.CurrentRoom;
        TMPro.TextMeshProUGUI matchName = Instantiate(textPrefab);
        TMPro.TextMeshProUGUI maxPlayers = Instantiate(textPrefab);
        TMPro.TextMeshProUGUI mapType = Instantiate(textPrefab);
        matchName.text = "Match name: " + room.Name;
        maxPlayers.text = "Players: " + room.MaxPlayers;
        mapType.text = "Map type: " + (MapType)room.CustomProperties["mapType"];
        matchName.transform.SetParent(dataGroup.transform, false);
        maxPlayers.transform.SetParent(dataGroup.transform, false);
        mapType.transform.SetParent(dataGroup.transform, false);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            startGameButton.gameObject.SetActive(true);

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Player player in players)
        {
            TMPro.TextMeshProUGUI username = Instantiate(textPrefab);
            username.text = player.NickName;
            if (player.IsMasterClient) username.text += " (room owner)";
            username.transform.SetParent(playersGroup.transform, false);
        }
    }

    public void OnClickStartGame()
    {
        // Load the game for every player
        if (PhotonNetwork.IsMasterClient)
        {
            Room room = PhotonNetwork.CurrentRoom;
            room.IsOpen = false;

            var mapType = (MapType)room.CustomProperties["mapType"];

            var level = "MapDesert";
            switch (mapType)
            {
                case MapType.Desert:
                    level = "MapDesert";
                    break;
                case MapType.Forest:
                    level = "MapForest";
                    break;
                case MapType.Snow:
                    level = "MapSnow";
                    break;
            }
            PhotonNetwork.LoadLevel(level);
        }
    }

    public void OnClickBack()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");
    }


}
