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


    private GameObject localPlayer;
    private double spawnTime;
    private bool spawned;
    void Start()
    {
        spawnTime = PhotonNetwork.Time + 5;
        spawned = false;
        gameStarted = false;
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
}
