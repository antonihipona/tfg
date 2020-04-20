using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PowerUpManager : MonoBehaviourPunCallbacks
{

    [Header("Images for power-ups")]
    public Sprite speedPowerUpSprite;
    public Sprite fireRatePowerUpSprite;
    public Sprite shootSpeedPowerUpSprite;
    public Sprite invisibilityPowerUpSprite;

    private PowerUp[] powerUps;

    void Start()
    {
        powerUps = GetComponentsInChildren<PowerUp>();
        if (PhotonNetwork.IsMasterClient)
            StartCoroutine("SpawnRandomPowerUp");
    }

    void Update()
    {
    }

    IEnumerator SpawnRandomPowerUp()
    {
        yield return new WaitForSeconds(15);
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < powerUps.Length; i++)
        {
            PowerUp p = powerUps[i];
            if (!p.isActive)
                availableIndices.Add(i);
        }
        if (availableIndices.Count > 0)
        {
            int availableIndex = UnityEngine.Random.Range(0, availableIndices.Count);
            var values = Enum.GetValues(typeof(PowerUpType));
            System.Random random = new System.Random();
            int powerTypeIndex = random.Next(values.Length);
            photonView.RPC("SpawnPowerUp", RpcTarget.AllBuffered, availableIndices[availableIndex], powerTypeIndex);
        }
        StartCoroutine("SpawnRandomPowerUp");
    }

    [PunRPC]
    void SpawnPowerUp(int index, int typeIndex, PhotonMessageInfo info)
    {
        PowerUp p = powerUps[index];

        var values = Enum.GetValues(typeof(PowerUpType));
        PowerUpType powerUp = (PowerUpType)values.GetValue(typeIndex);
        powerUps[index].Spawn(powerUp);
    }

}
