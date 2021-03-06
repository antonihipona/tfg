﻿using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class BuyColorSection : MonoBehaviour
{
    public TextMeshProUGUI text;

    public GameObject colorPrefab;
    public GameObject target;

    private BuyableColor _currentSelectedColor;
    private UICustomization _uiCustomizationManager;

    private void OnEnable()
    {
        if (_currentSelectedColor != null)
            text.text = "Price: " + _currentSelectedColor.itemData.sbPrice + " SB";
        _uiCustomizationManager = FindObjectOfType<UICustomization>();
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), AddColors, GetCatalogItemsError);
    }

    public void ChangeSelected(BuyableColor _color)
    {
        if (_currentSelectedColor != null)
            _currentSelectedColor.SetSelected(false);
        _color.SetSelected(true);
        _currentSelectedColor = _color;

        text.text = "Price: " + _color.itemData.sbPrice + " SB";
    }

    private void AddColors(GetCatalogItemsResult res)
    {
        ClearChildren();

        for (int i = 0; i < res.Catalog.Count; i++)
        {
            var item = res.Catalog[i];
            if (_uiCustomizationManager.inventoryItemsIds.Contains(item.ItemId))
                continue;
            var colorGameObject = colorPrefab;
            colorGameObject.GetComponent<BuyableColor>().color = GameController.MapIdToColor(item.ItemId);
            
            var colorInstance = Instantiate(colorGameObject, transform);
            var buyableColor = colorInstance.GetComponent<BuyableColor>();
            buyableColor.target = target;
            buyableColor.itemData = new UICustomization.ItemData
            {
                sbPrice = item.VirtualCurrencyPrices["SB"],
                itemId = item.ItemId
            };
        }
    }

    private void ClearChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }

    private void GetCatalogItemsError(PlayFabError error)
    {
        Debug.LogError("Could not get catalog items: " + error.ErrorMessage);
    }

    public void PurchaseSelectedColor()
    {
        if (_currentSelectedColor == null)
            return;
        var request = new PurchaseItemRequest
        {
            ItemId = _currentSelectedColor.itemData.itemId,
            Price = (int)_currentSelectedColor.itemData.sbPrice,
            VirtualCurrency = "SB"
        };
        _currentSelectedColor.gameObject.SetActive(false);
        _currentSelectedColor = null;
        PlayFabClientAPI.PurchaseItem(request, PurchaseSuccessCallback, PurchaseErrorCallback);
    }

    private void PurchaseSuccessCallback(PurchaseItemResult res)
    {
        AuthenticationManager.Instance.UpdateUserInventory();
        _uiCustomizationManager.inventoryItemsIds.Add(res.Items[0].ItemId);
        RefreshContent();
    }

    private void PurchaseErrorCallback(PlayFabError error)
    {
        Debug.LogError("Failed to buy color: " + error.ErrorMessage);
        RefreshContent();
    }

    private void RefreshContent()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
