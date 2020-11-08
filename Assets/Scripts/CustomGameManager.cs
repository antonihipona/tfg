using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGameManager : MonoBehaviourPunCallbacks
{

    public GameObject playerPrefab;
    public GameObject[] spawnPoints;
    public TMPro.TextMeshProUGUI textCountdown;
    public bool gameStarted { get; private set; }
    public bool gameEnded { get; private set; }
    public GameObject localPlayer;

    private double spawnTime;
    private bool spawned;
    private int deadPlayerNumber = 0;
    private UIGameManager uiGameManager;
    void Start()
    {
        PlayAudio();
        uiGameManager = FindObjectOfType<UIGameManager>();
        spawnTime = PhotonNetwork.Time + 5;
        spawned = false;
        gameStarted = false;
        gameEnded = false;
    }

    // Update is called once per frame
    void Update()
    {
        SpawnAndCountdown();
    }

    private void SpawnAndCountdown()
    {
        if (!spawned)
        {
            spawned = true;
            for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                if (PhotonNetwork.PlayerList[i].Equals(PhotonNetwork.LocalPlayer))
                {
                    localPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPoints[i].transform.position, Quaternion.identity, 0);
                    // Rotate all children instead of object because we want to track chasis and turret rotation independently
                    for (int j = 0; j < localPlayer.transform.childCount; j++)
                    {
                        localPlayer.transform.GetChild(j).rotation = spawnPoints[i].transform.rotation;
                    }
                }
            }
        }
        double timeleft = spawnTime - PhotonNetwork.Time;
        if (timeleft <= 0)
        {
            textCountdown.gameObject.SetActive(false);
            gameStarted = true;
        }
        else
        {
            textCountdown.text = Mathf.RoundToInt((float)timeleft).ToString();
        }
    }

    public void IncreaseDeadPlayers()
    {
        deadPlayerNumber += 1;
        CheckGameEnded(false);
    }

    public void CheckGameEnded(bool isLeaving)
    {
        int playersInRoom = (int)PhotonNetwork.CurrentRoom.PlayerCount;
        if (isLeaving)
            playersInRoom -= 1;
        if (deadPlayerNumber == playersInRoom - 1)
        {
            gameEnded = true;

            if (localPlayer != null)
            {
                PlayerStats playerStats = localPlayer.GetComponent<PlayerStats>();
                if (!playerStats.IsDead())
                {
                    AuthenticationManager.instance.AddSBCurrency(50);
                    float r = 0.4f, g = 1f, b = 0.4f; // Green color
                    uiGameManager.endText.color = new Color(r, g, b);
                    uiGameManager.endText.text = "VICTORY";
                    uiGameManager.messageText.text = "You won 50 SB";
                }
            }
            uiGameManager.panel.SetActive(true);
        }
    }

    public static Color MapIdToColor(string colorId)
    {
        Color res = Color.white;
        switch (colorId)
        {
            case "color_red":
                res = Color.red;
                break;
            case "color_blue":
                res = Color.blue;
                break;
            case "color_green":
                res = Color.green;
                break;
            case "color_pink":
                res = Color.magenta;
                break;
        }
        return res;
    }

    private void PlayAudio()
    {
        var mapType = (MapType)PhotonNetwork.CurrentRoom.CustomProperties["mapType"];

        switch (mapType)
        {
            case MapType.Desert:
                AudioManager.Instance.PlayDesertTheme();
                break;
            case MapType.Forest:
                AudioManager.Instance.PlayForestTheme();
                break;
            case MapType.Snow:
                AudioManager.Instance.PlaySnowTheme();
                break;
        }
    }
}
