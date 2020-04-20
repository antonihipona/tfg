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
    public GameObject speedPowerUp;
    public GameObject fireRatePowerUp;
    public GameObject shootSpeedPowerUp;
    public GameObject invisibilityPowerUp;

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
