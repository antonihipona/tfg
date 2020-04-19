using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UIGameManager : UIBase
{
    public GameObject panel;
    public Text endText;
    public Text messageText;

    public Image shootCooldown;
    public Image bombCooldown;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            panel.SetActive(!panel.activeSelf);
    }

    public void OnClickBack()
    {
        if (PhotonNetwork.IsConnectedAndReady)
            PhotonNetwork.LeaveRoom();
    }

}
