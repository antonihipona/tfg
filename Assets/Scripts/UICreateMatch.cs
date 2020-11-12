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
        textError.gameObject.SetActive(false);
        inputRoomName.enabled = false;
        string errors = CheckInputs(inputRoomName.text);
        if (errors.Length != 0) textError.gameObject.SetActive(true);
        textError.text = errors;
        if (errors.Length == 0)
            PhotonNetworkManager.Instance.CreateRoom(inputRoomName.text, (MapType)dropdownMapType.value, (byte)(dropdownNumberOfPlayers.value + 2));
        if (inputRoomName.text.Length != 0) inputRoomName.enabled = true;
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
        inputRoomName.enabled = false;
        SceneManager.LoadScene("MainMenu");
    }

}
