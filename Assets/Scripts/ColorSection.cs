﻿using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

public enum SectionType { Body, Turret};

public class ColorSection : MonoBehaviour
{
    public SectionType Section;

    public GameObject colorPrefab;
    public GameObject target;
    private CustomizableColor _currentSelectedColor;
    private UICustomization _uiCustomizationManager;

    private string _turretColor;
    private string _bodyColor;

    private void OnEnable()
    {
        _uiCustomizationManager = FindObjectOfType<UICustomization>();
        GetUserData();
    }

    public void ChangeSelected(CustomizableColor _color)
    {
        if (_currentSelectedColor != null)
            _currentSelectedColor.SetSelected(false);
        _color.SetSelected(true);
        _currentSelectedColor = _color;
    }

    private void AddColors(GetUserInventoryResult res)
    {
        ClearChildren();

        // Add default (white)
        var colorGameObject = colorPrefab;
        colorGameObject.GetComponent<CustomizableColor>().color = Color.white;
        var colorGO = Instantiate(colorGameObject, transform);
        var customizableColor = colorGO.GetComponent<CustomizableColor>();
        customizableColor.target = target;
        customizableColor.itemData = new UICustomization.ItemData
        {
            itemId = "color_white"
        };
        SetColorActive(customizableColor, "color_white");

        for (int i = 0; i < res.Inventory.Count; i++)
        {
            var item = res.Inventory[i];
            _uiCustomizationManager.inventoryItemsIds.Add(item.ItemId);

            colorGameObject = colorPrefab;
            colorGameObject.GetComponent<CustomizableColor>().color = GameController.MapIdToColor(item.ItemId);
            colorGO = Instantiate(colorGameObject, transform);
            customizableColor = colorGO.GetComponent<CustomizableColor>();
            customizableColor.target = target;
            customizableColor.itemData = new UICustomization.ItemData
            {
                itemId = item.ItemId
            };

            SetColorActive(customizableColor, item.ItemId);

        }
    }

    private void ClearChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }

    private void GetUserInventoryError(PlayFabError error)
    {
        Debug.LogError("Could not get player inventory" + error.ErrorMessage);
    }

    private void SetColorActive(CustomizableColor color, string colorName)
    {
        if (Section == SectionType.Turret && _turretColor == colorName)
        {
            ChangeSelected(color);
        }
        else if (Section == SectionType.Body && _bodyColor == colorName)
        {
            ChangeSelected(color);
        }
    }

    // From playfab https://docs.microsoft.com/en-us/gaming/playfab/features/data/playerdata/quickstart
    public void GetUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = AuthenticationManager.Instance.playFabPlayerId,
            Keys = null
        }, result =>
        {
            _turretColor = result.Data["turret_color"].Value;
            _bodyColor = result.Data["body_color"].Value;
            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), AddColors, GetUserInventoryError);
        }, (error) =>
        {
            Debug.Log("Got error retrieving user color data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    // From playfab https://docs.microsoft.com/en-us/gaming/playfab/features/data/playerdata/quickstart
    public void SetUserColors()
    {
        UpdateUserDataRequest request = null;
        if (Section == SectionType.Turret)
        {
            request = new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>() {
                    {"turret_color", _currentSelectedColor.itemData.itemId}
                }
            };
        }
        else if (Section == SectionType.Body)
        {
            request = new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>() {
                    {"body_color", _currentSelectedColor.itemData.itemId}
                }
            };
        }

        PlayFabClientAPI.UpdateUserData(request,
        result => Debug.Log("Successfully set user data"),
        error =>
        {
            Debug.Log("Got error setting user data");
            Debug.Log(error.GenerateErrorReport());
        });
    }
}
