using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class BuyColorSection : MonoBehaviour
{
    public GameObject colorPrefab;
    public GameObject target;

    private BuyableColor _currentSelectedColor;

    private void Start()
    {
        ClearChildren();
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), AddColors, GetCatalogItemsError);
    }

    public void ChangeSelected(BuyableColor _color)
    {
        if (_currentSelectedColor != null)
            _currentSelectedColor.SetSelected(false);
        _color.SetSelected(true);
        _currentSelectedColor = _color;
    }

    private void AddColors(GetCatalogItemsResult res)
    {
        // Add default (white)
        var colorGameObject = colorPrefab;
        colorGameObject.GetComponent<BuyableColor>().color = Color.white;
        colorPrefab.GetComponent<BuyableColor>().target = target;

        Instantiate(colorGameObject, transform);

        for (int i = 0; i < res.Catalog.Count; i++)
        {
            var item = res.Catalog[i];

            colorGameObject = colorPrefab;
            switch (item.ItemId)
            {
                case "color_red":
                    colorGameObject.GetComponent<BuyableColor>().color = Color.red;
                    break;
                case "color_blue":
                    colorGameObject.GetComponent<BuyableColor>().color = Color.blue;
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
