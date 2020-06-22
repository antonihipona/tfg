using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Boo.Lang;
using System.Collections.Generic;

public class UICustomizationManager : MonoBehaviour
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
        AuthenticationManager.OnInventoryUpdate += UpdateSB;
    }

    private void OnDisable()
    {
        AuthenticationManager.OnInventoryUpdate -= UpdateSB;
    }

    private void Awake()
    {
        inventoryItemsIds = new HashSet<string>();
        ToggleCustomizeColors(ContentType.CustomizeColor);
    }

    private void Start()
    {
        UpdateSB();
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

    private void UpdateSB()
    {
        sbAmount.text = AuthenticationManager.instance.virtualCurrency["SB"].ToString();
    }
}
