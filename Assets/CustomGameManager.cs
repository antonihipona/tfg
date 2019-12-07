using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGameManager : MonoBehaviourPunCallbacks
{

    public GameObject playerPrefab;
    public GameObject[] spawnPoints;
    public TMPro.TextMeshProUGUI textCountdown;

    private GameObject localPlayer;
    private double spawnTime;
    private bool spawned;
    void Start()
    {
        spawnTime = PhotonNetwork.Time + 5;
        spawned = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawned)
        {
            spawned = true;
            for (int i = 0; i < PhotonNetwork.CountOfPlayers; i++)
            {
                if (PhotonNetwork.PlayerList[i].Equals(PhotonNetwork.LocalPlayer))
                {
                    localPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPoints[i].transform.position, spawnPoints[i].transform.rotation, 0);
                }
            }
        }
        double timeleft = spawnTime - PhotonNetwork.Time;
        if (timeleft <= 0)
        {
            textCountdown.gameObject.SetActive(false);
        }
        else
        {
            textCountdown.text = Mathf.RoundToInt((float)timeleft).ToString();
        }
    }
}
