using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

public class UICustomization : MonoBehaviour
{
    public Text sbAmount;

    private enum ContentType { CustomizeColor, CustomizeStats, BuyColors }
    [Header("Navbar Buttons")]
    public Button customizeColorsButton;
    public Button customizeStatsButton;
    public Button buyColorsButton;

    [Header("Content")]
    public GameObject customizeColorsContent;
    public GameObject customizeStatsContent;
    public GameObject buyColorsContent;

    public HashSet<string> inventoryItemsIds;



    private void OnEnable()
    {
        AuthenticationManager.OnInventoryUpdate += UpdateSBText;
    }

    private void OnDisable()
    {
        AuthenticationManager.OnInventoryUpdate -= UpdateSBText;
    }

    private void Awake()
    {
        inventoryItemsIds = new HashSet<string>();
        ToggleCustomizeColors(ContentType.CustomizeColor);
    }

    private void Start()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            UpdateSB,
            AuthenticationManager.Instance.GetUserInventoryError);
        customizeColorsButton.onClick.AddListener(() => ToggleCustomizeColors(ContentType.CustomizeColor));
        customizeStatsButton.onClick.AddListener(() => ToggleCustomizeColors(ContentType.CustomizeStats));
        buyColorsButton.onClick.AddListener(() => ToggleCustomizeColors(ContentType.BuyColors));
    }

    private void ToggleCustomizeColors(ContentType contentType)
    {
        customizeColorsButton.GetComponent<Image>().color = Color.gray;
        customizeStatsButton.GetComponent<Image>().color = Color.gray;
        buyColorsButton.GetComponent<Image>().color = Color.gray;

        customizeColorsContent.SetActive(false);
        customizeStatsContent.SetActive(false);
        buyColorsContent.SetActive(false);

        switch (contentType)
        {
            case ContentType.CustomizeColor:
                customizeColorsButton.GetComponent<Image>().color = Color.white;
                customizeColorsContent.SetActive(true);
                break;
            case ContentType.CustomizeStats:
                customizeStatsButton.GetComponent<Image>().color = Color.white;
                customizeStatsContent.SetActive(true);
                break;
            case ContentType.BuyColors:
                buyColorsButton.GetComponent<Image>().color = Color.white;
                buyColorsContent.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void UpdateSB(GetUserInventoryResult res)
    {
        AuthenticationManager.Instance.virtualCurrency = res.VirtualCurrency;
        UpdateSBText();
    }

    private void UpdateSBText()
    {
        sbAmount.text = AuthenticationManager.Instance.virtualCurrency["SB"].ToString();
    }

    public class ItemData
    {
        public uint sbPrice;
        public string itemId;
    }
}
