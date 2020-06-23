using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class StatsSection : MonoBehaviour
{
    [Header("Speed")]
    public TextMeshProUGUI speedText;
    public Slider speedSlider;

    [Header("Damage")]
    public TextMeshProUGUI damageText;
    public Slider damageSlider;

    [Header("Life")]
    public TextMeshProUGUI lifeText;
    public Slider lifeSlider;

    [Header("Total")]
    public TextMeshProUGUI totalText;

    private void OnEnable()
    {
        GetUserData();

        speedSlider.onValueChanged.AddListener(RefreshSpeedText);
        damageSlider.onValueChanged.AddListener(RefreshDamageText);
        lifeSlider.onValueChanged.AddListener(RefreshLifeText);
    }

    // From playfab https://docs.microsoft.com/en-us/gaming/playfab/features/data/playerdata/quickstart
    public void GetUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = AuthenticationManager.instance.playFabPlayerId,
            Keys = null
        }, result => {
            var speed = result.Data["speed"].Value;
            var damage = result.Data["damage"].Value;
            var life = result.Data["life"].Value;

            Debug.Log($"Speed: {speed}, Damage: {damage}, Life: {life}");
            speedText.text = speed;
            speedSlider.value = float.Parse(speed);

            damageText.text = damage;
            damageSlider.value = float.Parse(damage);

            lifeText.text = $"{float.Parse(life) * 2}";
            lifeSlider.value = float.Parse(life);
        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    private void RefreshSpeedText(float newVal)
    {
        speedText.text = $"{newVal}";
        RefreshTotal();
    }

    private void RefreshDamageText(float newVal)
    {
        damageText.text = $"{newVal}";
        RefreshTotal();
    }

    private void RefreshLifeText(float newVal)
    {
        lifeText.text = $"{newVal * 2}";
        RefreshTotal();
    }

    private void RefreshTotal()
    {
        totalText.text = $"{15 - (speedSlider.value + damageSlider.value + lifeSlider.value)}";
    }

    private float GetTotal()
    {
        var total = $"{speedSlider.value + damageSlider.value + lifeSlider.value}";
        return float.Parse(total);
    }

    // From playfab https://docs.microsoft.com/en-us/gaming/playfab/features/data/playerdata/quickstart
    public void SetUserData()
    {
        if (GetTotal() != 15)
            return;

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
            {"speed", $"{speedSlider.value}" },
            {"damage", $"{damageSlider.value}" },
            {"life", $"{lifeSlider.value}" }
        }
        },
        result => Debug.Log("Successfully set user data"),
        error =>
        {
            Debug.Log("Got error setting user data");
            Debug.Log(error.GenerateErrorReport());
        });
    }
}
