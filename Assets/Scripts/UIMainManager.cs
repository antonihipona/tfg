﻿using Photon.Pun;
using UnityEngine.SceneManagement;


public class UIMainManager : UIBase
{

    public void OnClickSearchMatch()
    {
        // We need to join the lobby
        PhotonNetwork.JoinLobby();
    }

    public void OnClickCreateMatch()
    {
        SceneManager.LoadScene("CreateMatchMenu");
    }

    public void OnClickCustomize()
    {
        // SceneManager.LoadScene("CustomizeMenu");
    }

    public void OnClickGameSettings()
    {
        SceneManager.LoadScene("GameSettingsMenu");
    }

}