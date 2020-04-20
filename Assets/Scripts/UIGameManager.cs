using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UIGameManager : UIBase
{
    public GameObject panel;
    public Text endText;
    public Text messageText;

    [Header("Cooldown")]
    public Image shootCooldown;
    public Image bombCooldown;

    [Header("Power-ups")]
    public Image speedPowerUp;
    public Image fireRatePowerUp;
    public Image shootSpeedPowerUp;
    public Image invisibilityPowerUp;

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
