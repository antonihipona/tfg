using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIRanking : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textPrefab;
    public HorizontalLayoutGroup horizontalLayoutGroupPrefab;
    public VerticalLayoutGroup listContent;

    // Start is called before the first frame update
    void Start()
    {
        RequestLeaderboard();
    }

    // source https://docs.microsoft.com/en-us/gaming/playfab/features/social/tournaments-leaderboards/quickstart
    // Get the players with the top 10 high scores in the game
    public void RequestLeaderboard()
    {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = "Points",
            StartPosition = 0,
            MaxResultsCount = 20
        }, result => DisplayLeaderboard(result), FailureCallback);
    }

    private void DisplayLeaderboard(GetLeaderboardResult result)
    {
        for (int i = 0; i < result.Leaderboard.Count; i++)
        {
            var player = result.Leaderboard[i];

            TMPro.TextMeshProUGUI playerName = Instantiate(textPrefab);
            TMPro.TextMeshProUGUI position = Instantiate(textPrefab);
            TMPro.TextMeshProUGUI points = Instantiate(textPrefab);
            HorizontalLayoutGroup horizontalLayoutGroup = Instantiate(horizontalLayoutGroupPrefab);

            playerName.text = player.DisplayName;
            position.text =  $"{player.Position + 1}";
            points.text = $"{player.StatValue}";

            horizontalLayoutGroup.transform.SetParent(listContent.transform, false);
            position.transform.SetParent(horizontalLayoutGroup.transform, false);
            playerName.transform.SetParent(horizontalLayoutGroup.transform, false);
            points.transform.SetParent(horizontalLayoutGroup.transform, false);
        }
    }

    private void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
