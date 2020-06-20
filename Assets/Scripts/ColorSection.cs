using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class ColorSection : MonoBehaviour
{
    public GameObject colorPrefab;
    public GameObject target;

    private CustomizableColor _currentSelectedColor;
    private UICustomizationManager _uiCustomizationManager;

    private void OnEnable()
    {
        _uiCustomizationManager = FindObjectOfType<UICustomizationManager>();
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), AddColors, GetUserInventoryError);
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
        colorGameObject.GetComponent<CustomizableColor>().target = target;
        colorGameObject.GetComponent<CustomizableColor>().color = Color.white;
        Instantiate(colorGameObject, transform);

        for (int i = 0; i < res.Inventory.Count; i++)
        {
            var item = res.Inventory[i];
            _uiCustomizationManager.inventoryItemsIds.Add(item.ItemId);
            
            colorGameObject = colorPrefab;
            colorGameObject.GetComponent<CustomizableColor>().target = target;
            switch (item.ItemId)
            {
                case "color_red":
                    colorGameObject.GetComponent<CustomizableColor>().color = Color.red;
                    break;
                case "color_blue":
                    colorGameObject.GetComponent<CustomizableColor>().color = Color.blue;
                    break;
                case "color_green":
                    colorGameObject.GetComponent<CustomizableColor>().color = Color.green;
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

    private void GetUserInventoryError(PlayFabError error)
    {
        Debug.LogError("Could not get player inventory" + error.ErrorMessage);
    }
}
