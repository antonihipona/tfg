﻿using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISearchMatchManager : UIBase
{
    public TMPro.TMP_InputField inputSearch;
    public TMPro.TextMeshProUGUI textPrefab;
    public VerticalLayoutGroup listContent;
    public Button btnJoinPrefab;
    public HorizontalLayoutGroup horizontalLayoutGroupPrefab;

    public void Start()
    {
        foreach (Transform child in listContent.transform)
        {
            Destroy(child.gameObject);
        }
        // Add content
        foreach (RoomInfo room in PhotonNetworkManager.instance.roomList)
        {
            TMPro.TextMeshProUGUI matchName = Instantiate(textPrefab);
            TMPro.TextMeshProUGUI players = Instantiate(textPrefab);
            TMPro.TextMeshProUGUI mapType = Instantiate(textPrefab);
            Button btnJoin = Instantiate(btnJoinPrefab);
            HorizontalLayoutGroup horizontalLayoutGroup = Instantiate(horizontalLayoutGroupPrefab);

            matchName.text = room.Name;
            players.text = room.PlayerCount + "/" + room.MaxPlayers;
            mapType.text = room.CustomProperties["mapType"].ToString();

            horizontalLayoutGroup.transform.SetParent(listContent.transform, false);
            matchName.transform.SetParent(horizontalLayoutGroup.transform, false);
            players.transform.SetParent(horizontalLayoutGroup.transform, false);
            mapType.transform.SetParent(horizontalLayoutGroup.transform, false);
            btnJoin.onClick.AddListener(delegate { joinRoom(matchName.text); });
            btnJoin.transform.SetParent(horizontalLayoutGroup.transform, false);      
        }
    }

    public void joinRoom(string name)
    {
        PhotonNetwork.JoinRoom(name);
    }
    public void OnClickSearch()
    {
        // DoSearch(inputSearch.text);
    }

    public void OnClickBack()
    {
        // We leave lobby
        PhotonNetwork.LeaveLobby();
        SceneManager.LoadScene("MainMenu");
    }

}
