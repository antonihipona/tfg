using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Realtime;

public class UIRoomManager : MonoBehaviourPunCallbacks
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
        matchName.text = "Match name:" + room.Name;
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
            username.transform.SetParent(playersGroup.transform, false);
        }
    }

    public void OnClickBack()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("CreateMatchMenu");
    }


}
