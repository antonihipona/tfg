﻿using UnityEngine;
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

    private CustomGameManager gameManager;
    private PlayerController playerController;
    private void Start() {
        gameManager = FindObjectOfType<CustomGameManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            panel.SetActive(!panel.activeSelf);
    }

    public void OnClickBack()
    {
        if (PhotonNetwork.IsConnectedAndReady){
            GameObject localPlayer = gameManager.localPlayer;
            if (localPlayer != null){
                playerController = localPlayer.GetComponent<PlayerController>();
                playerController.Leave();
            }
            PhotonNetwork.LeaveRoom();
        }
    }

}
