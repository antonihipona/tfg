using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System;

public class UICreateMatch : UIBase
{

    public TMPro.TMP_InputField inputRoomName;
    public TMPro.TMP_Dropdown dropdownMapType;
    public TMPro.TMP_Dropdown dropdownNumberOfPlayers;
    public Text textError;

    public void OnClickCreateMatch()
    {
        string errors = CheckInputs(inputRoomName.text);
        textError.text = errors;
        if (errors.Equals(""))
            PhotonNetworkManager.instance.CreateRoom(inputRoomName.text, (MapType)dropdownMapType.value, (byte)(dropdownNumberOfPlayers.value + 2));
    }

    private string CheckInputs(string text)
    {
        string errors = "";
        if (text.Trim().Equals(""))
            errors = "Match name can't be empty";
        return errors;
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
