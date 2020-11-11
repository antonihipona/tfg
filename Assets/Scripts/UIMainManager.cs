using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;


public class UIMainManager : UIBase
{
    public Text sbAmount;

    private void OnEnable()
    {
        AuthenticationManager.OnInventoryUpdate += UpdateSB;
    }

    private void OnDisable()
    {
        AuthenticationManager.OnInventoryUpdate -= UpdateSB;
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        AudioManager.Instance.PlayMainTheme();
        UpdateSB();
    }
    public void OnClickSearchMatch()
    {
        // We need to join the lobby
        PhotonNetwork.JoinLobby();
    }

    public void OnClickCreateMatch()
    {
        SceneManager.LoadScene("CreateMatchMenu");
    }

    public void OnClickCustomize()
    {
        SceneManager.LoadScene("CustomizationScreen");
    }

    public void OnClickGameSettings()
    {
        SceneManager.LoadScene("GameSettingsMenu");
    }

    public void OnClickRanking()
    {
        SceneManager.LoadScene("RankingMenu");
    }

    private void UpdateSB()
    {
        sbAmount.text = AuthenticationManager.instance.virtualCurrency["SB"].ToString();
    }
}
