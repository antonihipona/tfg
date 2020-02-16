using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameManager : UIBase
{
    public GameObject playerUIPrefab;

    private PlayerStats playerStats;
    private GameObject playerUI;
    void Start()
    {
        playerStats = FindObjectOfType<CustomGameManager>().playerPrefab.GetComponent<PlayerStats>();
        // Instantiate UI
        SetupUI();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    void SetupUI()
    {
        var playerStatsPos = Camera.main.WorldToScreenPoint(transform.position) + Vector3.forward * 5;
        playerUI = Instantiate(playerUIPrefab, playerStatsPos, Quaternion.identity);
        playerUI.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }

    void UpdateUI()
    {
        var playerStatsPos = Camera.main.WorldToScreenPoint(transform.position) + Vector3.forward * 5;
        playerUI.transform.position = playerStatsPos;
    }
}
