using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateName : MonoBehaviour
{
    public GameObject NameField;

    public string Name;

    private void Start()
    {
        PlayFabClientAPI.GetPlayerProfile(
            new GetPlayerProfileRequest(),
            OnGetProfile,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    private void OnGetProfile(GetPlayerProfileResult result)
    {
        if (result.PlayerProfile.DisplayName == "" || result.PlayerProfile.DisplayName is null)
        {
            Name = "Test";
        }
        else
        {
            Name = result.PlayerProfile.DisplayName;
        }

        PhotonNetwork.NickName = Name;

        NameField.GetComponent<Text>().text = Name;
    }
}
