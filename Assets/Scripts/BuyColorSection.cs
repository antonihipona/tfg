using Boo.Lang;
using PlayFab;
using PlayFab.ClientModels;
using System.Globalization;
using TMPro;
using UnityEngine;

public class BuyColorSection : MonoBehaviour
{
    public TextMeshProUGUI text;

    public GameObject colorPrefab;
    public GameObject target;

    private BuyableColor _currentSelectedColor;
    private UICustomizationManager _uiCustomizationManager;

    private void OnEnable()
    {
        if (_currentSelectedColor != null)
            text.text = "Price: " + _currentSelectedColor.sbPrice + " SB";
        _uiCustomizationManager = FindObjectOfType<UICustomizationManager>();
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), AddColors, GetCatalogItemsError);
    }

    public void ChangeSelected(BuyableColor _color)
    {
        if (_currentSelectedColor != null)
            _currentSelectedColor.SetSelected(false);
        _color.SetSelected(true);
        _currentSelectedColor = _color;
        text.text = "Price: " + _color.sbPrice + " SB";
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
            colorGameObject.GetComponent<BuyableColor>().target = target;
            colorGameObject.GetComponent<BuyableColor>().sbPrice = item.VirtualCurrencyPrices["SB"];
            switch (item.ItemId)
            {
                case "color_red":
                    colorGameObject.GetComponent<BuyableColor>().color = Color.red;
                    break;
                case "color_blue":
                    colorGameObject.GetComponent<BuyableColor>().color = Color.blue;
                    break;
                case "color_green":
                    colorGameObject.GetComponent<BuyableColor>().color = Color.green;
                    break;
                default:
                    break;
            }
            Instantiate(colorGameObject, transform);
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
}
